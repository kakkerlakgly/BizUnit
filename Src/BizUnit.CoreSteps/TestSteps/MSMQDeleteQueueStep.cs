//---------------------------------------------------------------------
// File: MSMQDeleteQueueStep.cs
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
using System.Messaging;
using System.Xml;

namespace BizUnit.CoreSteps.TestSteps
{
    /// <summary>
    ///     The MSMQDeleteQueueStep deletes one or more MSMQ queues
    /// </summary>
    /// <remarks>
    ///     The following shows an example of the Xml representation of this test step.
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath="" typeName="BizUnit.MSMQDeleteQueueStep">
    ///             <QueuePath>.\Private$\Test01</QueuePath>
    ///             <QueuePath>.\Private$\Test02</QueuePath>
    ///         </TestStep>
    /// 	</code>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Tag</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>QueueName</term>
    ///             <description>The name of the MSMQ queue to delete, e.g. .\Private$\Test02
    ///                 <para>(one or more)</para>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Obsolete("MSMQDeleteQueueStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class MSMQDeleteQueueStep : ITestStep
    {
        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            var queues = testConfig.SelectNodes("*");

            foreach (XmlNode queue in queues)
            {
                var queuePath = queue.InnerText;
                MessageQueue.Delete(queuePath);

                context.LogInfo("The queue: \"{0}\" was deleted successfully.", queuePath);
            }
        }
    }
}