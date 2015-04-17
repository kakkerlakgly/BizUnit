namespace BizUnit.BizTalkTestArtifacts {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://BizUnit.BizTalkTestArtifacts.Schema1",@"Schema1Root")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Schema1Root"})]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizUnit.BizTalkTestArtifacts.Schema0", typeof(global::BizUnit.BizTalkTestArtifacts.Schema0))]
    public sealed class Schema1 : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://BizUnit.BizTalkTestArtifacts.Schema1"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" xmlns:ns0=""http://BizUnit.BizTalkTestArtifacts.Schema0"" targetNamespace=""http://BizUnit.BizTalkTestArtifacts.Schema1"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:import schemaLocation=""BizUnit.BizTalkTestArtifacts.Schema0"" namespace=""http://BizUnit.BizTalkTestArtifacts.Schema0"" />
  <xs:annotation>
    <xs:appinfo>
      <b:references>
        <b:reference targetNamespace=""http://BizUnit.BizTalkTestArtifacts.Schema0"" />
      </b:references>
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Schema1Root"">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref=""ns0:Child"" />
        <xs:element name=""Child2"">
          <xs:complexType>
            <xs:attribute name=""Child2Attribute1"" type=""xs:string"" />
            <xs:attribute name=""Child2Attribute2"" type=""xs:string"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public Schema1() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Schema1Root";
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
