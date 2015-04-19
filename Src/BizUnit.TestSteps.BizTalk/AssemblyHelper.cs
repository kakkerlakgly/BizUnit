//---------------------------------------------------------------------
// File: AssemblyHelper.cs
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

using System.IO;
using System.Reflection;

namespace BizUnit.TestSteps.BizTalk
{
    public static class AssemblyHelper
    {
        public static Assembly LoadAssembly(string path)
        {
            string filename = Path.GetFileName(path);
            string newPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);
            if (!System.IO.File.Exists(newPath))
            {
                System.IO.File.Copy(path, newPath, false);
            }

            return Assembly.LoadFrom(newPath);
        }
    }
}
