//---------------------------------------------------------------------
// File: DBExecuteNonQueryStep.cs
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
using System.Linq;
using System.Threading;
using System.Xml;
using BizUnit.BizUnitOM;
using BizUnit.Common;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
    /// <summary>
    ///     The DBExecuteNonQueryStep executes a non-query SQL statement. The number of rows affected is asserted if the
    ///     NumberOfRowsAffected element is specified
    /// </summary>
    /// <remarks>
    ///     The following shows an example of the Xml representation of this test step.
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath="" typeName="BizUnit.DBExecuteNonQueryStep">
    ///             <DelayBeforeExecution>1</DelayBeforeExecution>
    ///             <ConnectionString>
    ///                 Persist Security Info=False;Integrated
    ///                 Security=SSPI;database=BAMPrimaryImport;server=(local);Connect Timeout=30
    ///             </ConnectionString>
    ///             <NumberOfRowsAffected></NumberOfRowsAffected>
    ///             <!-- 
    /// 		The SQL Query to execute is built by formatting the RawSQLQuery substituting in the 
    /// 		SQLQueryParam's
    /// 		-->
    ///             <SQLQuery>
    ///                 <RawSQLQuery>INSERT INTO TABLE (COLUMN1, COLUMN2) VALUES (VALUE1, {0},{1} )</RawSQLQuery>
    ///                 <SQLQueryParams>
    ///                     <SQLQueryParam takeFromCtx="Key1"></SQLQueryParam>
    ///                     <SQLQueryParam takeFromCtx="Key2"></SQLQueryParam>
    ///                 </SQLQueryParams>
    ///             </SQLQuery>
    ///         </TestStep>
    /// 	</code>
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
    ///             <description>The connection string used for the DB query</description>
    ///         </item>
    ///         <item>
    ///             <term>NumberOfRowsAffected</term>
    ///             <description>
    ///                 The number of rows affected. This is an optional element. If specified, it causes the test step to
    ///                 raise an exception when the number of rows affected
    ///                 by executing the non-query does not match the specified value
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>SQLQuery/RawSQLQuery</term>
    ///             <description>The raw SQL string that will be formatted by substituting in the SQLQueryParam</description>
    ///         </item>
    ///         <item>
    ///             <term>SQLQuery/SQLQueryParams/SQLQueryParam</term>
    ///             <description>
    ///                 The parameters to substitute into RawSQLQuery
    ///                 <para>(repeating)</para>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Obsolete("DBExecuteNonQueryStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class DBExecuteNonQueryStep : ITestStepOM
    {
        /// <summary>
        /// </summary>
        public int DelayBeforeExecution { set; get; }

        /// <summary>
        /// </summary>
        public string ConnectionString { set; get; }

        /// <summary>
        /// </summary>
        public int NumberOfRowsAffected { set; get; }

        /// <summary>
        /// </summary>
        [BizUnitParameterFormatter("BizUnit.SqlQueryParamFormatter")]
        public SqlQuery SQLQuery { set; get; }

        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            DelayBeforeExecution = context.ReadConfigAsInt32(testConfig, "DelayBeforeExecution");
            ConnectionString = context.ReadConfigAsString(testConfig, "ConnectionString");
            NumberOfRowsAffected = testConfig.InnerXml.IndexOf("NumberOfRowsAffected", 0, testConfig.InnerXml.Length) !=
                                   -1
                ? context.ReadConfigAsInt32(testConfig, "NumberOfRowsAffected")
                : -1;
            var queryConfig = testConfig.SelectSingleNode("SQLQuery");
            SQLQuery = SqlQuery.BuildSQLQuery(queryConfig, context);

            Execute(context);
        }

        /// <summary>
        /// </summary>
        public void Execute(Context context)
        {
            context.LogInfo("Using database connection string: {0}", ConnectionString);
            context.LogInfo("Executing query: {0}", SQLQuery.GetFormattedSqlQuery());

            // Sleep for delay seconds...
            Thread.Sleep(DelayBeforeExecution*1000);

            var rowCount = DatabaseHelper.ExecuteNonQuery(ConnectionString, SQLQuery.GetFormattedSqlQuery());

            if (NumberOfRowsAffected != -1)
            {
                if (rowCount != NumberOfRowsAffected)
                    throw new InvalidOperationException(
                        "Number of rows affected by the query does not match the value specified in the teststep. Number of rows affected was " +
                        rowCount + " and value specified was " + NumberOfRowsAffected);
            }
            else
            {
                context.LogInfo(
                    "The number of rows affected by the query matched the value specified in the test step. " + rowCount +
                    "  rows were affected");
            }
        }

        /// <summary>
        /// </summary>
        public void Validate(Context context)
        {
            // delayBeforeExecution - optional

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException("ConnectionString is either null or of zero length");
            }
            ConnectionString = context.SubstituteWildCards(ConnectionString);

            // numberOfRowsAffected - no validation

            if (null == SQLQuery)
            {
                throw new ArgumentNullException("ConnectionString is either null or of zero length");
            }
            SQLQuery.Validate(context);
        }
    }

    /// <summary>
    /// </summary>
    public class SqlQuery
    {
        private readonly IList<object> _queryParameters = new List<object>();
        private object[] _objParams;
        private string _rawSqlQuery;

        /// <summary>
        /// </summary>
        public SqlQuery()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="rawSqlQuery"></param>
        public SqlQuery(string rawSqlQuery)
        {
            ArgumentValidation.CheckForEmptyString(rawSqlQuery, "rawSqlQuery");

            _rawSqlQuery = rawSqlQuery;
            _objParams = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="objParameters"></param>
        /// <exception cref="ArgumentException"></exception>
        public SqlQuery(object[] objParameters)
        {
            ArgumentValidation.CheckForNullReference(objParameters, "objParameters");
            if (objParameters.Length == 0)
            {
                throw new ArgumentException(
                    "The array objParams must be contain at least one object, i.e. the raw SQL Query text");
            }

            _rawSqlQuery = Convert.ToString(objParameters[0]);
            if (objParameters.Length > 1)
            {
                _objParams = new object[objParameters.Length - 1];
                var dst = 0;
                for (var src = 1; src < objParameters.Length; src++)
                {
                    if (objParameters[src] is DateTime)
                    {
                        // Convert to SQL Datetime
                        _objParams[dst++] = ((DateTime) objParameters[src]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        _objParams[dst++] = objParameters[src];
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="rawSqlQuery"></param>
        /// <param name="objParams"></param>
        public SqlQuery(string rawSqlQuery, object[] objParams)
        {
            ArgumentValidation.CheckForEmptyString(rawSqlQuery, "rawSqlQuery");
            ArgumentValidation.CheckForNullReference(objParams, "objParams");

            _rawSqlQuery = rawSqlQuery;
            _objParams = objParams;
        }

        /// <summary>
        /// </summary>
        public string RawSQLQuery
        {
            set { _rawSqlQuery = value; }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetFormattedSqlQuery()
        {
            if (_queryParameters.Count > 0)
            {
                _objParams = new object[_queryParameters.Count];
                var c = 0;
                foreach (var obj in _queryParameters)
                {
                    if (obj is DateTime)
                    {
                        // Convert to SQL Datetime
                        _objParams[c++] = ((DateTime) obj).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        _objParams[c++] = obj;
                    }
                }
            }
            else if (null == _objParams)
            {
                return _rawSqlQuery;
            }

            return string.Format(_rawSqlQuery, _objParams);
        }

        /// <summary>
        /// </summary>
        /// <param name="param"></param>
        public void AddSqlParam(object param)
        {
            ArgumentValidation.CheckForNullReference(param, "param");

            _queryParameters.Add(param);
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Validate(Context context)
        {
            var sqlQuery = GetFormattedSqlQuery();
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new ArgumentNullException("The Sql Query cannot be formmatted correctly");
            }

            for (var c = 0; c < _queryParameters.Count; c++)
            {
                if (_queryParameters[c] is string)
                {
                    _queryParameters[c] = context.SubstituteWildCards((string) _queryParameters[c]);
                }
            }

            _rawSqlQuery = context.SubstituteWildCards(_rawSqlQuery);
        }

        /// <summary>
        /// </summary>
        /// <param name="queryConfig"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SqlQuery BuildSQLQuery(XmlNode queryConfig, Context context)
        {
            var rawSQLQuery = context.ReadConfigAsString(queryConfig, "RawSQLQuery");
            var sqlParams = queryConfig.SelectNodes("SQLQueryParams/*");

            if (null != sqlParams)
            {
                var paramArray = (from XmlNode sqlParam in sqlParams select context.ReadConfigAsString(sqlParam, "."));
                //context

                var paramObjs = paramArray.Cast<object>().ToArray();
                return new SqlQuery(rawSQLQuery, paramObjs);
            }

            return new SqlQuery(rawSQLQuery);
        }
    }

    /// <summary>
    /// </summary>
    public class SqlQueryParamFormatter : ITestStepParameterFormatter
    {
        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            object[] retVal;

            if (typeof (SqlQuery) == type)
            {
                var argsFetchedFromCtx = new object[args.Length];
                var c = 0;
                foreach (var arg in args)
                {
                    argsFetchedFromCtx[c++] = ctx.ReadArgument(arg);
                }

                retVal = new object[1];
                retVal[0] = new SqlQuery(argsFetchedFromCtx);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format("The type {0} is not supported in SqlQueryParamFormatter", type.FullName));
            }

            return retVal;
        }
    }
}