//---------------------------------------------------------------------
// File: DBQueryStep.cs
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
using System.Linq;
using System.Threading;
using System.Xml;
using BizUnit.BizUnitOM;
using BizUnit.Common;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The DBQueryStep is used to query a SQL query against a database and validate the values for the row returned.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.DBQueryStep">
	///     <NumberOfRowsExpected>1</NumberOfRowsExpected>
	///		<DelayBeforeCheck>1</DelayBeforeCheck>
	///		<ConnectionString>Persist Security Info=False;Integrated Security=SSPI;database=BAMPrimaryImport;server=(local);Connect Timeout=30</ConnectionString>
	/// 
	///		<!-- 
	///		The SQL Query to execute is built by formatting the RawSQLQuery substituting in the 
	///		SQLQueryParam's
	///		-->
	///		<SQLQuery>
	///			<RawSQLQuery>select * from dbo.bam_Event_Completed where EventId = {0}</RawSQLQuery>
	///			<SQLQueryParams>
	///				<SQLQueryParam takeFromCtx="EventId"></SQLQueryParam>
	///			</SQLQueryParams>
	///		</SQLQuery>
	///			
	///		<Rows>
	///			<Columns>
	///			<!-- 
	///			Note: The column names are dependant on the DB schema being checked against.
	///			Adding the attribute isUnique="true" to one of the columns allows it to be 
	///			uniquely selcted in the scenario where multiple rows are returned.
	///			-->
	///					
	///				<EventType>Switch</EventType>
	///				<EventStatus>Completed</EventStatus>
	///				<ProcessorId>JVQFFCCZ0</ProcessorId>
	///				<SchemeId>GF81300000</SchemeId>
	///				<EventFailed>null</EventFailed>
	///				<EventHeld>null</EventHeld>
	///				<EventHoldNotifRec>null</EventHoldNotifRec>
	///			</Columns>
	///		</Rows>	
	///	</TestStep>
	///	</code>
	///	
	///	The ContextManipulator builds a new context item by appeanding the values of multiple context items
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayBeforeCheck</term>
	///			<description>The number of seconds to wait before executing the step</description>
	///		</item>
	///		<item>
	///			<term>ConnectionString</term>
	///			<description>The connection string used for the DB query</description>
	///		</item>
	///		<item>
	///			<term>NumberOfRowsExpected</term>
	///			<description>The expected number of rows (optional)</description>
	///		</item>	
	///		<item>
	///			<term>SQLQuery/RawSQLQuery</term>
	///			<description>The raw SQL string that will be formatted by substituting in the SQLQueryParam</description>
	///		</item>
	///		<item>
	///			<term>SQLQuery/SQLQueryParams/SQLQueryParam</term>
	///			<description>The parameters to substitute into RawSQLQuery <para>(repeating)</para></description>
	///		</item>
	///		<item>
	///			<term>Rows/Columns</term>
	///			<description>One or more columns which represent the expected query result</description>
	///		</item>
	///		<item>
	///			<term>Rows/Columns/User defined element</term>
	///			<description>The fields that are validated in the response</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("DBQueryStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class DBQueryStep : ITestStepOM
	{
	    /// <summary>
	    /// 
	    /// </summary>
	    public DBQueryStep()
	    {
	        DBRowsToValidate = new DBRowsToValidate();
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    public int DelayBeforeExecution { set; get; }

	    /// <summary>
	    /// 
	    /// </summary>
	    public string ConnectionString { set; get; }

	    /// <summary>
	    /// 
	    /// </summary>
	    [BizUnitParameterFormatter("BizUnit.SqlQueryParamFormatter")]
        public SqlQuery SQLQuery { set; get; }

	    /// <summary>
	    /// 
	    /// </summary>
	    [BizUnitParameterFormatter("BizUnit.DBRowsToValidateParamFormatter")]
        public DBRowsToValidate DBRowsToValidate { set; get; }

	    /// <summary>
	    /// 
	    /// </summary>
	    public int NumberOfRowsExpected { set; get; }

	    /// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			DelayBeforeExecution = context.ReadConfigAsInt32( testConfig, "DelayBeforeCheck" );			
			ConnectionString = context.ReadConfigAsString( testConfig, "ConnectionString" );
			var queryConfig = testConfig.SelectSingleNode( "SQLQuery" );
            SQLQuery = SqlQuery.BuildSQLQuery(queryConfig, context);
            NumberOfRowsExpected = context.ReadConfigAsInt32(testConfig, "NumberOfRowsExpected", true);			

            var rowCollection = testConfig.SelectSingleNode("Rows");
            var bamValidationRows = rowCollection.SelectNodes("*");
            foreach (XmlNode bamValidationRow in bamValidationRows)
            {
                var drtv = new DBRowToValidate();
                
                var bamValidationCols = bamValidationRow.SelectNodes("*");
                foreach (XmlNode bamValidationCol in bamValidationCols)
                {
                    bool isUnique = context.ReadConfigAsBool(bamValidationCol, "@isUnique", true);
                    string colName = bamValidationCol.LocalName;
                    string colValue = bamValidationCol.InnerText;
                    var dctv = new DBCellToValidate(colName, colValue);
                    if (isUnique)
                    {
                        drtv.AddUniqueCell(dctv);
                    }
                    else
                    {
                        drtv.AddCell(dctv);
                    }
                }

                DBRowsToValidate.AddRow(drtv);
            }

            Execute(context);
		}

		private static int ValidateData( object dbData, string targetValue, ref string dbDataStringValue)
		{
			dbDataStringValue = Convert.ToString(dbData);

		    switch (Type.GetTypeCode(dbData.GetType()))
			{
				case(TypeCode.DateTime):
					var dbDt = (DateTime)dbData;
					var targetDt = Convert.ToDateTime(targetValue);
					return targetDt.CompareTo( dbDt );

				case(TypeCode.DBNull):
					dbDataStringValue = "null";
					return targetValue.CompareTo( "null" );

				case(TypeCode.String):
					dbDataStringValue = (string)dbData;
					return targetValue.CompareTo( (string)dbData );
                     
				case(TypeCode.Int16):
					var dbInt16 = (Int16)dbData;
					var targetInt16 = Convert.ToInt16(targetValue);
					return targetInt16.CompareTo( dbInt16 );

				case(TypeCode.Int32):
					var dbInt32 = (Int32)dbData;
					var targetInt32 = Convert.ToInt32(targetValue);
					return targetInt32.CompareTo( dbInt32 );

				case(TypeCode.Int64):
					var dbInt64 = (Int64)dbData;
					var targetInt64 = Convert.ToInt64(targetValue);
					return targetInt64.CompareTo( dbInt64 );

				case(TypeCode.Double):
					var dbDouble = (Double)dbData;
					var targetDouble = Convert.ToDouble(targetValue);
					return targetDouble.CompareTo( dbDouble );

				case(TypeCode.Decimal):
					var dbDecimal = (Decimal)dbData;
					var targetDecimal = Convert.ToDecimal(targetValue);
					return targetDecimal.CompareTo( dbDecimal );

				case(TypeCode.Boolean):
					var dbBoolean = (Boolean)dbData;
					var targetBoolean = Convert.ToBoolean(targetValue);
					return targetBoolean.CompareTo( dbBoolean );

				case(TypeCode.Byte):
					var dbByte = (Byte)dbData;
					var targetByte = Convert.ToByte(targetValue);
					return targetByte.CompareTo( dbByte );

				case(TypeCode.Char):
					var dbChar = (Char)dbData;
					var targetChar = Convert.ToChar(targetValue);
					return targetChar.CompareTo( dbChar );

				case(TypeCode.SByte):
					var dbSByte = (SByte)dbData;
					var targetSByte = Convert.ToSByte(targetValue);
					return targetSByte.CompareTo( dbSByte );

				default:
					throw new InvalidOperationException(string.Format("Unsupported type: {0}", dbData.GetType()) );
			}
		}

        private static DataSet FillDataSet(string connectionString, string sqlQuery)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = null;
                        try
                        {
                            ds = new DataSet();
                            adapter.Fill(ds);
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
        }

		private static string BuildSqlQuery( XmlNode queryConfig, Context context )
		{
			var rawSqlQuery = context.ReadConfigAsString( queryConfig, "RawSQLQuery" );			
			var sqlParams = queryConfig.SelectNodes( "SQLQueryParams/*" );

			if ( null != sqlParams )
			{
				var paramArray = (from XmlNode sqlParam in sqlParams select context.ReadConfigAsString(sqlParam, "."));
				//context

			    var paramObjs = paramArray.Cast<object>().ToArray();
				return string.Format( rawSqlQuery, paramObjs );
			}

            return rawSqlQuery;
		}

        /// <summary>
        /// 
        /// </summary>
        public void Execute(Context context)
	    {
            context.LogInfo("Using database connection string: {0}", ConnectionString);
	        var sqlQueryToExecute = SQLQuery.GetFormattedSqlQuery();
            context.LogInfo("Executing database query: {0}", sqlQueryToExecute);

            // Sleep for delay seconds...
            Thread.Sleep(DelayBeforeExecution * 1000);


            var ds = FillDataSet(ConnectionString, sqlQueryToExecute);
	        
            if(NumberOfRowsExpected != ds.Tables[0].Rows.Count)
            {
                throw new InvalidOperationException(string.Format("Number of rows expected to be returned by the query does not match the value specified in the teststep. Number of rows the NnumberOfRowsExpected were: {0}, actual: {1}", NumberOfRowsExpected, ds.Tables[0].Rows.Count));
            }

            context.LogInfo("NumberOfRowsExpected: {0}, actual number returned: {1}", NumberOfRowsExpected, ds.Tables[0].Rows.Count);

            if (0 == NumberOfRowsExpected)
            {
                return;
            }

            if (null != DBRowsToValidate.RowsToValidate)
            {
                foreach (var row in DBRowsToValidate.RowsToValidate)
                {
                    DataRow bamDbRow;

                    // Get the element which has the unique flag on...
                    var uniqueCell = row.UniqueCell;
                    if (null != uniqueCell)
                    {
                        bamDbRow =
                            ds.Tables[0].Select(
                                string.Format("{0} = '{1}'", uniqueCell.ColumnName, uniqueCell.ExpectedValue)).First();
                    }
                    else
                    {
                        bamDbRow = ds.Tables[0].Rows[0];
                    }

                    var cells = row.Cells;
                    foreach (var cell in cells)
                    {
                        var dbData = bamDbRow[cell.ColumnName];
                        var dbDataStringValue = "";
                        if (0 == ValidateData(dbData, cell.ExpectedValue, ref dbDataStringValue))
                        {
                            context.LogInfo("Validation succeeded for field: {0} equals: {1}", cell.ColumnName,
                                            dbDataStringValue);
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                String.Format(
                                    "Validation failed for field: {0}. Expected value: {1}, actual value: {2}",
                                    cell.ColumnName, cell.ExpectedValue, dbDataStringValue));
                        }
                    }
                }
            }
	    }

        /// <summary>
        /// 
        /// </summary>
        public void Validate(Context context)
	    {
            // delayBeforeCheck - optional

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException("ConnectionString is either null or of zero length");
            }
            ConnectionString = context.SubstituteWildCards(ConnectionString);

            if (null == SQLQuery)
            {
                throw new ArgumentNullException("ConnectionString is either null or of zero length");
            }

            SQLQuery.Validate(context);
        }
	}

    /// <summary>
    /// 
    /// </summary>
    public class DBRowsToValidate
    {
        /// <summary>
        /// 
        /// </summary>
        public DBRowsToValidate()
        {
            RowsToValidate = new List<DBRowToValidate>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationArgs"></param>
        /// <exception cref="ArgumentException"></exception>
        public DBRowsToValidate(object[] validationArgs)
        {
            RowsToValidate = new List<DBRowToValidate>();
            ArgumentValidation.CheckForNullReference(validationArgs, "validationArgs");
            if (validationArgs.Length == 0)
            {
                throw new ArgumentException(
                    "The array objParams must be contain at least two objects, i.e. a column name and an expected value");
            }

            var drtv = new DBRowToValidate();
            for (int c = 0; c < validationArgs.Length; c += 2 )
            {
                drtv.AddCell(new DBCellToValidate((string)validationArgs[c], (string)validationArgs[c + 1]));    
            }
            RowsToValidate.Add(drtv);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbRowToValidate"></param>
        public void AddRow(DBRowToValidate dbRowToValidate)
        {
            RowsToValidate.Add(dbRowToValidate);
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<DBRowToValidate> RowsToValidate { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DBRowToValidate
    {
        private readonly IList<DBCellToValidate> _cells = new List<DBCellToValidate>();
        private readonly DBCellToValidate _uniqueCell;

        /// <summary>
        /// 
        /// </summary>
        public DBRowToValidate() {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueCell"></param>
        public DBRowToValidate(DBCellToValidate uniqueCell)
        {
            _uniqueCell = uniqueCell;
        }

        /// <summary>
        /// 
        /// </summary>
        public DBCellToValidate UniqueCell
        {
            get
            {
                return _uniqueCell;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(DBCellToValidate cell)
        {
            ArgumentValidation.CheckForNullReference(cell, "cell");

            _cells.Add(cell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        public void AddUniqueCell(DBCellToValidate cell)
        {
            ArgumentValidation.CheckForNullReference(cell, "cell");

            _cells.Add(cell);
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<DBCellToValidate> Cells
        {
            get
            {
                return _cells;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DBCellToValidate
    {
        private readonly string _columnName;
        private readonly string _expectedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="expectedValue"></param>
        public DBCellToValidate(string columnName, string expectedValue)
        {
            ArgumentValidation.CheckForEmptyString(columnName, "columnName");
            ArgumentValidation.CheckForEmptyString(expectedValue, "expectedValue");

            _columnName = columnName;
            _expectedValue = expectedValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get { return _columnName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExpectedValue
        {
            get { return _expectedValue; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DBRowsToValidateParamFormatter : ITestStepParameterFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            object[] retVal;

            if (typeof(DBRowsToValidate) == type)
            {
                object[] argsFetchedFromCtx = new object[args.Length];
                int c = 0;
                foreach (object arg in args)
                {
                    argsFetchedFromCtx[c++] = ctx.ReadArgument(arg);
                }

                retVal = new object[]{new DBRowsToValidate(argsFetchedFromCtx)};
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("The type {0} is not supported in DBRowsToValidateParamFormatter", type.FullName));
            }

            return retVal;
        }
    }
}
