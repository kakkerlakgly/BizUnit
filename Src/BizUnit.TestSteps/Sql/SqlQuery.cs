using System;
using System.Collections.Generic;
using BizUnit.BizUnitOM;

namespace BizUnit.TestSteps.Sql
{
    /// <summary>
    ///     Database query definition
    /// </summary>
    public class SqlQuery
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public SqlQuery()
        {
            QueryParameters = new List<object>();
        }

        /// <summary>
        ///     The raw Sql query to be executed, can be include formatting instructions which are replaced with the
        ///     cref="QueryParameters". e.g. select * from dbo.MtTable where runTime > '{0}' AND SystemState = '{1}'
        /// </summary>
        public string RawSqlQuery { get; set; }

        /// <summary>
        ///     The parameters to substitute into the the cref="RawSqlQuery"
        /// </summary>
        public IList<object> QueryParameters { get; set; }

        /// <summary>
        ///     Formats the query string, replacing the formatting instructions in cref="RawSqlQuery" with the parameters in
        ///     cref="QueryParameters"
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        /// <returns></returns>
        public string GetFormattedSqlQuery(Context context)
        {
            if (QueryParameters.Count > 0)
            {
                var objParams = new object[QueryParameters.Count];
                var c = 0;

                foreach (var obj in QueryParameters)
                {
                    var objValue = obj.GetType() == typeof (ContextProperty)
                        ? ((ContextProperty) obj).GetPropertyValue(context)
                        : obj;

                    if (objValue is DateTime)
                    {
                        // Convert to SQL Datetime
                        objParams[c++] = ((DateTime) objValue).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        objParams[c++] = objValue;
                    }
                }

                return string.Format(RawSqlQuery, objParams);
            }

            return RawSqlQuery;
        }

        /// <summary>
        ///     Validates the SqlQuery
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        /// <exception cref="InvalidOperationException "></exception>
        public void Validate(Context context)
        {
            var sqlQuery = GetFormattedSqlQuery(context);
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new InvalidOperationException("The Sql Query cannot be formmatted correctly");
            }

            for (var c = 0; c < QueryParameters.Count; c++)
            {
                if (QueryParameters[c] is string)
                {
                    QueryParameters[c] = context.SubstituteWildCards((string) QueryParameters[c]);
                }
            }

            RawSqlQuery = context.SubstituteWildCards(RawSqlQuery);
        }
    }
}