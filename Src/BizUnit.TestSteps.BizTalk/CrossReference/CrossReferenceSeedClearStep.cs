//---------------------------------------------------------------------
// File: CrossReferenceSeedClearStep.cs
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

using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.Win32;

namespace BizUnit.TestSteps.BizTalk.CrossReference
{
    /// <summary>
	/// The CrossReferenceSeedClearStep 
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.CrossReferenceSeedClearStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<LoginId>sa</LoginId> 
	///		<password>password</password> 
	///	</TestStep>
	/// </code>
	/// 
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>LoginId</term>
	///			<description>The logon Id</description>
	///		</item>
	///		<item>
	///			<term>password</term>
	///			<description>The password for the log on id</description>
	///		</item>
	///	</list>
	///	</remarks>

	public class CrossReferenceSeedClearStep: ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
		    string dbname;
		    string server;
		    using (var rk = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\BizTalk Server\\3.0\\Administration\\"))
		    {
		        dbname = rk.GetValue("MgmtDBName").ToString();
		        server = rk.GetValue("MgmtDBServer").ToString();
		    }
		    string connection = "Initial Catalog=" + dbname + " ;Data Source=" + server + ";Integrated Security=SSPI;";
		    XmlNode loginNode = testConfig.SelectSingleNode("LoginId");
		    XmlNode passwordNode = testConfig.SelectSingleNode("password");

		    if(loginNode != null && passwordNode != null)
		    {
		        connection = "Initial Catalog=" + dbname + " ;Data Source=" + server + ";User Id="+ loginNode.InnerText +";password=" + passwordNode.InnerText + ";";
		    }
		    else if(loginNode != null && passwordNode == null) //blank password !
		    {
		        connection = "Initial Catalog=" + dbname + " ;Data Source=" + server + ";User Id="+ loginNode.InnerText +";password=" + string.Empty + ";";
		    }

		    using (var conn = new SqlConnection(connection))
		    {
		        conn.Open();

		        using (var command = new SqlCommand("xref_Cleanup", conn))
		        {
                    command.CommandType = CommandType.StoredProcedure;
		            command.ExecuteNonQuery();
		        }
		    }

		    context.LogInfo("Cross reference tables were cleared.");
		}
	}
}
