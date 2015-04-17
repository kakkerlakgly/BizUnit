namespace BizUnit.BizTalkTestArtifacts {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://BizUnit.BizTalkTestArtifacts.Schema2",@"Schema2Root")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Schema2Root"})]
    public sealed class Schema2 : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://BizUnit.BizTalkTestArtifacts.Schema2"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""http://BizUnit.BizTalkTestArtifacts.Schema2"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""Schema2Root"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Child1"">
          <xs:complexType>
            <xs:attribute name=""Child1Attribute1"" type=""xs:int"" />
            <xs:attribute name=""Child1Attribute2"" type=""xs:string"" />
          </xs:complexType>
        </xs:element>
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
        
        public Schema2() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Schema2Root";
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
