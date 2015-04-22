//---------------------------------------------------------------------
// File: DatabaseHelper.cs
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
using System.Data.Odbc;
using System.Data.SqlClient;

namespace BizUnit.CoreSteps.Utilities
{
    /// <summary>
    ///     Static Helper for executing SQL statements
    /// </summary>
    public class DatabaseHelper
    {
        #region constructor(s)

        /// <summary>
        ///     Constructor for class, default constructor is private to prevent instances being
        ///     created as the class only has static methods
        /// </summary>
        private DatabaseHelper()
        {
        }

        #endregion // constructor(s)

        /// <summary>
        ///     /// Executes the SQL statement using ODBC
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlQuery">SQL statement to execute</param>
        internal static int ExecuteODBCNonQuery(string connectionString, string sqlQuery)
        {
            using (var connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                using (var command = new OdbcCommand(sqlQuery, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///     /// Executes the SQL statement and returns a single int
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlQuery">SQL statement to execute</param>
        internal static int ExecuteScalarODBCQuery(string connectionString, string sqlQuery)
        {
            using (var connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                using (var command = new OdbcCommand(sqlQuery, connection))
                {
                    return int.Parse(command.ExecuteScalar().ToString());
                }
            }
        }

        #region Static Methods

        /// <summary>
        ///     Excecutes the SQL statement against the database and returns a DataSet with the results
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        /// <returns>DataSet with the results of the executed command</returns>
        public static DataSet ExecuteSqlCommand(string connectionString, string sqlCommand)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var adapter = new SqlDataAdapter(sqlCommand, connection))
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
            } // connection
        }

        /// <summary>
        ///     Executes the SQL statement and returns the first column of the first row in the resultset returned by the query.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        /// <returns>The contents of the first column of the first row in the resultset</returns>
        public static int ExecuteScalar(string connectionString, string sqlCommand)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sqlCommand, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        ///     Executes the SQL statement
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        public static int ExecuteNonQuery(string connectionString, string sqlCommand)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sqlCommand, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        #endregion // Static Methods
    }
}