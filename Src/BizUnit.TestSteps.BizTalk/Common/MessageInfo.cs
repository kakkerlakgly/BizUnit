//---------------------------------------------------------------------
// File: MessageInfo.cs
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

using System.Xml;
using System.Xml.Serialization;
using BizUnit.TestSteps.BizTalk.Map;
using Microsoft.BizTalk.Message.Interop;

namespace BizUnit.TestSteps.BizTalk
{
    public partial class MessageInfo
    {
        private static XmlSerializer _messageInfoSerializer = new XmlSerializer(typeof (MessageInfo));
        private MessageInfoContextInfo _mici;
        private MessageInfoPartInfo _mipi;

        /// <summary>
        /// </summary>
        public MessageInfoContextInfoProperty[] MessageContextProperties
        {
            get
            {
                if (null != MessageInfoContextInfo)
                {
                    return MessageInfoContextInfo.Property;
                }
                return null;
            }
        }

        /// <summary>
        /// </summary>
        public MessageInfoContextInfo MessageInfoContextInfo
        {
            get
            {
                if (null == _mici)
                {
                    var found = false;
                    var index = 0;
                    while (!found && (index < Items.Length))
                    {
                        _mici = Items[index] as MessageInfoContextInfo;
                        found = (null != _mici);
                        index++;
                    }
                }
                return _mici;
            }
        }

        /// <summary>
        /// </summary>
        public MessageInfoPartInfo MessageInfoPartInfo
        {
            get
            {
                if (null == _mipi)
                {
                    var found = false;
                    var index = 0;
                    while (!found && (index < Items.Length))
                    {
                        _mipi = Items[index] as MessageInfoPartInfo;
                        found = (null != _mipi);
                        index++;
                    }
                }
                return _mipi;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public void MergeIntoMessage(IBaseMessage message)
        {
            foreach (var prop in MessageContextProperties)
            {
                if (prop.Promoted)
                {
                    message.Context.Promote(prop.Name, prop.Namespace, prop.Value);
                }
                else
                {
                    message.Context.Write(prop.Name, prop.Namespace, prop.Value);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MessageInfo Deserialize(string path)
        {
            object obj = null;
            using (var reader = XmlReader.Create(path))
            {
                obj = _messageInfoSerializer.Deserialize(reader);
            }

            return obj as MessageInfo;
        }

        /// <summary>
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="path"></param>
        public static void Serialize(MessageInfo mi, string path)
        {
            using (var writer = XmlWriter.Create(path, BizTalkMapTester.WriterSettings))
            {
                _messageInfoSerializer.Serialize(writer, mi);
            }
        }
    }
}