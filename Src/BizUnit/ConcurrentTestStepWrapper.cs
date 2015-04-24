//---------------------------------------------------------------------
// File: ConcurrentTestStepWrapper.cs
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
using BizUnit.BizUnitOM;
using BizUnit.Xaml;

namespace BizUnit
{
    /// <summary>
    ///     Summary description for ConcurrentTestStepWrapper.
    /// </summary>
    internal class ConcurrentTestStepWrapper
    {
        private readonly Context _context;

        public ConcurrentTestStepWrapper(TestStepBase testStep, Context ctx)
        {
            TestStep = testStep;
            Logger = new Logger {ConcurrentExecutionMode = true};
            _context = ctx.CloneForConcurrentUse(Logger);
        }

        public ConcurrentTestStepWrapper(BizUnitTestStepWrapper stepWrapper, Context ctx)
        {
            StepWrapper = stepWrapper;
            // TODO:.... ILogger
            Logger = new Logger {ConcurrentExecutionMode = true};
            _context = ctx.CloneForConcurrentUse(Logger);
        }

        public string Name
        {
            get { return (null != TestStep) ? TestStep.GetType().ToString() : StepWrapper.TypeName; }
        }

        public bool FailOnError
        {
            get { return (null != TestStep) ? TestStep.FailOnError : StepWrapper.FailOnError; }
        }

        public string StepName
        {
            get
            {
                if (null != TestStep)
                    return TestStep.GetType().ToString();

                if (null != StepWrapper)
                    return StepWrapper.TypeName;

                return null;
            }
        }

        public BizUnitTestStepWrapper StepWrapper { get; private set; }
        public TestStepBase TestStep { get; private set; }
        public Exception FailureException { get; private set; }
        public ILogger Logger { get; private set; }

        public void Execute()
        {
            try
            {
                if (null != TestStep)
                {
                    Logger.TestStepStart(TestStep.GetType().ToString(), DateTime.Now, true, TestStep.FailOnError);
                    var step = TestStep as ImportTestCaseStep;
                    if (step != null)
                    {
                        ExecuteImportedTestCase(step, _context);
                    }
                    else
                    {
                        TestStep.Execute(_context);
                    }
                }
                else
                {
                    Logger.TestStepStart(StepWrapper.TypeName, DateTime.Now, true, StepWrapper.FailOnError);
                    StepWrapper.Execute(_context);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                FailureException = e;
            }
        }

        private static void ExecuteImportedTestCase(ImportTestCaseStep testStep, Context context)
        {
            var testCase = testStep.GetTestCase();
            var bu = new BizUnit(testCase, context);
            bu.RunTest();
        }
    }
}