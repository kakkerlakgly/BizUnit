namespace SampleProject {
    
    
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"SampleProject.RequestMsg", typeof(global::SampleProject.RequestMsg))]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"SampleProject.ResponseMsg", typeof(global::SampleProject.ResponseMsg))]
    public sealed class NoUpgradeMap : global::Microsoft.XLANGs.BaseTypes.TransformBase {
        
        private const string _strMap = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:var=""http://schemas.microsoft.com/BizTalk/2003/var"" exclude-result-prefixes=""msxsl var s0"" version=""1.0"" xmlns:s0=""http://bizUnit.sdk.flightUpgrade/upgradeRequest"" xmlns:ns0=""http://bizUnit.sdk.flightUpgrade/upgradeResponse"">
  <xsl:output omit-xml-declaration=""yes"" method=""xml"" version=""1.0"" />
  <xsl:template match=""/"">
    <xsl:apply-templates select=""/s0:UpgradeRequest"" />
  </xsl:template>
  <xsl:template match=""/s0:UpgradeRequest"">
    <ns0:UpgradeResponse>
      <xsl:for-each select=""CustomerDetails"">
        <CustomerDetails>
          <FirstName>
            <xsl:value-of select=""FirstName/text()"" />
          </FirstName>
          <LastName>
            <xsl:value-of select=""LastName/text()"" />
          </LastName>
        </CustomerDetails>
      </xsl:for-each>
      <xsl:for-each select=""FlightDetails"">
        <FlightDetails>
          <FlightBookingNumber>
            <xsl:value-of select=""FlightBookingNumber/text()"" />
          </FlightBookingNumber>
        </FlightDetails>
      </xsl:for-each>
      <UpgradeResult>
        <Result>
          <xsl:text>false</xsl:text>
        </Result>
      </UpgradeResult>
    </ns0:UpgradeResponse>
  </xsl:template>
</xsl:stylesheet>";
        
        private const string _strArgList = @"<ExtensionObjects />";
        
        private const string _strSrcSchemasList0 = @"SampleProject.RequestMsg";
        
        private const string _strTrgSchemasList0 = @"SampleProject.ResponseMsg";
        
        public override string XmlContent {
            get {
                return _strMap;
            }
        }
        
        public override string XsltArgumentListContent {
            get {
                return _strArgList;
            }
        }
        
        public override string[] SourceSchemas {
            get {
                string[] _SrcSchemas = new string [1];
                _SrcSchemas[0] = @"SampleProject.RequestMsg";
                return _SrcSchemas;
            }
        }
        
        public override string[] TargetSchemas {
            get {
                string[] _TrgSchemas = new string [1];
                _TrgSchemas[0] = @"SampleProject.ResponseMsg";
                return _TrgSchemas;
            }
        }
    }
}
