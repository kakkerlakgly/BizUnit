//---------------------------------------------------------------------
// File: ContextLoaderStepBuilder.cs
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
using System.Xml;
using BizUnit.Common;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The ContextLoaderStepBuilder abstracts a validation sub step, it is responsible for 
    /// creating and configuring a validation sub step that implements IContextLoaderStepOM.
    /// </summary>
    [Obsolete("ContextLoaderStepBuilder has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class ContextLoaderStepBuilder : TestStepBuilderBase
    {
        private readonly IContextLoaderStepOM _contextLoaderStep;

        /// <summary>
        /// ContextLoaderStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='config'>The Xml configuration for a context loader sub step that 
        /// implements the ITestStep interface.</param>
        public ContextLoaderStepBuilder(XmlNode config)
            : base(config) {}

        /// <summary>
        /// ContextLoaderStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='typeName'>The type name of the test step to be created by the builder.</param>
        /// <param name='assemblyPath'>The assembly path name of the context loader sub step to 
        /// be created by the builder.</param>
        public ContextLoaderStepBuilder(string typeName, string assemblyPath)
            : base(typeName, assemblyPath)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");
            // assemblyPath - optional

            _contextLoaderStep = TestStep as IContextLoaderStepOM;
            if (null == _contextLoaderStep)
            {
                throw new ArgumentException("The validation step created is invalid: IContextLoaderStepOM is not implemented");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IContextLoaderStepOM ContextLoaderStep
        {
            get
            {
                return _contextLoaderStep;
            }
        }
    }
}
