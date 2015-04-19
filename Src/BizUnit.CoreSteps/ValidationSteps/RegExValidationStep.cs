//---------------------------------------------------------------------
// File: RegExValidationStep.cs
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
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using BizUnit.BizUnitOM;

namespace BizUnit.CoreSteps.ValidationSteps
{
    /// <summary>
	/// The RegExValidationStep is used to validate a data stream by evaluating one or more regular expressions against it.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ValidationStep assemblyPath="" typeName="BizUnit.RegExValidationStep">
	///		<ValidationRegEx>Event Type   - Switch</ValidationRegEx>
	///		<ValidationRegEx>{1:[a-z]3</ValidationRegEx>
	///	</ValidationStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>ValidationRegEx</term>
	///			<description>The regualar expression to be evaluated <para>(repeating)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("RegExValidationStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class RegExValidationStep : IValidationStepOM
	{
	    /// <summary>
	    /// 
	    /// </summary>
	    public RegExValidationStep()
	    {
	        ValidationRegEx = new List<string>();
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    public IList<string> ValidationRegEx { get; set; }

	    /// <summary>
		/// IValidationStep.ExecuteValidation() implementation
		/// </summary>
		/// <param name='data'>The stream cintaining the data to be validated.</param>
		/// <param name='validatorConfig'>The Xml fragment containing the configuration for the test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
		{
			XmlNodeList validationNodes = validatorConfig.SelectNodes("ValidationRegEx");

			foreach (XmlNode validationNode in validationNodes)
			{
				ValidationRegEx.Add(validationNode.InnerText);
			}

            ExecuteValidation(data, context);
		}

	    public void ExecuteValidation(Stream data, Context context)
	    {
            StreamReader sr = new StreamReader(data);
            string strData = sr.ReadToEnd();

            foreach (string validationRegEx in ValidationRegEx)
            {
                Match match = Regex.Match(strData, validationRegEx);

                if (match.Success)
                {
                    context.LogInfo("Regex validation succeeded for pattern \"{0}\".", validationRegEx);
                }
                else
                {
                    throw new Exception(String.Format("Regex validation failed for pattern \"{0}\".", validationRegEx));
                }
            }
        }

        public void Validate(Context context)
	    {
            // validationRegEx - no validation to do
	    }
	}
}
