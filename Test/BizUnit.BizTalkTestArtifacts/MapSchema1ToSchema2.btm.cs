namespace BizUnit.BizTalkTestArtifacts {
    
    
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizUnit.BizTalkTestArtifacts.Schema1", typeof(global::BizUnit.BizTalkTestArtifacts.Schema1))]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BizUnit.BizTalkTestArtifacts.Schema2", typeof(global::BizUnit.BizTalkTestArtifacts.Schema2))]
    public sealed class MapSchema1ToSchema2 : global::Microsoft.XLANGs.BaseTypes.TransformBase {
        
        private const string _strMap = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:var=""http://schemas.microsoft.com/BizTalk/2003/var"" exclude-result-prefixes=""msxsl var s1 s0 ScriptNS0"" version=""1.0"" xmlns:ns0=""http://BizUnit.BizTalkTestArtifacts.Schema2"" xmlns:s1=""http://BizUnit.BizTalkTestArtifacts.Schema1"" xmlns:s0=""http://BizUnit.BizTalkTestArtifacts.Schema0"" xmlns:ScriptNS0=""http://schemas.microsoft.com/BizTalk/2003/ScriptNS0"">
  <xsl:output omit-xml-declaration=""yes"" method=""xml"" version=""1.0"" />
  <xsl:template match=""/"">
    <xsl:apply-templates select=""/s1:Schema1Root"" />
  </xsl:template>
  <xsl:template match=""/s1:Schema1Root"">
    <ns0:Schema2Root>
      <Child1>
        <xsl:if test=""s0:Child/@Child1Attribute1"">
          <xsl:attribute name=""Child1Attribute1"">
            <xsl:value-of select=""s0:Child/@Child1Attribute1"" />
          </xsl:attribute>
        </xsl:if>
        <xsl:variable name=""var:v1"" select=""ScriptNS0:convertString(string(s0:Child/@Child1Attribute2))"" />
        <xsl:attribute name=""Child1Attribute2"">
          <xsl:value-of select=""$var:v1"" />
        </xsl:attribute>
      </Child1>
      <Child2>
        <xsl:if test=""Child2/@Child2Attribute1"">
          <xsl:attribute name=""Child2Attribute1"">
            <xsl:value-of select=""Child2/@Child2Attribute1"" />
          </xsl:attribute>
        </xsl:if>
        <xsl:if test=""Child2/@Child2Attribute2"">
          <xsl:attribute name=""Child2Attribute2"">
            <xsl:value-of select=""Child2/@Child2Attribute2"" />
          </xsl:attribute>
        </xsl:if>
        <xsl:value-of select=""Child2/text()"" />
      </Child2>
    </ns0:Schema2Root>
  </xsl:template>
</xsl:stylesheet>";
        
        private const string _strArgList = @"<ExtensionObjects>
  <ExtensionObject Namespace=""http://schemas.microsoft.com/BizTalk/2003/ScriptNS0"" AssemblyName=""BizUnit.BizTalkTestArtifacts.Components, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8ab3cc29608bfce0"" ClassName=""BizUnit.BizTalkTestArtifacts.Components.StringMapper"" />
</ExtensionObjects>";
        
        private const string _strSrcSchemasList0 = @"BizUnit.BizTalkTestArtifacts.Schema1";
        
        private const string _strTrgSchemasList0 = @"BizUnit.BizTalkTestArtifacts.Schema2";
        
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
                _SrcSchemas[0] = @"BizUnit.BizTalkTestArtifacts.Schema1";
                return _SrcSchemas;
            }
        }
        
        public override string[] TargetSchemas {
            get {
                string[] _TrgSchemas = new string [1];
                _TrgSchemas[0] = @"BizUnit.BizTalkTestArtifacts.Schema2";
                return _TrgSchemas;
            }
        }
    }
}
