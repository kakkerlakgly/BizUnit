namespace SampleProject {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://bizUnit.sdk.flightUpgrade/upgradeRequest",@"UpgradeRequest")]
    [Microsoft.XLANGs.BaseTypes.DistinguishedFieldAttribute(typeof(System.String), "CustomerDetails.FirstName", XPath = @"/*[local-name()='UpgradeRequest' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeRequest']/*[local-name()='CustomerDetails' and namespace-uri()='']/*[local-name()='FirstName' and namespace-uri()='']", XsdType = @"string")]
    [Microsoft.XLANGs.BaseTypes.DistinguishedFieldAttribute(typeof(System.String), "CustomerDetails.LastName", XPath = @"/*[local-name()='UpgradeRequest' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeRequest']/*[local-name()='CustomerDetails' and namespace-uri()='']/*[local-name()='LastName' and namespace-uri()='']", XsdType = @"string")]
    [Microsoft.XLANGs.BaseTypes.PropertyAttribute(typeof(global::BizUnit.Sdk.FlightUpgrade.FlightBookingNumber), XPath = @"/*[local-name()='UpgradeRequest' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeRequest']/*[local-name()='FlightDetails' and namespace-uri()='']/*[local-name()='FlightBookingNumber' and namespace-uri()='']", XsdType = @"string")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"UpgradeRequest"})]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizUnit.Sdk.FlightUpgrade.FlightProperties", typeof(global::BizUnit.Sdk.FlightUpgrade.FlightProperties))]
    public sealed class RequestMsg : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://bizUnit.sdk.flightUpgrade/upgradeRequest"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" xmlns:ns0=""https://BizUnit.Sdk.FlightUpgrade.FlightProperties"" targetNamespace=""http://bizUnit.sdk.flightUpgrade/upgradeRequest"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:annotation>
    <xs:appinfo>
      <b:imports>
        <b:namespace prefix=""ns0"" uri=""https://BizUnit.Sdk.FlightUpgrade.FlightProperties"" location=""BizUnit.Sdk.FlightUpgrade.FlightProperties"" />
      </b:imports>
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""UpgradeRequest"">
    <xs:annotation>
      <xs:appinfo>
        <b:properties>
          <b:property distinguished=""true"" xpath=""/*[local-name()='UpgradeRequest' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeRequest']/*[local-name()='CustomerDetails' and namespace-uri()='']/*[local-name()='FirstName' and namespace-uri()='']"" />
          <b:property distinguished=""true"" xpath=""/*[local-name()='UpgradeRequest' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeRequest']/*[local-name()='CustomerDetails' and namespace-uri()='']/*[local-name()='LastName' and namespace-uri()='']"" />
          <b:property name=""ns0:FlightBookingNumber"" xpath=""/*[local-name()='UpgradeRequest' and namespace-uri()='http://bizUnit.sdk.flightUpgrade/upgradeRequest']/*[local-name()='FlightDetails' and namespace-uri()='']/*[local-name()='FlightBookingNumber' and namespace-uri()='']"" />
        </b:properties>
      </xs:appinfo>
    </xs:annotation>
    <xs:complexType>
      <xs:sequence>
        <xs:element minOccurs=""0"" maxOccurs=""1"" name=""CustomerDetails"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""FirstName"" type=""xs:string"" />
              <xs:element name=""LastName"" type=""xs:string"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element minOccurs=""0"" maxOccurs=""1"" name=""FlightDetails"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""FlightBookingNumber"" type=""xs:string"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element minOccurs=""0"" maxOccurs=""1"" name=""UpgradeDetails"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""RequestedCabin"" type=""xs:string"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public RequestMsg() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "UpgradeRequest";
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
