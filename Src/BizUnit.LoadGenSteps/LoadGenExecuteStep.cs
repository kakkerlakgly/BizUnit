//---------------------------------------------------------------------
// File: LoadGenExecuteStep.cs
// 
// Summary: 
//
// Author: Kevin B. Smith
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
using System.Globalization;
using System.Threading;
using System.Xml;
using LoadGen;

namespace BizUnit.LoadGenSteps
{
    /// <summary>
    ///     The LoadGenExecuteStep step executes a LoadGen test
    /// </summary>
    /// <remarks>
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath=""
    ///             typeName="BizUnit.LoadGenSteps.LoadGenExecuteStep, BizUnit.LoadGenSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///             <LoadGenTestConfig>c:\LoadTests\MemphisFeedPerfTest001.xml</LoadGenTestConfig>
    ///         </TestStep>
    ///  </code>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Tag</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>LoadGenTestConfig</term>
    ///             <description>The path to the LoadGen test configuration</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Obsolete("LoadGenExecuteStep has been deprecated.")]
    public class LoadGenExecuteStep : ITestStep
    {
        private bool _bExitApp;
        private Context _ctx;

        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            _ctx = context;
            var loadGenTestConfig = context.ReadConfigAsString(testConfig, "LoadGenTestConfig");

            try
            {
                context.LogInfo("About to execute LoadGen script: {0}", loadGenTestConfig);

                var doc = new XmlDocument();
                doc.Load(loadGenTestConfig);

                if (string.Compare(doc.FirstChild.Name, "LoadGenFramework", true, new CultureInfo("en-US")) != 0)
                {
                    throw new ConfigException("LoadGen Configuration File Schema Invalid!");
                }

                var loadGen = new LoadGen.LoadGen(doc.FirstChild);
                loadGen.LoadGenStopped += LoadGenStopped;
                loadGen.Start();
            }
            catch (ConfigException cex)
            {
                context.LogError(cex.Message);
                throw;
            }
            catch (Exception ex)
            {
                context.LogError(ex.Message);
                throw;
            }

            while (!_bExitApp)
            {
                Thread.Sleep(0x3e8);
            }
            Thread.Sleep(0x1388);
        }

        private void LoadGenStopped(object sender, LoadGenStopEventArgs e)
        {
            var span1 = e.LoadGenStopTime.Subtract(e.LoadGenStartTime);
            _ctx.LogInfo("FilesSent: " + e.NumFilesSent);
            _ctx.LogInfo("StartTime: " + e.LoadGenStartTime);
            _ctx.LogInfo("StopTime:  " + e.LoadGenStopTime);
            _ctx.LogInfo("DeltaTime: " + span1.TotalSeconds + "Secs.");
            _ctx.LogInfo("Rate:      " + ((e.NumFilesSent)/span1.TotalSeconds));

            _bExitApp = true;
        }
    }
}