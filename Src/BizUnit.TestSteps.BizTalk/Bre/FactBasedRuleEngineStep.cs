//---------------------------------------------------------------------
// File: FactBasedRuleEngineStep.cs
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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;
using Microsoft.RuleEngine;

namespace BizUnit.TestSteps.BizTalk.Bre
{
    /// <summary>
    ///     Summary description for FactBasedRuleEngineStep.
    /// </summary>
    /// <remarks>
    ///     The following shows an example of the Xml representation of this test step.
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath=""
    ///             typeName="BizUnit.BizTalkSteps.FactBasedRuleEngineStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///             <RuleStoreName>
    ///                 C:\Program Files\Microsoft BizTalk Server
    ///                 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\LoanProcessing.xml
    ///             </RuleStoreName>
    ///             <RuleSetInfoCollectionName>LoanProcessing</RuleSetInfoCollectionName>
    ///             <DebugTracking>
    ///                 C:\Program Files\Microsoft BizTalk Server
    ///                 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\outputtraceforLoanPocessing.txt
    ///             </DebugTracking>
    ///             <ResultFilePath>
    ///                 C:\Program Files\Microsoft BizTalk Server
    ///                 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases
    ///             </ResultFilePath>
    ///             <Facts>
    ///                 <Fact type="document" schemaType="LoanProcessing"
    ///                     instanceDocument="C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\SampleLoan.xml" />
    ///                 <Fact type="object"
    ///                     assemblyPath="C:\Program Files\Microsoft Biztalk Server\SDK\Samples\Business Rules\Business Rules Hello World1\MySampleLibrary\bin\Debug\MySampleLibrary.dll"
    ///                     typeName="Microsoft.Samples.BizTalk.BusinessRulesHelloWorld1.MySampleLibrary.MySampleBusinessObject" />
    ///                 <Fact type="dataConnection" connectionString="" dataset="" tableName="" />
    ///                 <Fact type="dataTable" connectionString="" command="" dataset="" tableName="" />
    ///                 <Fact type="dataRow" connectionString="" command="" dataset="" tableName="" />
    ///             </Facts>
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
    ///             <term>Tag</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>RuleStoreName</term>
    ///             <description>The location of the rule store</description>
    ///         </item>
    ///         <item>
    ///             <term>RuleSetInfoCollectionName</term>
    ///             <description>The name of the rule set List</description>
    ///         </item>
    ///         <item>
    ///             <term>DebugTracking</term>
    ///             <description>Location of the debug tracking</description>
    ///         </item>
    ///         <item>
    ///             <term>ResultFilePath</term>
    ///             <description>The path used to write updated fact documents to</description>
    ///         </item>
    ///         <item>
    ///             <term>Facts</term>
    ///             <description>Facts to pass to rules engine prior to ruleset execution</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class FactBasedRuleEngineStep : TestStepBase
    {
        /// <summary>
        /// </summary>
        public FactBasedRuleEngineStep()
        {
            FactsList = new List<Fact>();
            SubSteps = new List<SubStepBase>();
        }

        /// <summary>
        /// </summary>
        public string RuleStoreName { get; set; }

        /// <summary>
        /// </summary>
        public string RuleSetInfoCollectionName { get; set; }

        /// <summary>
        /// </summary>
        public string DebugTracking { get; set; }

        /// <summary>
        /// </summary>
        public IList<Fact> FactsList { get; set; }

        /// <summary>
        ///     TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            var fi = new FileInfo(RuleStoreName);

            if (!fi.Exists)
            {
                throw new FileNotFoundException("RuleStoreName", RuleStoreName);
            }

            var ruleStore = new FileRuleStore(fi.FullName);
            var rsInfo = ruleStore.GetRuleSets(RuleSetInfoCollectionName, RuleStore.Filter.Latest);
            if (rsInfo.Count != 1)
            {
                // oops ... error
                throw new InvalidOperationException(string.Format("RuleStore {0} did not contain RuleSet {1}",
                    RuleStoreName, RuleSetInfoCollectionName));
            }

            var ruleset = ruleStore.GetRuleSet(rsInfo[0]);


            // load the facts into array
            var facts = new List<object>(FactsList.Count);

            // Create an instance of the Policy Tester class
            using (var policyTester = new PolicyTester(ruleset))
            {
                foreach (var currentFact in FactsList)
                {
                    switch (currentFact.GetType().ToString())
                    {
                        case "ObjectFact":
                        {
                            var fact = currentFact as ObjectFact;

                            object[] objectArgs = null;
                            if (null != fact.Args)
                            {
                                objectArgs = fact.Args.Split(',').Cast<object>().ToArray();
                            }

                            Type type;
                            if (fact.AssemblyPath.Length > 0)
                            {
                                var asm = Assembly.Load(fact.AssemblyPath);
                                if (asm == null)
                                {
                                    // fail
                                    throw (new InvalidOperationException("failed to create type " + fact.Type));
                                }
                                type = asm.GetType(fact.Type, true, false);
                            }
                            else
                            {
                                // must be in path
                                type = Type.GetType(fact.Type);
                            }

                            facts.Add(Activator.CreateInstance(type, objectArgs));
                            break;
                        }
                        case "DocumentFact":
                        {
                            var fact = currentFact as DocumentFact;
                            var xd1 = new XmlDocument();
                            xd1.Load(fact.InstanceDocument);
                            var txd = new TypedXmlDocument(fact.SchemaType, xd1);
                            facts.Add(txd);
                            break;
                        }
                        case "DataConnectionFact":
                        {
                            var fact = currentFact as DataConnectionFact;
                            var conn = new SqlConnection(fact.ConnectionString);
                            conn.Open();
                            var dc = new DataConnection(fact.Dataset, fact.TableName, conn);
                            facts.Add(dc);
                            break;
                        }
                        case "dataTable":
                        case "dataRow":
                        {
                            var fact = currentFact as DataTableFact;

                            var conn = new SqlConnection(fact.ConnectionString);
                            conn.Open();
                            var myCommand = new SqlCommand(fact.Command, conn) {CommandType = CommandType.Text};
                            var dAdapt = new SqlDataAdapter();
                            dAdapt.TableMappings.Add("Table", fact.TableName);
                            dAdapt.SelectCommand = myCommand;
                            var ds = new DataSet(fact.Dataset);
                            dAdapt.Fill(ds);
                            var tdt = new TypedDataTable(ds.Tables[fact.TableName]);
                            if (fact.Type == "dataRow")
                            {
                                var tdr = new TypedDataRow(ds.Tables[fact.TableName].Rows[0], tdt);
                                facts.Add(tdr);
                            }
                            else
                            {
                                facts.Add(tdt);
                            }
                            break;
                        }
                    }
                }

                // Create an instance of the DebugTrackingInterceptor
                using (var dti = new DebugTrackingInterceptor(DebugTracking))
                {
                    // Execute Policy Tester
                    try
                    {
                        policyTester.Execute(facts.ToArray(), dti);
                    }
                    catch (Exception e)
                    {
                        context.LogException(e);
                        throw;
                    }
                }
            }

            // write out all document instances passed in
            foreach (var fact in facts)
            {
                switch (fact.GetType().Name)
                {
                    case "TypedXmlDocument":
                    {
                        var txd = (TypedXmlDocument) fact;

                        context.LogData("TypedXmlDocument result: ", txd.Document.OuterXml);
                        using (var data = StreamHelper.LoadMemoryStream(txd.Document.OuterXml))
                        {
                            if (txd.DocumentType == "UBS.CLAS.PoC.Schemas.INSERTS")
                            {
                                SubSteps.Aggregate(data, (current, subStep) => subStep.Execute(current, context));
                            }
                        }

                        break;
                    }
                    case "DataConnection":
                    {
                        var dc = (DataConnection) fact;
                        dc.Update(); // persist any changes
                        break;
                    }
                    case "TypedDataTable":
                    {
                        var tdt = (TypedDataTable) fact;
                        tdt.DataTable.AcceptChanges();
                        break;
                    }
                    case "TypedDataRow":
                    {
                        var tdr = (TypedDataRow) fact;
                        tdr.DataRow.AcceptChanges();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public override void Validate(Context context)
        {
        }
    }
}