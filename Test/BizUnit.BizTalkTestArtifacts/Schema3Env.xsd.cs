namespace BizUnit.BizTalkTestArtifacts {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://BizUnit.BizTalkTestArtifacts.Schema3Env",@"Schema3Env")]
    [BodyXPath(@"/*[local-name()='Schema3Env' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema3Env']")]
    [Microsoft.XLANGs.BaseTypes.PropertyAttribute(typeof(global::BizUnit.BizTalkTestArtifacts.Property1), XPath = @"/*[local-name()='Schema3Env' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema3Env']/@*[local-name()='PromotedProperty' and namespace-uri()='']", XsdType = @"int")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Schema3Env"})]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizUnit.BizTalkTestArtifacts.PropertySchema1", typeof(global::BizUnit.BizTalkTestArtifacts.PropertySchema1))]
    public sealed class Schema3Env : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://BizUnit.BizTalkTestArtifacts.Schema3Env"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" xmlns:ns1=""http://BizUnit.BizTalkTestArtifacts.PropertySchema1"" targetNamespace=""http://BizUnit.BizTalkTestArtifacts.Schema3Env"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:annotation>
    <xs:appinfo>
      <b:schemaInfo is_envelope=""yes"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" />
      <b:imports>
        <b:namespace prefix=""ns1"" uri=""http://BizUnit.BizTalkTestArtifacts.PropertySchema1"" location=""BizUnit.BizTalkTestArtifacts.PropertySchema1"" />
      </b:imports>
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Schema3Env"">
    <xs:annotation>
      <xs:appinfo>
        <b:recordInfo body_xpath=""/*[local-name()='Schema3Env' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema3Env']"" />
        <b:properties>
          <b:property name=""ns1:Property1"" xpath=""/*[local-name()='Schema3Env' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema3Env']/@*[local-name()='PromotedProperty' and namespace-uri()='']"" />
        </b:properties>
      </xs:appinfo>
    </xs:annotation>
    <xs:complexType>
      <xs:sequence>
        <xs:any maxOccurs=""unbounded"" namespace=""##any"" processContents=""skip"" />
      </xs:sequence>
      <xs:attribute name=""PromotedProperty"" type=""xs:int"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public Schema3Env() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Schema3Env";
                return _RootElements;
            }
        }
        
        protected override object RawSchema {
            get {
                return _rawSchema;
            }
            set {
                _rawSchema = value;
            }
        }
    }
}
