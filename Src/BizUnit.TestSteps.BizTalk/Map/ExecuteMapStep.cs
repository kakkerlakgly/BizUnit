//---------------------------------------------------------------------
// File: ExecuteMapStep.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Map
{
    /// <summary>
    ///     The ExecuteMapStep can be used to execute a map and test the output from it
    /// </summary>
    /// <remarks>
    ///     The following shows an example of the Xml representation of this test step.
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath=""
    ///             typeName="BizUnit.BizTalkSteps.ExecuteMapStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///             <Map assemblyPath="BizUnit.BizTalkTestArtifacts"
    ///                 typeName="MapSchema1ToSchema2" />
    ///             <Source>.\TestData\Schema1.xml</Source>
    ///             <Destination>.\TestData\Schema2.actual.xml</Destination>
    ///             <!-- Note: ContextLoader Step could be any generic validation step -->
    ///             <ContextLoaderStep assemblyPath="" typeName="BizUnit.RegExContextLoader">
    ///                 <RegEx contextKey="HTTP_Url">/def:html/def:body/def:p[2]/def:form</RegEx>
    ///                 <RegEx contextKey="ActionID">/def:html/def:body/def:p[2]/def:form/def:input[3]</RegEx>
    ///                 <RegEx contextKey="ActionType">/def:html/def:body/def:p[2]/def:form/def:input[4]</RegEx>
    ///                 <RegEx contextKey="HoldEvent">/def:html/def:body/def:p[2]/def:form/def:input[2]</RegEx>
    ///             </ContextLoaderStep>
    ///             <!-- Note: Validation step could be any generic validation step -->
    ///             <ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///                 <XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
    ///                 <XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
    ///                 <XPathList>
    ///                     <XPathValidation
    ///                         query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">
    ///                         PONumber_0
    ///                     </XPathValidation>
    ///                 </XPathList>
    ///             </ValidationStep>
    ///         </TestStep>
    /// 	</code>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Map:assemblyPath</term>
    ///             <description>The assembly containing the BizTalk map to execute.</description>
    ///         </listheader>
    ///         <item>
    ///             <term>Map:typeName</term>
    ///             <description>The typename of the BizTalk map to execute</description>
    ///         </item>
    ///         <item>
    ///             <term>Source</term>
    ///             <description>The relative file location of the input document to be mapped</description>
    ///         </item>
    ///         <item>
    ///             <term>Destination</term>
    ///             <description>The relative file location of the ouput document that has been mapped</description>
    ///         </item>
    ///         <item>
    ///             <term>ContextLoaderStep</term>
    ///             <description>
    ///                 The configuration for the context loader step used to load data into the BizUnit context which
    ///                 may be used by subsequent test steps
    ///                 <para>(optional)</para>
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>ValidationStep</term>
    ///             <description>
    ///                 The configuration for the validation step used to validate the contents of the file, the
    ///                 validation step should implement IValidationTestStep
    ///                 <para>(optional)</para>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class ExecuteMapStep : TestStepBase
    {
        /// <summary>
        /// </summary>
        public ExecuteMapStep()
        {
            SubSteps = new List<SubStepBase>();
        }

        /// <summary>
        ///     Gets and sets the assembly path for the .NET type of the map to be executed
        /// </summary>
        public string MapAssemblyPath { get; set; }

        /// <summary>
        ///     Gets and sets the type name for the .NET type of the map to be executed
        /// </summary>
        public string MapTypeName { get; set; }

        /// <summary>
        ///     Gets and sets the relative file location of the input document to be mapped
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Gets and sets the relative file location of the ouput document that has been mapped
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            var mapType = ObjectCreator.GetType(MapTypeName, MapAssemblyPath);

            var destDir = Path.GetDirectoryName(Destination);
            if ((destDir.Length > 0) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            var bmt = new BizTalkMapTester(mapType);
            bmt.Execute(Source, Destination);

            using (var fs = new FileStream(Destination, FileMode.Open, FileAccess.Read))
            {
                using (var data = StreamHelper.LoadMemoryStream(fs))
                {
                    data.Seek(0, SeekOrigin.Begin);
                    SubSteps.Aggregate(data, (current, subStep) => subStep.Execute(current, context));
                }
            }

        }

        /// <summary>
        ///     ITestStepOM.Validate() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Validate(Context context)
        {
            Source = context.SubstituteWildCards(Source);
            if (!System.IO.File.Exists(Source))
            {
                throw new ArgumentException("Source file does not exist.", Source);
            }

            ArgumentValidation.CheckForEmptyString(MapTypeName, "MapTypeName");

            Destination = context.SubstituteWildCards(Destination);

            foreach (var step in SubSteps)
            {
                step.Validate(context);
            }
        }
    }
}