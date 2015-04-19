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

using System.Linq;

namespace BizUnit.BizUnitOM
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    [Obsolete("DefaultTestStepParameterFormatter has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class DefaultTestStepParameterFormatter : ITestStepParameterFormatter
    {
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            object[] retVal;

            if (typeof(Double) == type)
            {
                retVal = new object[1];
                retVal[0] = Convert.ToDouble(ctx.ReadArgument(args[0]));
            }
            else if (typeof(Boolean) == type)
            {
                retVal = new object[1];
                retVal[0] = Convert.ToBoolean(ctx.ReadArgument(args[0]));
            }
            else if (typeof(Int16) == type)
            {
                retVal = new object[1];
                retVal[0] = Convert.ToInt16(ctx.ReadArgument(args[0]));
            }
            else if (typeof(Int32) == type)
            {
                retVal = new object[1];
                retVal[0] = Convert.ToInt32(ctx.ReadArgument(args[0]));
            }
            else if (typeof(Int64) == type)
            {
                retVal = new object[1];
                retVal[0] = Convert.ToInt64(ctx.ReadArgument(args[0]));
            }
            else if (typeof(String) == type)
            {
                retVal = new object[1];
                retVal[0] = ctx.ReadArgument(args[0]);
            }
            else if (typeof(IList<string>) == type)
            {
                IList<string> argsAsstringList = args.Select(arg => ctx.ReadArgument(arg).ToString()).ToList();
                retVal = new object[] {argsAsstringList};
            }
            else if (typeof(IList<Pair>) == type)
            {
                IList<Pair> argsAsPairList = new List<Pair>();
                for (int c = 0; c < args.Length; c += 2)
                {
                    argsAsPairList.Add(new Pair(ctx.ReadArgument(args[c]), ctx.ReadArgument(args[c + 1])));
                }
                retVal = new object[1];
                retVal[0] = argsAsPairList;
            }
            else
            {
                throw new ApplicationException(
                    string.Format("The type {0} is not supported in the BizBizUnit object model", type.FullName));
            }

            return retVal;
        }
    }
}
