//---------------------------------------------------------------------
// File: BAMDeploymentStep.cs
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
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;

namespace BizUnit.CoreSteps.TestSteps
{
    /// <summary>
    ///     ExportDBDataToDataSetStep exports data contained in a list of tables to a Xml file. The export is carried out using
    ///     the WriteXml capability of the DataSet.
    /// </summary>
    /// <remarks>
    ///     <code escaped="true">
    ///  <TestStep assemblyPath="" typeName="BizUnit.ExportDBTableDataToDatasetStep">
    ///             <DelayBeforeExecution>0</DelayBeforeExecution>
    ///             <!-- Optional, seconds to delay for this step to complete -->
    ///             <ConnectionString>
    ///                 Persist Security Info=False;Integrated
    ///                 Security=SSPI;database=DBname;server=(local);Connect Timeout=30
    ///             </ConnectionString>
    ///             <DatasetWriteXmlSchemaPath>C:\DBDtableSchema.xml</DatasetWriteXmlSchemaPath>
    ///             <DatasetWriteXmlPath>C:\DBDtable.xml</DatasetWriteXmlPath>
    ///             <TableNames>Orders,OrderDetails</TableNames>
    ///         </TestStep>
    ///  </code>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Tag</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>DelayBeforeExecution</term>
    ///             <description>The number of seconds to wait before executing the step</description>
    ///         </item>
    ///         <item>
    ///             <term>ConnectionString</term>
    ///             <description>The connection string specifying the database from which a DataSet will be exported</description>
    ///         </item>
    ///         <item>
    ///             <term>DatasetWriteXmlSchemaPath</term>
    ///             <description>File location where the Xml schema represenation of the DataSet should be saved</description>
    ///         </item>
    ///         <item>
    ///             <term>DatasetWriteXmlPath</term>
    ///             <description>File location where the Xml representation of the DataSet data should be saved</description>
    ///         </item>
    ///         <item>
    ///             <term>TableNames</term>
    ///             <description>Comma-separated list of tablenames that should be exported in the DataSet</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Obsolete("ExportDBDataToDataSetStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class ExportDBDataToDataSetStep : ITestStep
    {
        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            var delayBeforeCheck = context.ReadConfigAsInt32(testConfig, "DelayBeforeExecution");
            var connectionString = context.ReadConfigAsString(testConfig, "ConnectionString");

            var datasetWriteXmlSchemaPath = context.ReadConfigAsString(testConfig, "DatasetWriteXmlSchemaPath");
            var datasetWriteXmlPath = context.ReadConfigAsString(testConfig, "DatasetWriteXmlPath");
            var tableNames = context.ReadConfigAsString(testConfig, "TableNames");

            // Sleep for delay seconds...
            Thread.Sleep(delayBeforeCheck);

            var ds = GetDataSet(connectionString, tableNames);

            ds.WriteXml(datasetWriteXmlPath);
            ds.WriteXmlSchema(datasetWriteXmlSchemaPath);
        }

        private static DataSet GetDataSet(string connectionString, string tableNames)
        {
            DataSet ds = null;

            try
            {
                ds = new DataSet("DataStore");
                var arrtableName = tableNames.Split(',');
                using (var connection = new SqlConnection(connectionString))
                {
                    foreach (var tableName in arrtableName)
                    {
                        using (var comm = new SqlCommand("SELECT * FROM " + tableName, connection))
                        {
                            comm.CommandType = CommandType.Text;
                            using (var da = new SqlDataAdapter(comm))
                            {
                                da.Fill(ds, tableName);
                                da.FillSchema(ds, SchemaType.Source);
                            }
                        }
                    }
                }
                return ds;
            }
            catch
            {
                if (ds != null) ds.Dispose();
                throw;
            }
        }
    }
}