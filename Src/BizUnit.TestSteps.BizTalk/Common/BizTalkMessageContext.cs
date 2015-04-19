﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BizUnit.TestSteps.BizTalk {
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    [XmlRoot(Namespace="", IsNullable=false)]
    public partial class MessageInfo {
        
        private object[] itemsField;
        
        /// <remarks/>
        [XmlElement("ContextInfo", typeof(MessageInfoContextInfo), Form=XmlSchemaForm.Unqualified)]
        [XmlElement("PartInfo", typeof(MessageInfoPartInfo), Form=XmlSchemaForm.Unqualified)]
        public object[] Items {
            get {
                return itemsField;
            }
            set {
                itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    public partial class MessageInfoContextInfo {
        
        private MessageInfoContextInfoProperty[] propertyField;
        
        private MessageInfoContextInfoArrayProperty[] arrayPropertyField;
        
        private string propertiesCountField;
        
        /// <remarks/>
        [XmlElement("Property", Form=XmlSchemaForm.Unqualified)]
        public MessageInfoContextInfoProperty[] Property {
            get {
                return propertyField;
            }
            set {
                propertyField = value;
            }
        }
        
        /// <remarks/>
        [XmlElement("ArrayProperty", Form=XmlSchemaForm.Unqualified)]
        public MessageInfoContextInfoArrayProperty[] ArrayProperty {
            get {
                return arrayPropertyField;
            }
            set {
                arrayPropertyField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string PropertiesCount {
            get {
                return propertiesCountField;
            }
            set {
                propertiesCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    public partial class MessageInfoContextInfoProperty {
        
        private string nameField;
        
        private string namespaceField;
        
        private string valueField;
        
        private bool promotedField;
        
        private bool promotedFieldSpecified;
        
        /// <remarks/>
        [XmlAttribute()]
        public string Name {
            get {
                return nameField;
            }
            set {
                nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string Namespace {
            get {
                return namespaceField;
            }
            set {
                namespaceField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string Value {
            get {
                return valueField;
            }
            set {
                valueField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public bool Promoted {
            get {
                return promotedField;
            }
            set {
                promotedField = value;
            }
        }
        
        /// <remarks/>
        [XmlIgnore()]
        public bool PromotedSpecified {
            get {
                return promotedFieldSpecified;
            }
            set {
                promotedFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    public partial class MessageInfoContextInfoArrayProperty {
        
        private MessageInfoContextInfoArrayPropertyArrayElement1[] arrayElement1Field;
        
        private string nameField;
        
        private string namespaceField;
        
        /// <remarks/>
        [XmlElement("ArrayElement1", Form=XmlSchemaForm.Unqualified)]
        public MessageInfoContextInfoArrayPropertyArrayElement1[] ArrayElement1 {
            get {
                return arrayElement1Field;
            }
            set {
                arrayElement1Field = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string Name {
            get {
                return nameField;
            }
            set {
                nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string Namespace {
            get {
                return namespaceField;
            }
            set {
                namespaceField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    public partial class MessageInfoContextInfoArrayPropertyArrayElement1 {
        
        private string valueField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string Value {
            get {
                return valueField;
            }
            set {
                valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    public partial class MessageInfoPartInfo {
        
        private MessageInfoPartInfoMessagePart[] messagePartField;
        
        private string partsCountField;
        
        /// <remarks/>
        [XmlElement("MessagePart", Form=XmlSchemaForm.Unqualified)]
        public MessageInfoPartInfoMessagePart[] MessagePart {
            get {
                return messagePartField;
            }
            set {
                messagePartField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string PartsCount {
            get {
                return partsCountField;
            }
            set {
                partsCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.42")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true)]
    public partial class MessageInfoPartInfoMessagePart {
        
        private string idField;
        
        private string nameField;
        
        private string fileNameField;
        
        private string charsetField;
        
        private string contentTypeField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string ID {
            get {
                return idField;
            }
            set {
                idField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string Name {
            get {
                return nameField;
            }
            set {
                nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string FileName {
            get {
                return fileNameField;
            }
            set {
                fileNameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string Charset {
            get {
                return charsetField;
            }
            set {
                charsetField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string ContentType {
            get {
                return contentTypeField;
            }
            set {
                contentTypeField = value;
            }
        }
    }
}
