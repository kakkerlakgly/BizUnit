//---------------------------------------------------------------------
// File: ITestStepParameterFormatter.cs
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

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    /// <param name="ctx"></param>
    public delegate object[] TestStepParameterFormatter(Type type, object[] args, Context ctx);

    /// <summary>
    /// 
    /// </summary>
    [Obsolete("ITestStepParameterFormatter has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public interface ITestStepParameterFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        object[] FormatParameters(Type type, object[] args, Context ctx);
    }
}
