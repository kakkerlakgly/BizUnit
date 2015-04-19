
using BizUnit.TestSteps.BizTalk.Map;
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.BizTalkSteps.Tests
{
    [TestClass]
    public class ExecuteMapStepTests
    {
        [TestMethod]
        public void MapDocumentInstanceTest()
        {
            var mapStep = new ExecuteMapStep
            {
                MapAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.xml",
                MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2",
                Destination = "Schema2.001.xml"
            };

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase {Name = "MapDocumentInstanceTest"};
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTest.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);
        }

        [TestMethod]
        public void MapDocumentInstanceTestAndValidate()
        {
            var mapStep = new ExecuteMapStep
            {
                MapAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.xml",
                MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2",
                Destination = "Schema2.002.xml"
            };

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd",
                XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2"
            };
            validation.XmlSchemas.Add(sd);
            var xpd = new XPathDefinition
            {
                XPath =
                    "/*[local-name()='Schema2Root' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema2']/*[local-name()='Child1' and namespace-uri()='']/@*[local-name()='Child1Attribute1' and namespace-uri()='']",
                Value = "1"
            };
            validation.XPathValidations.Add(xpd);

            // Add validation...
            mapStep.SubSteps.Add(validation);

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase {Name = "MapDocumentInstanceTest"};
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTestAndValidate.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);
        }

        [TestMethod]
        public void MapDocumentInstanceTestAndValidateAndAddValueToCtx()
        {
            var mapStep = new ExecuteMapStep
            {
                MapAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.xml",
                MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2",
                Destination = "Schema2.003.xml"
            };

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd",
                XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2"
            };
            validation.XmlSchemas.Add(sd);
            var xpd = new XPathDefinition
            {
                XPath =
                    "/*[local-name()='Schema2Root' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema2']/*[local-name()='Child1' and namespace-uri()='']/@*[local-name()='Child1Attribute1' and namespace-uri()='']",
                Value = "1",
                ContextKey = "Child1.Child1Attribute1"
            };
            validation.XPathValidations.Add(xpd);

            // Add validation...
            mapStep.SubSteps.Add(validation);

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase {Name = "MapDocumentInstanceTest"};
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTestAndValidateAndAddValueToCtx.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);

            Assert.AreEqual("1", ctx.GetValue("Child1.Child1Attribute1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationStepExecutionException))]
        public void MapDocumentInstanceTestAndValidateInvalidDocument()
        {
            var mapStep = new ExecuteMapStep
            {
                MapAssemblyPath =
                    @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                Source = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.error.xml",
                MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2",
                Destination = "Schema2.005.xml"
            };

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition
            {
                XmlSchemaPath = @"..\..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd",
                XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2"
            };
            validation.XmlSchemas.Add(sd);
            var xpd = new XPathDefinition
            {
                XPath =
                    "/*[local-name()='Schema2Root' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema2']/*[local-name()='Child1' and namespace-uri()='']/@*[local-name()='Child1Attribute1' and namespace-uri()='']",
                Value = "1"
            };
            validation.XPathValidations.Add(xpd);

            // Add validation...
            mapStep.SubSteps.Add(validation);

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase {Name = "MapDocumentInstanceTest"};
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTestAndValidateInvalidDocument.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);
        }
    }
}