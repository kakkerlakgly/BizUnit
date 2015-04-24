//---------------------------------------------------------------------
// File: DefaultTestStepParameterFormatter.cs
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
using System.Web.UI;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// </summary>
    [Obsolete(
        "DefaultTestStepParameterFormatter has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class DefaultTestStepParameterFormatter : ITestStepParameterFormatter
    {
        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            if (typeof (double) == type)
            {
                return new object[] {Convert.ToDouble(ctx.ReadArgument(args[0]))};
            }
            if (typeof (bool) == type)
            {
                return new object[] {Convert.ToBoolean(ctx.ReadArgument(args[0]))};
            }
            if (typeof (short) == type)
            {
                return new object[] {Convert.ToInt16(ctx.ReadArgument(args[0]))};
            }
            if (typeof (int) == type)
            {
                return new object[] {Convert.ToInt32(ctx.ReadArgument(args[0]))};
            }
            if (typeof (long) == type)
            {
                return new object[] {Convert.ToInt64(ctx.ReadArgument(args[0]))};
            }
            if (typeof (string) == type)
            {
                return new[] {ctx.ReadArgument(args[0])};
            }
            if (typeof (IList<string>) == type)
            {
                IList<string> argsAsstringList = args.Select(arg => ctx.ReadArgument(arg).ToString()).ToList();
                return new object[] {argsAsstringList};
            }
            if (typeof (IList<Pair>) == type)
            {
                IList<Pair> argsAsPairList = new List<Pair>();
                for (var c = 0; c < args.Length; c += 2)
                {
                    argsAsPairList.Add(new Pair(ctx.ReadArgument(args[c]), ctx.ReadArgument(args[c + 1])));
                }
                return new object[] {argsAsPairList};
            }
            throw new ArgumentException(
                string.Format("The type {0} is not supported in the BizBizUnit object model", type.FullName), "type");
        }
    }
}