//---------------------------------------------------------------------
// File: Pop3LoginException.cs
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

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    internal class Pop3LoginException : Exception
    {
        private readonly string _exceptionString;

        internal Pop3LoginException()
        {
            _exceptionString = null;
        }

        internal Pop3LoginException(string exceptionString)
        {
            _exceptionString = exceptionString;
        }

        internal Pop3LoginException(string exceptionString, Exception ex) : base(exceptionString, ex)
        {
        }

        public override string ToString()
        {
            return _exceptionString;
        }
    }
}