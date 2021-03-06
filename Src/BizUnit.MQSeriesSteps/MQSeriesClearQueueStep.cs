//---------------------------------------------------------------------
// File: MQSeriesClearQueueStep.cs
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
using IBM.WMQ;

namespace BizUnit.MQSeriesSteps
{
    // $:\Program Files\IBM\WebSphere MQ\bin\amqmdnet.dll - Requires Patch Level > CSD07

    /// <summary>
    ///     The MQSeriesClearQueueStep test step clears one or more MQ Series queues, this test step is typically used to
    ///     cleanup a test case.
    /// </summary>
    /// <remarks>
    ///     Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
    ///     <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
    ///     <para>The following shows an example of the Xml representation of this test step.</para>
    ///     <code escaped="true">
    /// 	<TestStep assemblyPath=""
    ///             typeName="BizUnit.MQSeriesSteps.MQSeriesClearQueueStep, BizUnit.MQSeriesSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///             <QueueManager>QM_server</QueueManager>
    ///             <Queue>QUEUE_1</Queue>
    ///             <Queue>QUEUE_2</Queue>
    ///         </TestStep>
    /// 	</code>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Tag</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>QueueManager</term>
    ///             <description>The name of the MQ Series queue manager</description>
    ///         </item>
    ///         <item>
    ///             <term>Queue</term>
    ///             <description>The name of the MQ Series queue to clear. (one or more)</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Obsolete("MQSeriesClearQueueStep has been deprecated.")]
    public class MQSeriesClearQueueStep : ITestStep
    {
        /// <summary>
        ///     ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            var qmgr = context.ReadConfigAsString(testConfig, "QueueManager");

            try
            {
                context.LogInfo("Opening queue manager '{0}'.", qmgr);
                using (var queueManager = new MQQueueManager(qmgr))
                {
                    var queueNodes = testConfig.SelectNodes("Queue");
                    foreach (XmlNode queueNode in queueNodes)
                    {
                        var q = queueNode.InnerText;
                        context.LogInfo("Opening queue '{0}'.", q);
                        using (
                            var queue = queueManager.AccessQueue(q,
                                MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING))
                        {
                            try
                            {
                                var mqMsg = new MQMessage();
                                var mqMsgOpts = new MQGetMessageOptions();

                                var i = 0;
                                while (true)
                                {
                                    try
                                    {
                                        // Get message from queue
                                        queue.Get(mqMsg, mqMsgOpts);
                                        i++;
                                    }
                                    catch (MQException mqe)
                                    {
                                        if (mqe.Reason == 2033) // No more messages.
                                        {
                                            break;
                                        }
                                        throw;
                                    }
                                }

                                context.LogInfo("Cleared {0} messages from queue '{1}'.", i, q);
                            }
                            catch (Exception e)
                            {
                                context.LogError("Failed to clear queue \"{0}\" with the following exception: {1}", q,
                                    e.ToString());
                                throw;
                            }
                        }
                    }
                }
            }
            catch (MQException e)
            {
                throw new InvalidOperationException(string.Format("Failed to open queue manager {0}.", qmgr), e);
            }
        }
    }
}