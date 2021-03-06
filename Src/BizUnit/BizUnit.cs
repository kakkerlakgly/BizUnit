//---------------------------------------------------------------------
// File: BizUnit.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using BizUnit.BizUnitOM;
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit
{
    /// <summary>
    ///     BizUnit test framework for the rapid development of automated test cases. Test cases may be created as 'coded
    ///     tests'
    ///     or in XAML.
    ///     <para>
    ///         Test cases have three stages:
    ///         <para>1. TestSetup - used to setup the conditions ready to execute the test</para>
    ///         <para>2. TestExecution - the main execution stage of the test</para>
    ///         <para>
    ///             3: TestCleanup - the final stage is always executed regardless of whether the test passes
    ///             or fails in order to leave the system in the state prior to executing the test
    ///         </para>
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     The following example demonstrates how to create a BizUnit coded test and execute it:
    ///     <code escaped="true">
    ///  namespace WoodgroveBank.BVTs
    /// 	{
    ///      using System;
    ///      using NUnit.Framework;
    ///      using BizUnit;
    /// 
    ///      // This is an example of calling BizUnit from NUnit...
    ///      [TestFixture]
    ///      public class SmokeTests
    ///      {
    ///          // Create the test case
    ///          var testCase = new TestCase();
    ///      
    ///          // Create test steps...
    ///          var delayStep = new DelayStep {DelayMilliSeconds = 500};
    ///      
    ///          // Add test steps to the required test stage
    ///          testCase.ExecutionSteps.Add(delayStep);
    ///      
    ///          // Create a new instance of BizUnit and run the test
    ///          var bizUnit = new BizUnit(testCase);
    ///          bizUnit.RunTest();
    ///      }
    ///  }		
    /// 	</code>
    ///     <para>
    ///         The following XML shows the XAML for the coded test case shown above:
    ///     </para>
    ///     <code escaped="true">
    ///  <TestCase
    ///             Description="{x:Null}"
    ///             ExpectedResults="{x:Null}"
    ///             Name="{x:Null}" Preconditions="{x:Null}"
    ///             Purpose="{x:Null}" Reference="{x:Null}"
    ///             BizUnitVersion="4.0.133.0"
    ///             xmlns="clr-namespace:BizUnit.Xaml;assembly=BizUnit"
    ///             xmlns:btt="clr-namespace:BizUnit.TestSteps.Time;assembly=BizUnit.TestSteps"
    ///             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    ///             <TestCase.ExecutionSteps>
    ///                 <btt:DelayStep
    ///                     SubSteps="{x:Null}"
    ///                     DelayMilliSeconds="500"
    ///                     FailOnError="True"
    ///                     RunConcurrently="False" />
    ///             </TestCase.ExecutionSteps>
    ///         </TestCase>    
    ///  </code>
    /// </remarks>
    public class BizUnit
    {
        internal const string BizUnitTestCaseStartTime = "BizUnitTestCaseStartTime";
        private const string BizUnitTestCaseName = "BizUnitTestCaseName";

        private readonly ConcurrentQueue<ConcurrentTestStepWrapper> _completedConcurrentSteps =
            new ConcurrentQueue<ConcurrentTestStepWrapper>();

        private XmlNodeList _executeSteps;
        private Exception _executionException;
        private int _inflightQueueDepth;
        private XmlNodeList _setupSteps;
        private XmlNodeList _teardownSteps;
        private BizUnitTestCase _testCaseObjectModel;
        private TestGroupPhase _testGroupPhase = TestGroupPhase.Unknown;
        private string _testName = "Unknown";
        private TestCase _xamlTestCase;
        internal ILogger Logger;

        /// <summary>
        ///     BizUnit constructor.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        /// 		[Test]
        /// 		public void Test_01_Adapter_MSMQ()
        /// 		{
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_01_Adapter_MSMQ.xml");
        /// 			bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            LoadXmlFromFileAndInit(configFile, TestGroupPhase.Unknown, null);
        }

        /// <summary>
        ///     BizUnit constructor.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit.
        ///     Note: the BizUnit _context object may be created and passed into
        ///     BizUnit, any _context properties set on the _context object may be
        ///     used by BizUnit steps. Context properties may be of any type, i.e. any
        ///     .Net type, of course the consumer of that _context object will need to know
        ///     what type to expect.
        ///     Also note that many test steps have the ability to fetch their configuration
        ///     from the BizUnit _context if their configuration is decorated with the
        ///     attribute takeFromCtx.
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        ///      AddressBook addressBook = new AddressBook("Redmond");
        ///  
        ///      Context ctx = new Context();
        ///      ctx.Add("CorrelationId", "1110023");
        ///      ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///      ctx.Add("AddressBook", addressBook);
        ///  
        /// 		[Test]
        /// 		public void Test_02_Adapter_MSMQ()
        /// 		{
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_01_Adapter_MSMQ.xml", ctx);
        /// 			bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            Logger = ctx.Logger;
            LoadXmlFromFileAndInit(configFile, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        ///     BizUnit constructor.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        /// 		[Test]
        /// 		public void Test_03_Adapter_MSMQ()
        /// 		{
        ///          // The test case is an embeded resource...
        /// 			BizUnit bizUnit = new BizUnit(Assembly.GetExecutingAssembly().GetManifestResourceStream("BizUnit.SampleTests.BizUnitFunctionalTests.Test_04_MQSeriesTest.xml"));
        /// 			bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");

            LoadXmlFromStreamAndInit(configStream, TestGroupPhase.Unknown, null);
        }

        /// <summary>
        ///     BizUnit constructor.
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit using
        ///     the BizUnit Test Case Object Model:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        /// 	[TestMethod]
        /// 	public class SmokeTests
        /// 	{
        /// 		[Test]
        /// 		public void Test_03_Adapter_MSMQ()
        /// 		{
        ///          // The test case is an embeded resource...
        ///          BizUnitTestCase testCase = new BizUnitTestCase();
        /// 
        ///          FileCreateStep fcs = new FileCreateStep();
        ///          fcs.SourcePath = @"C:\Tests\BizUnit.Tests\Data\PO_MSFT001.xml";
        ///          fcs.CreationPath = @"C:\Tests\BizUnit.Tests\Data\PO_MSFT001_%Guid%.xml";
        ///          testCase.AddTestStep(fcs, TestStage.Execution);
        /// 
        ///          BizUnit bizUnit = new BizUnit(testCase);
        ///          bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(BizUnitTestCase testCase)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");

            LoadObjectModelTestCaseAndInit(testCase, TestGroupPhase.Unknown, null);
        }

        /// <summary>
        ///     BizUnit constructor.
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit using
        ///     the BizUnit Test Case Object Model:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        /// 	[TestMethod]
        /// 	public class SmokeTests
        /// 	{
        /// 		[Test]
        /// 		public void Test_03_Adapter_MSMQ()
        /// 		{
        ///          // The test case is an embeded resource...
        ///          BizUnitTestCase testCase = new BizUnitTestCase();
        /// 
        ///          Context ctx = new Context();
        ///          ctx.Add("PathToWriteFileTo", testDirectory + @"\Data_%Guid%.xml");
        ///  
        ///          FileCreateStep fcs = new FileCreateStep();
        ///          fcs.SourcePath = @"C:\Tests\BizUnit.Tests\Data\PO_MSFT001.xml";
        ///          fcs.CreationPath = "takeFromCtx:PathToWriteFileTo";
        ///          testCase.AddTestStep(fcs, TestStage.Execution);
        /// 
        ///          BizUnit bizUnit = new BizUnit(testCase, ctx);
        ///          bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(BizUnitTestCase testCase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");

            Logger = ctx.Logger;
            LoadObjectModelTestCaseAndInit(testCase, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        ///     BizUnit constructor.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit.
        ///     Note: the BizUnit _context object may be created and passed into
        ///     BizUnit, any _context properties set on the _context object may be
        ///     used by BizUnit steps. Context properties may be of any type, i.e. any
        ///     .Net type, of course the consumer of that _context object will need to know
        ///     what type to expect.
        ///     Also note that many test steps have the ability to fetch their configuration
        ///     from the BizUnit _context if their configuration is decorated with the
        ///     attribute takeFromCtx.
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        ///      AddressBook addressBook = new AddressBook("Redmond");
        ///  
        ///      Context ctx = new Context();
        ///      ctx.Add("CorrelationId", "1110023");
        ///      ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///      ctx.Add("AddressBook", addressBook);
        ///  
        /// 		[Test]
        /// 		public void Test_04_Adapter_MSMQ()
        /// 		{
        ///          // The test case is an embeded resource...
        /// 			BizUnit bizUnit = new BizUnit(Assembly.GetExecutingAssembly().GetManifestResourceStream("BizUnit.SampleTests.BizUnitFunctionalTests.Test_04_MQSeriesTest.xml"), ctx);
        /// 			bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            LoadXmlFromStreamAndInit(configStream, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        ///     BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">
        ///     The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This
        ///     constructor is used during the initialization or termination of a group of test cases, for example when using the
        ///     NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].
        /// </param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        /// 		[TestFixtureSetUp]
        /// 		public void Test_Group_Setup()
        /// 		{
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup);
        /// 			bizUnit.RunTest();
        /// 		}
        /// 		
        /// 		...
        /// 		
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile, TestGroupPhase testGroupPhase)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");

            LoadXmlFromFileAndInit(configFile, testGroupPhase, null);
        }

        /// <summary>
        ///     BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">
        ///     The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This
        ///     constructor is used during the initialization or termination of a group of test cases, for example when using the
        ///     NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].
        /// </param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit.
        ///     Note: the BizUnit _context object may be created and passed into
        ///     BizUnit, any _context properties set on the _context object may be
        ///     used by BizUnit steps. Context properties may be of any type, i.e. any
        ///     .Net type, of course the consumer of that _context object will need to know
        ///     what type to expect.
        ///     Also note that many test steps have the ability to fetch their configuration
        ///     from the BizUnit _context if their configuration is decorated with the
        ///     attribute takeFromCtx.
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        ///  
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        ///      AddressBook addressBook = new AddressBook("Redmond");
        ///  
        ///      Context ctx = new Context();
        ///      ctx.Add("CorrelationId", "1110023");
        ///      ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///      ctx.Add("AddressBook", addressBook);
        ///  
        /// 		[TestFixtureSetUp]
        /// 		public void Test_Group_Setup()
        /// 		{
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup, ctx);
        /// 			bizUnit.RunTest();
        /// 		}
        /// 		
        /// 		...
        /// 		
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile, TestGroupPhase testGroupPhase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            LoadXmlFromFileAndInit(configFile, testGroupPhase, ctx);
        }

        /// <summary>
        ///     BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">
        ///     The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This
        ///     constructor is used during the initialization or termination of a group of test cases, for example when using the
        ///     NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].
        /// </param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        /// 		[TestFixtureSetUp]
        /// 		public void Test_Group_Setup()
        /// 		{
        ///          // The test case is an embeded resource...
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup);
        /// 			bizUnit.RunTest();
        /// 		}
        /// 		
        /// 		...
        /// 		
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream, TestGroupPhase testGroupPhase)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");

            LoadXmlFromStreamAndInit(configStream, testGroupPhase, null);
        }

        /// <summary>
        ///     BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">
        ///     The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This
        ///     constructor is used during the initialization or termination of a group of test cases, for example when using the
        ///     NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].
        /// </param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit.
        ///     Note: the BizUnit _context object may be created and passed into
        ///     BizUnit, any _context properties set on the _context object may be
        ///     used by BizUnit steps. Context properties may be of any type, i.e. any
        ///     .Net type, of course the consumer of that _context object will need to know
        ///     what type to expect.
        ///     Also note that many test steps have the ability to fetch their configuration
        ///     from the BizUnit _context if their configuration is decorated with the
        ///     attribute takeFromCtx.
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        ///      AddressBook addressBook = new AddressBook("Redmond");
        ///  
        ///      Context ctx = new Context();
        ///      ctx.Add("CorrelationId", "1110023");
        ///      ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///      ctx.Add("AddressBook", addressBook);
        ///  
        /// 		[TestFixtureSetUp]
        /// 		public void Test_Group_Setup()
        /// 		{
        ///          // The test case is an embeded resource...
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup, ctx);
        /// 			bizUnit.RunTest();
        /// 		}
        /// 		
        /// 		...
        /// 		
        /// 	}		
        /// 	</code>
        /// </remarks>
        [Obsolete(
            "BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream, TestGroupPhase testGroupPhase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            LoadXmlFromStreamAndInit(configStream, testGroupPhase, ctx);
        }

        /// <summary>
        ///     BizUnit constructor, introduced in BizUnit 4.0
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// <remarks>
        ///     From BizUnit 4.0 test case maybe programatically created by creating
        ///     test steps, configuring them and then adding them to a test case or
        ///     by loading Xaml test cases. Test cases developed programatically
        ///     maybe serialised to Xaml using TestCase.SaveToFile(),
        ///     similarly Xaml test cases maybe deserialised into a
        ///     TestCase using TestCase.LoadFromFile().
        ///     The exmaple below illustrates loading and running a Xaml test case:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        /// 	[TestMethod]
        /// 	public class SampleTests
        /// 	{
        /// 		[Test]
        /// 		public void ExecuteXamlTestCase()
        /// 		{
        ///          // Load the Xaml test case...
        ///          var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
        ///          
        ///          // Run the test...
        ///          bu.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        ///     The exmaple below illustrates programtically creating a test case and subsequently running it:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        /// 	[TestMethod]
        /// 	public class SampleTests
        /// 	{
        /// 		[Test]
        /// 		public void ExecuteProgramaticallyCreatedTestCase()
        /// 		{
        ///          int stepDelayDuration = 500;
        ///          var step = new DelayStep();
        ///          step.DelayMilliSeconds = stepDelayDuration;
        /// 
        ///          var sw = new Stopwatch();
        ///          sw.Start();
        /// 
        ///          var tc = new TestCase();
        ///          tc.ExecutionSteps.Add(step);
        ///          
        ///          // If we wanted to serialise the test case:
        ///          // TestCase.SaveToFile(tc, "DelayTestCaseTest.xaml");
        ///  
        ///          var bu = new BizUnit(tc));
        /// 
        ///          sw = new Stopwatch().Start();
        /// 
        ///          // Run the test case...
        ///          bu.RunTest();
        /// 
        ///          var actualDuration = sw.ElapsedMilliseconds;
        ///          Console.WriteLine("Observed delay: {0}", actualDuration);
        ///          Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        public BizUnit(TestCase testCase)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");

            Ctx = new Context(this);
            Logger = Ctx.Logger;
            LoadXamlTestCaseAndInit(testCase, TestGroupPhase.Unknown, Ctx);
        }

        /// <summary>
        ///     BizUnit constructor, introduced in BizUnit 4.0
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// <param name="ctx">The BizUnit test context to be used. If this is not supplied a new contxt will created.</param>
        /// <remarks>
        ///     From BizUnit 4.0 test case maybe programatically created by creating
        ///     test steps, configuring them and then adding them to a test case or
        ///     by loading Xaml test cases. Test cases developed programatically
        ///     maybe serialised to Xaml using TestCase.SaveToFile(),
        ///     similarly Xaml test cases maybe deserialised into a
        ///     TestCase using TestCase.LoadFromFile().
        ///     The exmaple below illustrates loading and running a Xaml test case:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        /// 	[TestMethod]
        /// 	public class SampleTests
        /// 	{
        /// 		[Test]
        /// 		public void ExecuteXamlTestCase()
        /// 		{
        ///          // Load the Xaml test case...
        ///          var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
        ///          
        ///          // Run the test...
        ///          bu.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        ///     The exmaple below illustrates programtically creating a test case and subsequently running it:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        /// 	[TestMethod]
        /// 	public class SampleTests
        /// 	{
        /// 		[Test]
        /// 		public void ExecuteProgramaticallyCreatedTestCase()
        /// 		{
        ///          int stepDelayDuration = 500;
        ///          var step = new DelayStep();
        ///          step.DelayMilliSeconds = stepDelayDuration;
        /// 
        ///          var sw = new Stopwatch();
        ///          sw.Start();
        /// 
        ///          var tc = new TestCase();
        ///          tc.ExecutionSteps.Add(step);
        ///          
        ///          // If we wanted to serialise the test case:
        ///          // TestCase.SaveToFile(tc, "DelayTestCaseTest.xaml");
        ///  
        ///          var bu = new BizUnit(tc));
        /// 
        ///          sw = new Stopwatch().Start();
        /// 
        ///          // Run the test case...
        ///          bu.RunTest();
        /// 
        ///          var actualDuration = sw.ElapsedMilliseconds;
        ///          Console.WriteLine("Observed delay: {0}", actualDuration);
        ///          Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        public BizUnit(TestCase testCase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            Logger = ctx.Logger;
            LoadXamlTestCaseAndInit(testCase, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        ///     Gets the BizUnit _context for the current test.
        /// </summary>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        ///      Context ctx;
        ///  
        /// 		[TestFixtureSetUp]
        /// 		public void Test_Group_Setup()
        /// 		{
        ///          // The test case is an embeded resource...
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup);
        ///          ctx = bizUnit.Ctx;
        ///  
        /// 			bizUnit.RunTest();
        /// 		}
        /// 		
        /// 		...
        /// 		
        /// 	}		
        /// 	</code>
        /// </remarks>
        public Context Ctx { get; private set; }

        /// <summary>
        /// </summary>
        public event EventHandler<TestStepEventArgs> TestStepStartEvent;

        /// <summary>
        /// </summary>
        public event EventHandler<TestStepEventArgs> TestStepStopEvent;

        private void LoadXamlTestCaseAndInit(TestCase testCase, TestGroupPhase phase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");
            // ctx - optional

            if (null != ctx)
            {
                Ctx = ctx;
                Ctx.Initialize(this);
            }
            else
            {
                Ctx = new Context(this);
                Logger = Ctx.Logger;
            }

            _xamlTestCase = testCase;
            _testGroupPhase = phase;
            _testName = testCase.Name;
            var now = DateTime.Now;

            // Validate test case...
            testCase.Validate(Ctx);

            if (phase == TestGroupPhase.Unknown)
            {
                Logger.TestStart(_testName, now, GetUserName());
                Ctx.Add(BizUnitTestCaseStartTime, now, true);
            }
            else
            {
                Logger.TestGroupStart(testCase.Name, phase, now, GetUserName());
            }
        }

        private void LoadObjectModelTestCaseAndInit(BizUnitTestCase testCase, TestGroupPhase phase, Context ctx)
        {
            if (null != ctx)
            {
                Ctx = ctx;
                Ctx.Initialize(this);
            }
            else
            {
                Ctx = new Context(this);
                Logger = Ctx.Logger;
            }

            _testGroupPhase = phase;
            _testName = testCase.Name;
            var now = DateTime.Now;

            if (phase == TestGroupPhase.Unknown)
            {
                Logger.TestStart(_testName, now, GetUserName());
                Ctx.Add(BizUnitTestCaseStartTime, now);
            }
            else
            {
                Logger.TestGroupStart(_testName, phase, now, GetUserName());
            }

            _testCaseObjectModel = testCase;
        }

        private void LoadXmlFromFileAndInit(
            string configFile, TestGroupPhase phase, Context ctx)
        {
            var testConfig = new XmlDocument();

            try
            {
                testConfig.Load(configFile);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.ERROR, "Failed to load the step configuration file: \"{0}\", exception: {1}",
                    configFile, ex.ToString());
                throw;
            }

            Init(testConfig, phase, ctx);
        }

        private void LoadXmlFromStreamAndInit(
            Stream configStream, TestGroupPhase phase, Context ctx)
        {
            var testConfig = new XmlDocument();

            try
            {
                var sr = new StreamReader(configStream);
                testConfig.LoadXml(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.ERROR, "Failed to load the step configuration stream, exception: {0}", ex.ToString());
                throw;
            }

            Init(testConfig, phase, ctx);
        }

        private void Init(
            XmlDocument testConfig, TestGroupPhase phase, Context ctx)
        {
            if (null != ctx)
            {
                Ctx = ctx;
                Ctx.Initialize(this);
            }
            else
            {
                Ctx = new Context(this);
                Logger = Ctx.Logger;
            }

            _testGroupPhase = phase;

            // Get test name...
            var nameNode = testConfig.SelectSingleNode("/TestCase/@_testName");
            if (null != nameNode)
            {
                _testName = nameNode.Value;
                Ctx.Add(BizUnitTestCaseName, _testName, true);
            }

            var now = DateTime.Now;

            if (phase == TestGroupPhase.Unknown)
            {
                Logger.TestStart(_testName, now, GetUserName());
                Ctx.Add(BizUnitTestCaseStartTime, now, true);
            }
            else
            {
                Logger.TestGroupStart(_testName, phase, now, GetUserName());
            }

            // Get test setup / execution / teardown steps
            _setupSteps = testConfig.SelectNodes("/TestCase/TestSetup/*");
            _executeSteps = testConfig.SelectNodes("/TestCase/TestExecution/*");
            _teardownSteps = testConfig.SelectNodes("/TestCase/TestCleanup/*");
        }

        /// <summary>
        ///     Executes a test case.
        /// </summary>
        /// <returns>Returns void</returns>
        /// <remarks>
        ///     The following example demonstrates how to create and call BizUnit:
        ///     <code escaped="true">
        /// 	namespace WoodgroveBank.BVTs
        /// 	{
        /// 	using System;
        /// 	using NUnit.Framework;
        /// 	using BizUnit;
        /// 
        ///  // This is an example of calling BizUnit from NUnit...
        /// 	[TestFixture]
        /// 	public class SmokeTests
        /// 	{
        /// 		[Test]
        /// 		public void Test_01_Adapter_MSMQ()
        /// 		{
        /// 			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_01_Adapter_MSMQ.xml");
        /// 			bizUnit.RunTest();
        /// 		}
        /// 	}		
        /// 	</code>
        /// </remarks>
        public void RunTest()
        {
            if (null != _xamlTestCase)
            {
                RunTestInternal(_xamlTestCase);
            }
            else
            {
                RunLegacyTestInternal();
            }
        }

        private void RunLegacyTestInternal()
        {
            try
            {
                Ctx.SetTestName(_testName);

                Setup();
                Execute();
                TearDown();
            }
            finally
            {
                if (null != Logger)
                {
                    Logger.Flush();
                    Logger.Close();
                }
            }

            if (null != _executionException)
            {
                throw _executionException;
            }
        }

        private void RunTestInternal(TestCase xamlTestCase)
        {
            try
            {
                Ctx.SetTestName(xamlTestCase.Name);

                Setup(xamlTestCase.SetupSteps);
                Execute(xamlTestCase.ExecutionSteps);
                TearDown(xamlTestCase.CleanupSteps);
            }
            finally
            {
                if (null != Logger)
                {
                    Logger.Flush();
                    Logger.Close();
                }
            }

            if (null != _executionException)
            {
                throw _executionException;
            }
        }

        private void Setup(IEnumerable<TestStepBase> testSteps)
        {
            ExecuteSteps(testSteps, TestStage.Setup);
        }

        private void Execute(IEnumerable<TestStepBase> testSteps)
        {
            ExecuteSteps(testSteps, TestStage.Execution);
        }

        private void TearDown(IEnumerable<TestStepBase> testSteps)
        {
            ExecuteSteps(testSteps, TestStage.Cleanup);
        }

        private void ExecuteSteps(IEnumerable<TestStepBase> testSteps, TestStage stage)
        {
            Logger.TestStageStart(stage, DateTime.Now);
            Ctx.SetTestStage(stage);

            try
            {
                if (null == testSteps)
                {
                    return;
                }

                foreach (var step in testSteps)
                {
                    ExecuteXamlTestStep(step, stage);
                }

                FlushConcurrentQueue(true, stage);
            }
            catch (Exception e)
            {
                // If we caught an exception on the main test execution, save it, perform cleanup,
                // then throw the exception...
                _executionException = e;
            }

            Logger.TestStageEnd(stage, DateTime.Now, _executionException);
        }

        private void ExecuteXamlTestStep(TestStepBase testStep, TestStage stage)
        {
            try
            {
                // Should this step be executed concurrently?
                if (testStep.RunConcurrently)
                {
                    Ctx.LogInfo("Queuing concurrent step: {0} for execution", testStep.GetType().ToString());
                    Interlocked.Increment(ref _inflightQueueDepth);
                    ThreadPool.QueueUserWorkItem(WorkerThreadThunk, new ConcurrentTestStepWrapper(testStep, Ctx));
                }
                else
                {
                    Logger.TestStepStart(testStep.GetType().ToString(), DateTime.Now, false, testStep.FailOnError);
                    var step = testStep as ImportTestCaseStep;
                    if (step != null)
                    {
                        ExecuteImportedTestCase(step, Ctx);
                    }
                    else
                    {
                        testStep.Execute(Ctx);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.TestStepEnd(testStep.GetType().ToString(), DateTime.Now, e);

                if (testStep.FailOnError)
                {
                    if (e is ValidationStepExecutionException)
                    {
                        throw;
                    }

                    var tsee = new TestStepExecutionException("BizUnit encountered an error executing a test step", e,
                        stage, _testName, testStep.GetType().ToString());
                    throw tsee;
                }
            }

            if (!testStep.RunConcurrently)
            {
                Logger.TestStepEnd(testStep.GetType().ToString(), DateTime.Now, null);
            }

            FlushConcurrentQueue(false, stage);
        }

        private static void ExecuteImportedTestCase(ImportTestCaseStep testStep, Context context)
        {
            var testCase = testStep.GetTestCase();
            var bu = new BizUnit(testCase, context);
            bu.RunTest();
        }

        private void Setup()
        {
            try
            {
                Logger.TestStageStart(TestStage.Setup, DateTime.Now);
                Ctx.SetTestStage(TestStage.Setup);

                if (null != _testCaseObjectModel)
                {
                    ExecuteSteps(_testCaseObjectModel.SetupSteps, TestStage.Setup);
                }
                else
                {
                    ExecuteSteps(_setupSteps, TestStage.Setup);
                }
            }
            catch (Exception e)
            {
                Logger.TestStageEnd(TestStage.Setup, DateTime.Now, e);
                throw;
            }

            Logger.TestStageEnd(TestStage.Setup, DateTime.Now, null);
        }

        private void Execute()
        {
            Logger.TestStageStart(TestStage.Execution, DateTime.Now);
            Ctx.SetTestStage(TestStage.Execution);

            try
            {
                if (null != _testCaseObjectModel)
                {
                    ExecuteSteps(_testCaseObjectModel.ExecutionSteps, TestStage.Execution);
                }
                else
                {
                    ExecuteSteps(_executeSteps, TestStage.Execution);
                }
            }
            catch (Exception e)
            {
                // If we caught an exception on the main test execution, save it, perform cleanup,
                // then throw the exception...
                _executionException = e;
            }

            Logger.TestStageEnd(TestStage.Execution, DateTime.Now, _executionException);
        }

        private void TearDown()
        {
            Logger.TestStageStart(TestStage.Cleanup, DateTime.Now);
            Ctx.SetTestStage(TestStage.Cleanup);

            try
            {
                if (null != _testCaseObjectModel)
                {
                    ExecuteSteps(_testCaseObjectModel.CleanupSteps, TestStage.Cleanup);
                }
                else
                {
                    ExecuteSteps(_teardownSteps, TestStage.Cleanup);
                }

                Ctx.Teardown();
            }
            catch (Exception e)
            {
                Logger.TestStageEnd(TestStage.Cleanup, DateTime.Now, e);

                Logger.TestEnd(_testName, DateTime.Now, e);

                if (null != _executionException)
                {
                    throw _executionException;
                }
                throw;
            }

            Logger.TestStageEnd(TestStage.Cleanup, DateTime.Now, null);

            if (TestGroupPhase.Unknown != _testGroupPhase)
            {
                Logger.TestGroupEnd(_testGroupPhase, DateTime.Now, _executionException);
            }
            else
            {
                Logger.TestEnd(_testName, DateTime.Now, _executionException);
            }
        }

        private void ExecuteSteps(IEnumerable<BizUnitTestStepWrapper> steps, TestStage stage)
        {
            if (null == steps)
            {
                return;
            }

            foreach (var step in steps)
            {
                ExecuteTestStep(step, stage);
            }

            FlushConcurrentQueue(true, stage);
        }

        private void ExecuteSteps(XmlNodeList steps, TestStage stage)
        {
            if (null == steps)
            {
                return;
            }

            foreach (XmlNode stepConfig in steps)
            {
                var stepWrapper = new BizUnitTestStepWrapper(stepConfig);
                ExecuteTestStep(stepWrapper, stage);
            }

            FlushConcurrentQueue(true, stage);
        }

        private void ExecuteTestStep(BizUnitTestStepWrapper stepWrapper, TestStage stage)
        {
            try
            {
                // Should this step be executed concurrently?
                if (stepWrapper.RunConcurrently)
                {
                    Logger.TestStepStart(stepWrapper.TypeName, DateTime.Now, true, stepWrapper.FailOnError);
                    Interlocked.Increment(ref _inflightQueueDepth);
                    ThreadPool.QueueUserWorkItem(WorkerThreadThunk, new ConcurrentTestStepWrapper(stepWrapper, Ctx));
                }
                else
                {
                    Logger.TestStepStart(stepWrapper.TypeName, DateTime.Now, false, stepWrapper.FailOnError);
                    stepWrapper.Execute(Ctx);
                }
            }
            catch (Exception e)
            {
                Logger.TestStepEnd(stepWrapper.TypeName, DateTime.Now, e);

                if (stepWrapper.FailOnError)
                {
                    if (e is ValidationStepExecutionException)
                    {
                        throw;
                    }
                    var tsee = new TestStepExecutionException("BizUnit encountered an error executing a test step", e,
                        stage, _testName, stepWrapper.TypeName);
                    throw tsee;
                }
            }

            if (!stepWrapper.RunConcurrently)
            {
                Logger.TestStepEnd(stepWrapper.TypeName, DateTime.Now, null);
            }

            FlushConcurrentQueue(false, stage);
        }

        private void FlushConcurrentQueue(bool waitingToFinish, TestStage stage)
        {
            if (waitingToFinish && _inflightQueueDepth == 0)
            {
                return;
            }

            while ((_completedConcurrentSteps.Count > 0) || waitingToFinish)
            {
                ConcurrentTestStepWrapper step;
                _completedConcurrentSteps.TryDequeue(out step);

                if (null != step)
                {
                    Logger.LogBufferedText(step.Logger);

                    Logger.TestStepEnd(step.Name, DateTime.Now, step.FailureException);

                    // Check to see if the test step failed, if it did throw the exception...
                    if (null != step.FailureException)
                    {
                        Interlocked.Decrement(ref _inflightQueueDepth);

                        if (step.FailOnError)
                        {
                            if (step.FailureException is ValidationStepExecutionException)
                            {
                                throw step.FailureException;
                            }
                            var tsee =
                                new TestStepExecutionException(
                                    "BizUnit encountered an error concurrently executing a test step",
                                    step.FailureException, stage, _testName, step.StepName);
                            throw tsee;
                        }
                    }
                    else
                    {
                        Interlocked.Decrement(ref _inflightQueueDepth);
                    }
                }

                if (waitingToFinish && (_inflightQueueDepth > 0))
                {
                    Thread.Sleep(250);
                }
                else if (waitingToFinish && (_inflightQueueDepth == 0))
                {
                    break;
                }
            }
        }

        private void WorkerThreadThunk(object stateInfo)
        {
            var step = (ConcurrentTestStepWrapper) stateInfo;
            step.Execute();

            // This step is completed, add to queue
            _completedConcurrentSteps.Enqueue(step);
        }

        private static IValidationStep CreateValidatorStep(string typeName, string assemblyPath)
        {
            return (IValidationStep) ObjectCreator.CreateStep(typeName, assemblyPath);
        }

        private static IContextLoaderStep CreateContextLoaderStep(string typeName, string assemblyPath)
        {
            return (IContextLoaderStep) ObjectCreator.CreateStep(typeName, assemblyPath);
        }

        internal void ExecuteValidator(Stream data, IValidationStepOM validationStep, Context ctx)
        {
            if (null == validationStep)
            {
                return;
            }

            Logger.ValidatorStart(validationStep.GetType().ToString(), DateTime.Now);

            try
            {
                validationStep.ExecuteValidation(data, ctx);
            }
            catch (Exception ex)
            {
                Logger.ValidatorEnd(validationStep.GetType().ToString(), DateTime.Now, ex);

                var vsee =
                    new ValidationStepExecutionException("BizUnit encountered an error executing a validation step", ex,
                        _testName);
                throw vsee;
            }

            Logger.ValidatorEnd(validationStep.GetType().ToString(), DateTime.Now, null);
        }

        internal void ExecuteValidator(Stream data, XmlNode validatorConfig, Context ctx)
        {
            if (null == validatorConfig)
            {
                return;
            }

            var assemblyPath = validatorConfig.SelectSingleNode("@assemblyPath");
            var typeName = validatorConfig.SelectSingleNode("@typeName");

            Logger.ValidatorStart(typeName.Value, DateTime.Now);

            try
            {
                var v = CreateValidatorStep(typeName.Value, assemblyPath.Value);
                v.ExecuteValidation(data, validatorConfig, ctx);
            }
            catch (Exception ex)
            {
                Logger.ValidatorEnd(typeName.Value, DateTime.Now, ex);
                var vsee =
                    new ValidationStepExecutionException("BizUnit encountered an error executing a validation step", ex,
                        _testName);
                throw vsee;
            }

            Logger.ValidatorEnd(typeName.Value, DateTime.Now, null);
        }

        internal void ExecuteContextLoader(Stream data, IContextLoaderStepOM contextLoaderStep, Context ctx)
        {
            if (null == contextLoaderStep)
            {
                return;
            }

            Logger.ContextLoaderStart(contextLoaderStep.GetType().ToString(), DateTime.Now);

            try
            {
                contextLoaderStep.ExecuteContextLoader(data, ctx);
            }
            catch (Exception ex)
            {
                Logger.ContextLoaderEnd(contextLoaderStep.GetType().ToString(), DateTime.Now, ex);
                throw;
            }

            Logger.ContextLoaderEnd(contextLoaderStep.GetType().ToString(), DateTime.Now, null);
        }

        internal void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context ctx)
        {
            if (null == contextConfig)
            {
                return;
            }

            var assemblyPath = contextConfig.SelectSingleNode("@assemblyPath");
            var typeName = contextConfig.SelectSingleNode("@typeName");

            Logger.ContextLoaderStart(typeName.Value, DateTime.Now);

            try
            {
                var cd = CreateContextLoaderStep(typeName.Value, assemblyPath.Value);
                cd.ExecuteContextLoader(data, contextConfig, ctx);
            }
            catch (Exception ex)
            {
                Logger.ContextLoaderEnd(typeName.Value, DateTime.Now, ex);
                throw;
            }

            Logger.ContextLoaderEnd(typeName.Value, DateTime.Now, null);
        }

        internal static string GetNow()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff dd/MM/yyyy");
        }

        internal static string GetUserName()
        {
            var usersDomain = Environment.UserDomainName;
            var usersName = Environment.UserName;

            return usersDomain + "\\" + usersName;
        }

        internal void OnTestStepStart(TestStepEventArgs e)
        {
            if (null != TestStepStartEvent)
            {
                var testStepStartEvent = TestStepStartEvent;
                testStepStartEvent(this, e);
            }
        }

        internal void OnTestStepStop(TestStepEventArgs e)
        {
            if (null != TestStepStopEvent)
            {
                var testStepStopEvent = TestStepStopEvent;
                testStepStopEvent(this, e);
            }
        }
    }
}