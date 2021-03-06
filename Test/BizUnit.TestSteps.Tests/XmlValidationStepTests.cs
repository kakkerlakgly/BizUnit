﻿using System;
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.ValidationSteps.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.TestSteps.Tests
{
    /// <summary>
    ///     Summary description for XmlValidationStepTests
    /// </summary>
    [TestClass]
    public class XmlValidationStepTests
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void XmlValidationStepTest()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition
            {
                Description = "PONumber",
                XPath =
                    "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']",
                Value = "12323"
            };
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data =
                StreamHelper.LoadFileToStream(@"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml");
            validation.Execute(data, ctx);
        }

        [ExpectedException(typeof (InvalidOperationException))]
        [TestMethod]
        public void XmlValidationStepTest_InvalidXPath()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition
            {
                Description = "PONumber",
                XPath =
                    "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']",
                Value = "12323"
            };
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data =
                StreamHelper.LoadFileToStream(
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder002_BadXPath.xml");
            validation.Execute(data, ctx);
        }

        [ExpectedException(typeof (ValidationStepExecutionException))]
        [TestMethod]
        public void XmlValidationStepTest_SchemaValidationFail()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition
            {
                Description = "PONumber",
                XPath =
                    "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']",
                Value = "12323"
            };
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data =
                StreamHelper.LoadFileToStream(
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder003_SchemaValidationFail.xml");
            validation.Execute(data, ctx);
        }

        [ExpectedException(typeof (ValidationStepExecutionException))]
        [TestMethod]
        public void XmlValidationStepTest_SchemaValidationFailMissingElem()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition
            {
                Description = "PONumber",
                XPath =
                    "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']",
                Value = "12323"
            };
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data =
                StreamHelper.LoadFileToStream(
                    @"..\..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder004_SchemaValidationFailMissingElem.xml");
            validation.Execute(data, ctx);
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
    }
}