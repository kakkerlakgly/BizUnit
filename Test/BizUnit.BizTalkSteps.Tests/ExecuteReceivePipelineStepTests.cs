
using BizUnit.TestSteps.BizTalk.Pipeline;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.BizTalkSteps.Tests
{
    [TestClass]
    public class ExecuteReceivePipelineStepTests
    {
        [TestMethod]
        public void ExecuteReceivePiplineWithXmlDisAsmTest()
        {
            // Create test case...
            var tc = new TestCase {Name = "ExecuteReceivePiplineWithXmlDisAsmTest"};

            var pipeStep = new ExecuteReceivePipelineStep
            {
                PipelineAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                PipelineTypeName = "BizUnit.BizTalkTestArtifacts.ReceivePipeline1"
            };
            var ds = new DocSpecDefinition
                         {
                             AssemblyPath =
                                 @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                             TypeName = "BizUnit.BizTalkTestArtifacts.Schema2"
                         };
            pipeStep.DocSpecs.Add(ds);
            pipeStep.Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema2.xml";
            pipeStep.DestinationFileFormat = "Output010.{0}.xml";
            pipeStep.DestinationFileFormat = "Output010.{0}.xml";
            pipeStep.OutputContextFileFormat = "Context010.{0}.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep
            {
                DirectoryPath = ".",
                Timeout = 2000,
                SearchPattern = "Output010*.xml",
                ExpectedNoOfFiles = 1
            };
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            exists = new ExistsStep
            {
                DirectoryPath = ".",
                Timeout = 2000,
                SearchPattern = "Context010*.xml",
                ExpectedNoOfFiles = 1
            };
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep
            {
                DirectoryPath = ".",
                SearchPattern = "Output010.0.xml",
                DeleteFiles = false
            };

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd",
                XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2"
            };
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteReceivePiplineWithXmlDisAsmTest.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteReceivePiplineWithXmlDisAsmTest.xaml"));
            bu.RunTest();
        }

        [TestMethod]
        public void ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree()
        {
            // Create test case...
            var tc = new TestCase {Name = "ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree"};

            var pipeStep = new ExecuteReceivePipelineStep
            {
                PipelineAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                PipelineTypeName = "BizUnit.BizTalkTestArtifacts.ReceivePipeline1"
            };
            var ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema3Env"
            };
            pipeStep.DocSpecs.Add(ds);
            pipeStep.Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema3Env.xml";
            pipeStep.DestinationFileFormat = "Output011.{0}.xml";
            pipeStep.DestinationFileFormat = "Output011.{0}.xml";
            pipeStep.OutputContextFileFormat = "Context011.{0}.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep
            {
                DirectoryPath = ".",
                Timeout = 2000,
                SearchPattern = "Output011*.xml",
                ExpectedNoOfFiles = 3
            };
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            exists = new ExistsStep
            {
                DirectoryPath = ".",
                Timeout = 2000,
                SearchPattern = "Context011*.xml",
                ExpectedNoOfFiles = 3
            };
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep
            {
                DirectoryPath = ".",
                SearchPattern = "Output011.0.xml",
                DeleteFiles = false
            };

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd",
                XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2"
            };
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree.xaml"));
            bu.RunTest();
        }

        [TestMethod]
        public void ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest()
        {
            // Create test case...
            var tc = new TestCase {Name = "ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest"};

            var pipeStep = new ExecuteReceivePipelineStep
            {
                PipelineAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                PipelineTypeName = "BizUnit.BizTalkTestArtifacts.ReceivePipeline3"
            };
            var ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema0"
            };
            pipeStep.DocSpecs.Add(ds);

            ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema3Env"
            };
            pipeStep.DocSpecs.Add(ds);

            pipeStep.Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema3Env.xml";
            pipeStep.DestinationFileFormat = "Output013.{0}.xml";
            pipeStep.DestinationFileFormat = "Output013.{0}.xml";
            pipeStep.OutputContextFileFormat = "Context013.{0}.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep
            {
                DirectoryPath = ".",
                Timeout = 2000,
                SearchPattern = "Output013*.xml",
                ExpectedNoOfFiles = 3
            };
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            exists = new ExistsStep
            {
                DirectoryPath = ".",
                Timeout = 2000,
                SearchPattern = "Context013*.xml",
                ExpectedNoOfFiles = 3
            };
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep
            {
                DirectoryPath = ".",
                SearchPattern = "Output013.0.xml",
                DeleteFiles = false
            };

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema0.xsd",
                XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema0"
            };
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest.xaml"));
            bu.RunTest();
        }
    }
}