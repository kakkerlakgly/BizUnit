//---------------------------------------------------------------------
// File: DbQueryStep.cs
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
using System.Threading;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Sql
{
    ///<summary>
    ///</summary>
    public class DbQueryStep : TestStepBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SqlQuery SQLQuery { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<DbRowToValidate> DbRowsToValidate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumberOfRowsExpected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DelayBeforeCheck { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbQueryStep()
        {
            DbRowsToValidate = new List<DbRowToValidate>();
        }

        /// <summary>
        /// Execute()
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            context.LogInfo("Using database connection string: {0}", ConnectionString);
            string sqlQueryToExecute = SQLQuery.GetFormattedSqlQuery(context);

            // Sleep for delay seconds...
            if (0 < DelayBeforeCheck)
            {
                context.LogInfo("Sleeping for: {0} seconds", DelayBeforeCheck);
                Thread.Sleep(DelayBeforeCheck*1000);
            }

            context.LogInfo("Executing database query: {0}", sqlQueryToExecute);
            DataSet ds = FillDataSet(ConnectionString, sqlQueryToExecute);

            if (NumberOfRowsExpected != ds.Tables[0].Rows.Count)
            {
                throw new InvalidOperationException(string.Format("Number of rows expected to be returned by the query does not match the value specified in the teststep. Number of rows the NnumberOfRowsExpected were: {0}, actual: {1}", NumberOfRowsExpected, ds.Tables[0].Rows.Count));
            }

            context.LogInfo("NumberOfRowsExpected: {0}, actual number returned: {1}", NumberOfRowsExpected, ds.Tables[0].Rows.Count);

            if (0 == NumberOfRowsExpected)
            {
                return;
            }

            if (0 < DbRowsToValidate.Count)
            {
                int rowCount = 0;

                foreach (var dbRowToValidate in DbRowsToValidate)
                {
                    context.LogInfo("Validating row number: {0}", rowCount);

                    var resultRow = ds.Tables[0].Rows[rowCount];
                    var cells = dbRowToValidate.Cells;

                    foreach (DbCellToValidate cell in cells)
                    {
                        object dbData = resultRow[cell.ColumnName];
                        var dbDataStringValue = string.Empty;

                        if (0 == ValidateData(dbData, cell.ExpectedValue, ref dbDataStringValue))
                        {
                            context.LogInfo("Validation succeeded for field: {0}. Expected value: {1}", cell.ColumnName, dbDataStringValue);
                        }
                        else
                        {
                            throw new InvalidOperationException(String.Format("Validation failed for field: {0}. Expected value: {1}, actual value: {2}", cell.ColumnName, cell.ExpectedValue, dbDataStringValue));
                        }
                    }

                    rowCount++;
                }
            }
        }

        private static int ValidateData(object dbData, string targetValue, ref string dbDataStringValue)
        {
            dbDataStringValue = Convert.ToString(dbData);

            switch (Type.GetTypeCode(dbData.GetType()))
            {
                case (TypeCode.DateTime):
                    var dbDt = (DateTime)dbData;
                    DateTime targetDt = Convert.ToDateTime(targetValue);
                    return targetDt.CompareTo(dbDt);

                case (TypeCode.DBNull):
                    dbDataStringValue = "null";
                    return targetValue.CompareTo("null");

                case (TypeCode.String):
                    dbDataStringValue = (string)dbData;
                    return targetValue.CompareTo((string)dbData);

                case (TypeCode.Int16):
                    var dbInt16 = (Int16)dbData;
                    var targetInt16 = Convert.ToInt16(targetValue);
                    return targetInt16.CompareTo(dbInt16);

                case (TypeCode.Int32):
                    var dbInt32 = (Int32)dbData;
                    var targetInt32 = Convert.ToInt32(targetValue);
                    return targetInt32.CompareTo(dbInt32);

                case (TypeCode.Int64):
                    var dbInt64 = (Int64)dbData;
                    var targetInt64 = Convert.ToInt64(targetValue);
                    return targetInt64.CompareTo(dbInt64);

                case (TypeCode.Double):
                    var dbDouble = (Double)dbData;
                    var targetDouble = Convert.ToDouble(targetValue);
                    return targetDouble.CompareTo(dbDouble);

                case (TypeCode.Decimal):
                    var dbDecimal = (Decimal)dbData;
                    var targetDecimal = Convert.ToDecimal(targetValue);
                    return targetDecimal.CompareTo(dbDecimal);

                case (TypeCode.Boolean):
                    var dbBoolean = (Boolean)dbData;
                    var targetBoolean = Convert.ToBoolean(targetValue);
                    return targetBoolean.CompareTo(dbBoolean);

                case (TypeCode.Byte):
                    var dbByte = (Byte)dbData;
                    var targetByte = Convert.ToByte(targetValue);
                    return targetByte.CompareTo(dbByte);

                case (TypeCode.Char):
                    var dbChar = (Char)dbData;
                    var targetChar = Convert.ToChar(targetValue);
                    return targetChar.CompareTo(dbChar);

                case (TypeCode.SByte):
                    var dbSByte = (SByte)dbData;
                    var targetSByte = Convert.ToSByte(targetValue);
                    return targetSByte.CompareTo(dbSByte);

                default:
                    throw new InvalidOperationException(string.Format("Unsupported type: {0}", dbData.GetType()));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name='context'></param>
        public override void Validate(Context context)
        {
            // _delayBeforeCheck - optional

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
}
