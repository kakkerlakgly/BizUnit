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
        public void MergeIntoMessage(IBaseMessage message)
        {
            foreach (MessageInfoContextInfoProperty prop in MessageContextProperties)
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

        private MessageInfoContextInfo _mici;
        public MessageInfoContextInfo MessageInfoContextInfo
        {
            get
            {
                if (null == _mici)
                {
                    bool found = false;
                    int index = 0;
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

        private MessageInfoPartInfo _mipi;
        public MessageInfoPartInfo MessageInfoPartInfo
        {
            get
            {
                if (null == _mipi)
                {
                    bool found = false;
                    int index = 0;
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

        private static XmlSerializer _messageInfoSerializer = new XmlSerializer(typeof(MessageInfo));
        public static MessageInfo Deserialize(string path)
        {
            object obj = null;
            using (XmlReader reader = XmlReader.Create(path))
            {
                obj = _messageInfoSerializer.Deserialize(reader);
            }

            return obj as MessageInfo;
        }

        public static void Serialize(MessageInfo mi, string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path, BizTalkMapTester.WriterSettings))
            {
                _messageInfoSerializer.Serialize(writer, mi);
            }
        }
    }
}
