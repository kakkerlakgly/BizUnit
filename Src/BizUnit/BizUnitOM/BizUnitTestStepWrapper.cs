//---------------------------------------------------------------------
// File: BizUnitTestStepWrapper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Xml;
using BizUnit.Common;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    ///     BizUnitTestStepWrapper wraps BizUnit test steps and provides access to any exceptions raised at runtime.
    /// </summary>
    [Obsolete("BizUnitTestStepWrapper has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class BizUnitTestStepWrapper
    {
        private readonly ITestStep _testStep;
        private readonly TestStepBuilder _testStepBuilder;
        private string _assemblyPath;
        private bool _failOnError = true;
        private XmlNode _stepConfig;

        internal BizUnitTestStepWrapper(XmlNode stepConfig)
        {
            ArgumentValidation.CheckForNullReference(stepConfig, "stepConfig");

            LoadStepConfig(stepConfig);
            var obj = ObjectCreator.CreateStep(TypeName, _assemblyPath);
            _testStep = obj as ITestStep;

            if (null == _testStep)
            {
                throw new ArgumentException(
                    string.Format(
                        "The test step could not be created, check the test step type and assembly path are correct, type: {0}, assembly path: {1}",
                        TypeName, _assemblyPath));
            }
        }

        internal BizUnitTestStepWrapper(ITestStep testStep, XmlNode stepConfig)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(stepConfig, "stepConfig");

            LoadStepConfig(stepConfig);
            _testStep = testStep;
        }

        internal BizUnitTestStepWrapper(ITestStep testStep, XmlNode stepConfig, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(stepConfig, "stepConfig");

            LoadStepConfig(stepConfig);
            _testStep = testStep;
            RunConcurrently = runConcurrently;
            _failOnError = failOnError;
        }

        internal BizUnitTestStepWrapper(ITestStepOM testStep, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");

            _testStepBuilder = new TestStepBuilder(testStep);
            RunConcurrently = runConcurrently;
            _failOnError = failOnError;
            TypeName = testStep.GetType().ToString();
        }

        internal BizUnitTestStepWrapper(TestStepBuilder testStepBuilder, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStepBuilder, "testStepBuilder");

            _testStepBuilder = testStepBuilder;
            RunConcurrently = runConcurrently;
            _failOnError = failOnError;
            TypeName = testStepBuilder.TestStepOM.GetType().ToString();
        }

        internal bool RunConcurrently { get; private set; }

        internal bool FailOnError
        {
            get { return _failOnError; }
        }

        internal string TypeName { get; private set; }

        /// <summary>
        ///     Returns the exception generated during execution, otherwise null.
        /// </summary>
        /// <value>The exception which occured during execution.</value>
        public Exception ExecuteException { get; private set; }

        internal void Execute(Context ctx)
        {
            try
            {
                var tsea = new TestStepEventArgs(ctx.CurrentTestStage, ctx.TestName, TypeName);
                ctx.BizUnitObject.OnTestStepStart(tsea);

                if (null != _stepConfig)
                {
                    _testStep.Execute(_stepConfig, ctx);
                }
                else
                {
                    _testStepBuilder.PrepareStepForExecution(ctx);

                    _testStepBuilder.PrepareSubStepsForExecution(ctx);

                    ctx.BizUnitObject.OnTestStepStart(tsea);

                    _testStepBuilder.TestStepOM.Validate(ctx);
                    _testStepBuilder.TestStepOM.Execute(ctx);
                }

                ctx.BizUnitObject.OnTestStepStop(tsea);
            }
            catch (Exception executionException)
            {
                ExecuteException = executionException;
                throw;
            }
        }

        private void LoadStepConfig(XmlNode config)
        {
            _stepConfig = config;
            var assemblyPathNode = _stepConfig.SelectSingleNode("@assemblyPath");
            var typeNameNode = _stepConfig.SelectSingleNode("@typeName");
            var runConcurrentlyNode = _stepConfig.SelectSingleNode("@runConcurrently");
            var failOnErrorNode = _stepConfig.SelectSingleNode("@failOnError");

            if (null != runConcurrentlyNode)
            {
                RunConcurrently = Convert.ToBoolean(runConcurrentlyNode.Value);
            }

            if (null != failOnErrorNode)
            {
                _failOnError = Convert.ToBoolean(failOnErrorNode.Value);
            }

            TypeName = typeNameNode.Value;
            _assemblyPath = null != assemblyPathNode ? assemblyPathNode.Value : string.Empty;
        }
    }
}