﻿//---------------------------------------------------------------------
// File: CreateDirectory.cs
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
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.File
{
    /// <summary>
    ///     The CreateDirectory creates a new Directory.
    /// </summary>
    public class CreateDirectoryStep : TestStepBase
    {
        /// <summary>
        ///     The name of the directory to create.
        /// </summary>
        public string DirectoryName { get; set; }

        /// <summary>
        ///     TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            context.LogInfo("About to create the directory: {0}", DirectoryName);

            Directory.CreateDirectory(DirectoryName);
        }

        /// <summary>
        /// </summary>
        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(DirectoryName, "DirectoryName");
        }
    }
}