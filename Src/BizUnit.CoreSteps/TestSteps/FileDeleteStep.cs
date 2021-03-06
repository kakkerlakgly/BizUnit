//---------------------------------------------------------------------
// File: FileDeleteStep.cs
// 
// Summary: 
//
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using BizUnit.BizUnitOM;

namespace BizUnit.CoreSteps.TestSteps
{
    /// <summary>
    ///     The FileDeleteStep deletes a FILE specified at a given location.
    /// </summary>
    /// <remarks>
    ///     The following shows an example of the Xml representation of this test step.
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath="" typeName="BizUnit.FileDeleteStep">
    ///             <FileToDeletePath>.\Rec_01\InDoc1.xml</FileToDeletePath>
    ///             <FileToDeletePath>.\Rec_01\InDoc2.xml</FileToDeletePath>
    ///         </TestStep>
    /// 	</code>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Tag</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>FileToDeletePath</term>
    ///             <description>The location of FILE to be deleted
    ///                 <para>(one or more)</para>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Obsolete("FileDeleteStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class FileDeleteStep : ITestStepOM
    {
        private IList<string> _filesToDelete;

        /// <summary>
        /// </summary>
        [BizUnitParameterFormatter("BizUnit.BizUnitOM.ComplexTypeParameterFormatter")]
        public IList<string> FilesToDeletePath
        {
            set { _filesToDelete = value; }
        }

        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            var files = testConfig.SelectNodes("*");

            foreach (XmlNode file in files)
            {
                DeleteFile(file.InnerText, context);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name='context'></param>
        public void Execute(Context context)
        {
            foreach (var file in _filesToDelete)
            {
                DeleteFile(file, context);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name='context'></param>
        public void Validate(Context context)
        {
            if (null == _filesToDelete || 0 == _filesToDelete.Count)
            {
                throw new InvalidOperationException("FilesToDelete is either null or of zero length");
            }

            for (var c = 0; c < _filesToDelete.Count; c++)
            {
                _filesToDelete[c] = context.SubstituteWildCards(_filesToDelete[c]);
            }
        }

        private static void DeleteFile(string fileToDeletePath, Context context)
        {
            File.Delete(fileToDeletePath);

            context.LogInfo("FileDeleteStep has deleted file: {0}", fileToDeletePath);
        }
    }
}