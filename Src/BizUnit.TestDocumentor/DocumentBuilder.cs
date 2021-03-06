﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BizUnit.Xaml;

namespace BizUnit.TestDocumentor
{
    /// <summary>
    /// </summary>
    public class DocumentBuilder
    {
        private const string XmlExtension = "*.xml";
        private readonly ILogger _logger;
        private readonly IDictionary<string, Exception> _testCaseLoadErrors = new Dictionary<string, Exception>();
        private readonly IDictionary<string, IList<TestCase>> _testCases = new Dictionary<string, IList<TestCase>>();

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        public DocumentBuilder(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <param name="templateReportPath"></param>
        /// <param name="templateCategoryPath"></param>
        /// <param name="templateTestCasePath"></param>
        /// <param name="testCaseDirectory"></param>
        /// <param name="outpurFileName"></param>
        /// <param name="searchRecursively"></param>
        /// <returns></returns>
        public string GenerateDocumentation(
            string templateReportPath,
            string templateCategoryPath,
            string templateTestCasePath,
            string testCaseDirectory,
            string outpurFileName,
            bool searchRecursively)
        {
            return GenerateDocumentation(templateReportPath, templateCategoryPath, templateTestCasePath,
                testCaseDirectory,
                outpurFileName, null, searchRecursively);
        }

        /// <summary>
        /// </summary>
        /// <param name="templateReportPath"></param>
        /// <param name="templateCategoryPath"></param>
        /// <param name="templateTestCasePath"></param>
        /// <param name="testCaseDirectory"></param>
        /// <param name="outpurFileName"></param>
        /// <param name="bizUnitAssemblyPath"></param>
        /// <param name="searchRecursively"></param>
        /// <returns></returns>
        public string GenerateDocumentation(
            string templateReportPath,
            string templateCategoryPath,
            string templateTestCasePath,
            string testCaseDirectory,
            string outpurFileName,
            string bizUnitAssemblyPath,
            bool searchRecursively)
        {
            LoadBizUnitAssemblies(bizUnitAssemblyPath);
            LoadTestCases(testCaseDirectory, searchRecursively);
            BuildReport(templateReportPath, templateCategoryPath, templateTestCasePath, testCaseDirectory,
                outpurFileName, searchRecursively);

            return null;
        }

        private static void LoadBizUnitAssemblies(string bizUnitAssemblyPath)
        {
            if (string.IsNullOrEmpty(bizUnitAssemblyPath))
            {
                return;
            }

            var assemblies = Directory.GetFiles(bizUnitAssemblyPath, "*.dll");

            foreach (var assembly in assemblies)
            {
                Assembly.LoadFrom(assembly);
            }

            var asms = AppDomain.CurrentDomain.GetAssemblies();
        }

        private void BuildReport(
            string templateReportPath,
            string templateCategoryPath,
            string templateTestCasePath,
            string testCaseDirectory,
            string outpurFileName,
            bool searchRecursively)
        {
            var sb = new StringBuilder();
            _logger.Info("Loading TemplateReport: {0}", templateReportPath);
            var templateReport =
                new StreamReader(File.Open(templateReportPath, FileMode.Open, FileAccess.Read)).ReadToEnd();

            _logger.Info("Loading TemplateCategory: {0}", templateCategoryPath);
            var categoryTemplate =
                new StreamReader(File.Open(templateCategoryPath, FileMode.Open, FileAccess.Read)).ReadToEnd();

            _logger.Info("Loading TemplateTestCase: {0}", templateTestCasePath);
            var testCaseTemplate =
                new StreamReader(File.Open(templateTestCasePath, FileMode.Open, FileAccess.Read)).ReadToEnd();

            foreach (var category in _testCases)
            {
                sb.Append(categoryTemplate.Replace("##Category##", category.Key));

                foreach (var test in category.Value)
                {
                    sb.Append(BuildTestCaseFragment(testCaseTemplate, test, testCaseDirectory, searchRecursively));
                }
            }

            var report = templateReport.Replace("<!-- ##BizUnitTests## -->", sb.ToString());
            using (var stream = File.Open(outpurFileName, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.WriteLine(report);
                }
            }
        }

        private static string BuildTestCaseFragment(string testCaseTemplate, TestCase test, string testCaseDirectory,
            bool searchRecursively)
        {
            var result = testCaseTemplate.Replace("##TestCaseName##", test.Name);
            result = result.Replace("##TestCaseDescription##", test.Description);
            result = result.Replace("##TestCasePurpose##", test.Purpose);
            result = result.Replace("##TestCaseReference##", test.Reference);
            result = result.Replace("##TestCasePreconditions##", test.Preconditions);

            result = result.Replace("##TestCaseDependancies##",
                GetTestCaseDependancies(test, testCaseDirectory, searchRecursively));

            result = result.Replace("##BizUnitVersion##", test.BizUnitVersion);
            return result.Replace("##TestCaseExpectedResults##", test.ExpectedResults);
        }

        private static string GetTestCaseDependancies(TestCase test, string testCaseDirectory, bool searchRecursively)
        {
            var sb = new StringBuilder();

            var importFound = AddImportedTestCases(test.SetupSteps, sb, "Setup test cases: ", false, testCaseDirectory,
                searchRecursively);
            importFound = AddImportedTestCases(test.ExecutionSteps, sb, "Execution test cases: ", importFound,
                testCaseDirectory, searchRecursively);
            importFound = AddImportedTestCases(test.CleanupSteps, sb, "Cleanup test cases: ", importFound,
                testCaseDirectory, searchRecursively);

            return (0 < sb.Length) ? sb.ToString() : "None";
        }

        private static bool AddImportedTestCases(IEnumerable<TestStepBase> testCollection, StringBuilder sb,
            string importLabel, bool importFound, string testCaseDirectory, bool searchRecursively)
        {
            var found = false;

            foreach (var step in testCollection)
            {
                var importContainer = step as ImportTestCaseStep;

                if (null != importContainer)
                {
                    var itc = GetBizUnitTestCase(importContainer, testCaseDirectory, searchRecursively);

                    if (null != itc)
                    {
                        if (!found)
                        {
                            if (importFound)
                            {
                                sb.Append(". ");
                            }

                            sb.Append(importLabel);
                            found = true;
                        }
                        else
                        {
                            sb.Append(", ");
                        }

                        sb.Append(itc.Name);
                    }
                }
            }

            return found;
        }

        private static TestCase GetBizUnitTestCase(ImportTestCaseStep importContainer, string testCaseDirectory,
            bool searchRecursively)
        {
            TestCase itc = null;
            try
            {
                itc = TestCase.LoadFromFile(importContainer.TestCasePath);
            }
            catch (DirectoryNotFoundException)
            {
            }

            if (null == itc)
            {
                var fileName = Path.GetFileName(importContainer.TestCasePath);

                var filePath = Directory.GetFiles(testCaseDirectory, fileName,
                    searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                if (1 == filePath.Length)
                {
                    itc = TestCase.LoadFromFile(filePath[0]);
                }
                else if (1 < filePath.Length)
                {
                    throw new InvalidOperationException(string.Format("Ambiguous file name specified as import: {0}",
                        fileName));
                }
            }

            return itc;
        }

        private void LoadTestCases(string testCaseDirectory, bool searchRecursively)
        {
            var files = Directory.GetFiles(testCaseDirectory, XmlExtension,
                searchRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            _logger.Info("{0} test cases found.", files.Length);

            foreach (var file in files)
            {
                try
                {
                    _logger.Info("Processing test case: {0}", file);

                    // TODO: Check the XML file is a BizUnit file before attempting to load - do this in TestCase.LoadFromFile
                    var tc = TestCase.LoadFromFile(file);

                    IList<TestCase> category;

                    if (_testCases.ContainsKey(tc.Category))
                    {
                        category = _testCases[tc.Category];
                    }
                    else
                    {
                        category = new List<TestCase>();
                        _testCases.Add(tc.Category, category);
                    }

                    category.Add(tc);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _testCaseLoadErrors.Add(file, ex);
                }
            }
        }
    }
}