namespace BizUnit.BizTalkTestArtifacts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class SendPipeline3 : Microsoft.BizTalk.PipelineOM.SendPipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>8c6b051c-0ff5-4fc2-9ae5-5016cb726282</CategoryId>  <FriendlyName>Transmit</FriendlyName"+
">  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Pre-Assemble\" minO"+
"ccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4101-4cce-4536-83fa-4a5040674ad6\" />      <Co"+
"mponents />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\"Assemb"+
"le\" minOccurs=\"0\" maxOccurs=\"1\" execMethod=\"All\" stageId=\"9d0e4107-4cce-4536-83fa-4a5040674ad6\" />  "+
"    <Components>        <Component>          <Name>Microsoft.BizTalk.Component.XmlAsmComp,Microsoft."+
"BizTalk.Pipeline.Components, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35</Name"+
">          <ComponentName>XML assembler</ComponentName>          <Description>XML assembler componen"+
"t.</Description>          <Version>1.0</Version>          <Properties>            <Property Name=\"En"+
"velopeDocSpecNames\">              <Value xsi:type=\"xsd:string\">BizUnit.BizTalkTestArtifacts.Schema3E"+
"nv</Value>            </Property>            <Property Name=\"EnvelopeSpecTargetNamespaces\">         "+
"     <Value xsi:type=\"xsd:string\">http://BizUnit.BizTalkTestArtifacts.Schema3Env</Value>            "+
"</Property>            <Property Name=\"DocumentSpecNames\">              <Value xsi:type=\"xsd:string\""+
">BizUnit.BizTalkTestArtifacts.Schema0</Value>            </Property>            <Property Name=\"Docu"+
"mentSpecTargetNamespaces\">              <Value xsi:type=\"xsd:string\">http://BizUnit.BizTalkTestArtif"+
"acts.Schema0</Value>            </Property>            <Property Name=\"XmlAsmProcessingInstructions\""+
" />            <Property Name=\"ProcessingInstructionsOptions\">              <Value xsi:type=\"xsd:int"+
"\">0</Value>            </Property>            <Property Name=\"ProcessingInstructionsScope\">         "+
"     <Value xsi:type=\"xsd:int\">0</Value>            </Property>            <Property Name=\"AddXmlDec"+
"laration\">              <Value xsi:type=\"xsd:boolean\">true</Value>            </Property>           "+
" <Property Name=\"TargetCharset\">              <Value xsi:type=\"xsd:string\" />            </Property>"+
"            <Property Name=\"TargetCodePage\">              <Value xsi:type=\"xsd:int\">0</Value>       "+
"     </Property>            <Property Name=\"PreserveBom\">              <Value xsi:type=\"xsd:boolean\""+
">true</Value>            </Property>            <Property Name=\"HiddenProperties\">              <Val"+
"ue xsi:type=\"xsd:string\">EnvelopeSpecTargetNamespaces,DocumentSpecTargetNamespaces,TargetCodePage</V"+
"alue>            </Property>          </Properties>          <CachedDisplayName>XML assembler</Cache"+
"dDisplayName>          <CachedIsManaged>true</CachedIsManaged>        </Component>      </Components"+
">    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"3\" Name=\"Encode\" minOccur"+
"s=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4108-4cce-4536-83fa-4a5040674ad6\" />      <Compon"+
"ents />    </Stage>  </Stages></Document>";
        
        private const string _versionDependentGuid = "7c2538e2-ad05-400d-ac01-07006dfbbee0";
        
        public SendPipeline3()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e4107-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.all);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("Microsoft.BizTalk.Component.XmlAsmComp,Microsoft.BizTalk.Pipeline.Components, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"EnvelopeDocSpec"+
"Names\">      <Value xsi:type=\"xsd:string\">BizUnit.BizTalkTestArtifacts.Schema3Env</Value>    </Prope"+
"rty>    <Property Name=\"EnvelopeSpecTargetNamespaces\">      <Value xsi:type=\"xsd:string\">http://BizU"+
"nit.BizTalkTestArtifacts.Schema3Env</Value>    </Property>    <Property Name=\"DocumentSpecNames\">   "+
"   <Value xsi:type=\"xsd:string\">BizUnit.BizTalkTestArtifacts.Schema0</Value>    </Property>    <Prop"+
"erty Name=\"DocumentSpecTargetNamespaces\">      <Value xsi:type=\"xsd:string\">http://BizUnit.BizTalkTe"+
"stArtifacts.Schema0</Value>    </Property>    <Property Name=\"XmlAsmProcessingInstructions\" />    <P"+
"roperty Name=\"ProcessingInstructionsOptions\">      <Value xsi:type=\"xsd:int\">0</Value>    </Property"+
">    <Property Name=\"ProcessingInstructionsScope\">      <Value xsi:type=\"xsd:int\">0</Value>    </Pro"+
"perty>    <Property Name=\"AddXmlDeclaration\">      <Value xsi:type=\"xsd:boolean\">true</Value>    </P"+
"roperty>    <Property Name=\"TargetCharset\">      <Value xsi:type=\"xsd:string\" />    </Property>    <"+
"Property Name=\"TargetCodePage\">      <Value xsi:type=\"xsd:int\">0</Value>    </Property>    <Property"+
" Name=\"PreserveBom\">      <Value xsi:type=\"xsd:boolean\">true</Value>    </Property>    <Property Nam"+
"e=\"HiddenProperties\">      <Value xsi:type=\"xsd:string\">EnvelopeSpecTargetNamespaces,DocumentSpecTar"+
"getNamespaces,TargetCodePage</Value>    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp0XmlProperties);;
                ((IPersistPropertyBag)(comp0)).Load(pb, 0);
            }
            this.AddComponent(stage, comp0);
        }
        
        public override string XmlContent
        {
            get
            {
                return _strPipeline;
            }
        }
        
        public override System.Guid VersionDependentGuid
        {
            get
            {
                return new System.Guid(_versionDependentGuid);
            }
        }
    }
}
