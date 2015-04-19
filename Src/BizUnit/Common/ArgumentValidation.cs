//---------------------------------------------------------------------
// File: ArgumentValidation.cs
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

namespace BizUnit.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArgumentValidation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="varName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T CheckForNullReference<T>(T var, string varName)
        {
            if (varName == null)
                throw new ArgumentNullException("varName");

            if (var == null)
                throw new ArgumentNullException(varName);

            return var;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);

            if (variable.Length == 0)
                throw new ArgumentException("Expected non-empty string.", variableName);

            return variable;
        }
    }
}
