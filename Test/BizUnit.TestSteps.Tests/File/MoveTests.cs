﻿
using BizUnit.TestSteps.File;

namespace BizUnit.TestSteps.Tests.File
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System;

    /// <summary>
    /// Summary description for FileCreateTests
    /// </summary>
    [TestClass]
    public class MoveTests
    {
        public MoveTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void MoveFileTest()
        {
            TestHelper.DeleteFile(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel2xml");
            TestHelper.DeleteFile(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xml");

            System.IO.File.Copy(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml",
                @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xml");

            var step = new MoveStep();
            step.SourcePath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xml";
            step.DestinationPath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel2xml";

            step.Execute(new Context());

            Assert.IsTrue(System.IO.File.Exists(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel2xml"));
         
        }

        [TestMethod]
        public void MoveFileTest_Negative()
        {
            TestHelper.DeleteFile(@"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel2xml");
            TestHelper.DeleteFile(@"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xml");

            System.IO.File.Copy(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml",
                @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xml");

            var step = new MoveStep();
            step.SourcePath = string.Empty;
            step.DestinationPath = @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel2xml";

            try
            {
                step.Validate(new Context());

            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("SourcePath is either null or of zero length"));
            }


            step.SourcePath = @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xml";
            step.DestinationPath = string.Empty;

            try
            {
                step.Validate(new Context());

            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("DestinationPath is either null or of zero length"));
            }


            step.SourcePath = @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xmlerror";
            step.DestinationPath = @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.testdel1xmlerror";

            try
            {
                var context = new Context();
                step.Validate(context);
                step.Execute(context);

            }
            catch (FileNotFoundException fex)
            {
                Assert.IsTrue(fex.Message.Contains("Could not find file"));
            }
        }
    }
}
