//---------------------------------------------------------------------
// File: FileMoveStep.cs
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
using BizUnit.Xaml;

namespace BizUnit.TestSteps.File
{
    /// <summary>
    ///     The FileMoveStep moves a file from one directory to another.
    /// </summary>
    public class MoveStep : TestStepBase
    {
        /// <summary>
        ///     Gets or sets the source path.
        /// </summary>
        /// <value>The source path.</value>
        public string SourcePath { get; set; }

        /// <summary>
        ///     Gets or sets the destination path.
        /// </summary>
        /// <value>The destination path.</value>
        public string DestinationPath { get; set; }

        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            System.IO.File.Move(SourcePath, DestinationPath);

            context.LogInfo("FileMoveStep has moved file: \"{0}\" to \"{1}\"", SourcePath, DestinationPath);
        }

        /// <summary>
        /// </summary>
        /// <param name='context'></param>
        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(DestinationPath))
            {
                throw new InvalidOperationException("DestinationPath is either null or of zero length");
            }
            DestinationPath = context.SubstituteWildCards(DestinationPath);

            if (string.IsNullOrEmpty(SourcePath))
            {
                throw new InvalidOperationException("SourcePath is either null or of zero length");
            }
            SourcePath = context.SubstituteWildCards(SourcePath);
        }
    }
}