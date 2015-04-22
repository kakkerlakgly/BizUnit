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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Soap
{
    /// <summary>
    /// </summary>
    public class WebServiceStep : TestStepBase
    {
        private Stream _request;
        private Stream _response;

        /// <summary>
        /// </summary>
        public WebServiceStep()
        {
            SoapHeaders = new List<SoapHeader>();
            SubSteps = new List<SubStepBase>();
        }

        /// <summary>
        /// </summary>
        public DataLoaderBase RequestBody { get; set; }

        /// <summary>
        /// </summary>
        public string ServiceUrl { get { return ServiceUri.AbsoluteUri; } set { ServiceUri = new Uri(value); } }

        /// <summary>
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        public IList<SoapHeader> SoapHeaders { private set; get; }

        /// <summary>
        /// </summary>
        /// <param name='context'></param>
        public override void Execute(Context context)
        {
            _request = RequestBody.Load(context);

            context.LogXmlData("Request", _request, true);

            _response = CallWebMethod(
                _request,
                ServiceUri,
                Action,
                Username,
                Password,
                context);

            var responseForPostProcessing = SubSteps.Aggregate(_response,
                (current, subStep) => subStep.Execute(current, context));
        }

        /// <summary>
        /// </summary>
        /// <param name='context'></param>
        public override void Validate(Context context)
        {
            if (ServiceUri == null)
            {
                throw new StepValidationException("ServiceUri may not be null", this);
            }

            if (string.IsNullOrEmpty(Action))
            {
                throw new StepValidationException("Action may not be null or empty", this);
            }

            RequestBody.Validate(context);
        }

        private Stream CallWebMethod(
            Stream requestData,
            Uri serviceUri,
            string action,
            string username,
            string password,
            Context ctx)
        {
            try
            {
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.UseDefaultWebProxy = true;

                var epa = new EndpointAddress(serviceUri);

                try
                {
                    using (var cf = new ChannelFactory<IGenericContract>(binding, epa))
                    {
                        cf.Credentials.UserName.UserName = username;
                        cf.Credentials.UserName.Password = password;

                        cf.Open();
                        var channel = cf.CreateChannel();
                        using (new OperationContextScope((IContextChannel) channel))
                        {
                            XmlReader r = new XmlTextReader(requestData);

                            using (var request = Message.CreateMessage(MessageVersion.Soap11, action, r))
                            {
                                foreach (var header in SoapHeaders)
                                {
                                    var messageHeader = MessageHeader.CreateHeader(header.HeaderName,
                                        header.HeaderNameSpace, header.HeaderInstance);
                                    OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader);
                                }

                                using (var response = channel.Invoke(request))
                                {
                                    var responseStr = response.GetReaderAtBodyContents().ReadOuterXml();
                                    ctx.LogXmlData("Response", responseStr);
                                    return StreamHelper.LoadMemoryStream(responseStr);
                                }
                            }
                        }
                    }
                }
                catch (CommunicationException ce)
                {
                    ctx.LogException(ce);
                    throw;
                }
                catch (TimeoutException te)
                {
                    ctx.LogException(te);
                    throw;
                }
                catch (Exception e)
                {
                    ctx.LogException(e);
                    throw;
                }
            }
            catch (Exception ex)
            {
                ctx.LogException(ex);
                throw;
            }
        }

        /// <summary>
        ///     A dummy WCF interface that will be manipulated by the CallWebMethod above
        /// </summary>
        [ServiceContract]
        private interface IGenericContract
        {
            [OperationContract(Action = "*", ReplyAction = "*")]
            Message Invoke(Message msg);
        }
    }
}