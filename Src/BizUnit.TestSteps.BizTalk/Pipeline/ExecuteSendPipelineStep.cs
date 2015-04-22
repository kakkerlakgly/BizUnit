//---------------------------------------------------------------------
// File: ExecuteSendPipelineStep.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using BizUnit.Common;
using BizUnit.TestSteps.BizTalk.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;
using Winterdom.BizTalk.PipelineTesting;

namespace BizUnit.TestSteps.BizTalk.Pipeline
{
    /// <summary>
    ///     The ExecuteReceivePipelineStep can be used to execute a pipeline and test the output from it
    /// </summary>
    /// <remarks>
    ///     The following shows an example of the Xml representation of this test step. The step
    ///     can perform a pipeline and output that to an output file
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Pipeline:assemblyPath</term>
    ///             <description>The assembly containing the BizTalk pipeline to execute.</description>
    ///         </listheader>
    ///         <item>
    ///             <term>Pipeline:typeName</term>
    ///             <description>The typename of the BizTalk pipeline to execute</description>
    ///         </item>
    ///         <item>
    ///             <term>DocSpecs:assembly</term>
    ///             <description>The assembly containing the BizTalk docspec schema assembly path (multiple)</description>
    ///         </item>
    ///         <item>
    ///             <term>DocSpecs:type</term>
    ///             <description>The assembly containing the BizTalk docspec schema type name (multiple)</description>
    ///         </item>
    ///         <item>
    ///             <term>SourceDir</term>
    ///             <description>The directory path for the source input file(s) to execute in the pipeline</description>
    ///         </item>
    ///         <item>
    ///             <term>SearchPattern</term>
    ///             <description>The search pattern for the input files. E.g. input*.xml</description>
    ///         </item>
    ///         <item>
    ///             <term>Destination</term>
    ///             <description>The destination to write the pipeline output file</description>
    ///         </item>
    ///         <item>
    ///             <term>InputContextDir</term>
    ///             <description>The directory path for the source context file(s) (optional)</description>
    ///         </item>
    ///         <item>
    ///             <term>InputContextSearchPattern</term>
    ///             <description>The search pattern for the source context file(s) (optional)</description>
    ///         </item>
    ///         <item>
    ///             <term>OutputContextFile</term>
    ///             <description>The location to write the output message context file (optional)</description>
    ///         </item>
    ///         <item>
    ///             <term>SourceEncoding</term>
    ///             <description>The charset to be written on the pipeline input message (optional)</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class ExecuteSendPipelineStep : TestStepBase
    {
        private Type[] _docSpecs;
        private IList<DocSpecDefinition> _docSpecsRawList = new List<DocSpecDefinition>();

        /// <summary>
        ///     Gets and sets the assembly path for the .NET type of the pipeline to be executed
        /// </summary>
        public string PipelineAssemblyPath { get; set; }

        /// <summary>
        ///     Gets and sets the type name for the .NET type of the pipeline to be executed
        /// </summary>
        public string PipelineTypeName { get; set; }

        /// <summary>
        ///     Gets and sets the docspecs for the pipeline to be executed. Pairs of typeName, assemblyPath.
        /// </summary>
        public IList<DocSpecDefinition> DocSpecs
        {
            get { return _docSpecsRawList; }
            private set { _docSpecsRawList = value; }
        }

        /// <summary>
        ///     Gets and sets the pipeline instance configuration for the pipeline to be executed
        /// </summary>
        public string InstanceConfigFile { get; set; }

        /// <summary>
        ///     Gets and sets the source file path for the input file to the pipeline
        /// </summary>
        public string SourceDir { get; set; }

        /// <summary>
        ///     Gets and sets the source encoding
        /// </summary>
        public string SourceEncoding { get; set; }

        /// <summary>
        ///     Gets and sets the search pattern for the input file
        /// </summary>
        public string SearchPattern { get; set; }

        /// <summary>
        ///     Gets and sets the destination of the pipeline output
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        ///     Gets and sets the directory containing the message context file for the input message
        /// </summary>
        public string InputContextDir { get; set; }

        /// <summary>
        ///     Gets and sets the message context search pattern for the input message
        /// </summary>
        public string InputContextSearchPattern { get; set; }

        /// <summary>
        ///     Gets and sets the file name for the message context for the output message
        /// </summary>
        public string OutputContextFile { get; set; }

        /// <summary>
        ///     TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            if (_docSpecsRawList.Count > 0)
            {
                var ds = new List<Type>(_docSpecsRawList.Count);
                foreach (var docSpec in _docSpecsRawList)
                {
                    var ass = AssemblyHelper.LoadAssembly(docSpec.AssemblyPath);
                    context.LogInfo("Loading DocumentSpec {0} from location {1}.", docSpec.TypeName, ass.Location);
                    var type = ass.GetType(docSpec.TypeName);

                    ds.Add(type);
                }
                _docSpecs = ds.ToArray();
            }

            context.LogInfo("Loading pipeline {0} from location {1}.", PipelineTypeName, PipelineAssemblyPath);
            var pipelineType = ObjectCreator.GetType(PipelineTypeName, PipelineAssemblyPath);

            var pipelineWrapper = PipelineFactory.CreateSendPipeline(pipelineType);
            if (!string.IsNullOrEmpty(InstanceConfigFile))
            {
                pipelineWrapper.ApplyInstanceConfig(InstanceConfigFile);
            }

            if (null != _docSpecs)
            {
                foreach (var docSpec in _docSpecs)
                {
                    pipelineWrapper.AddDocSpec(docSpec);
                }
            }

            var mc = new MessageCollection();

            FileInfo[] contexts = null;
            if (!string.IsNullOrEmpty(InputContextDir) && !string.IsNullOrEmpty(InputContextSearchPattern))
            {
                var cdi = new DirectoryInfo(InputContextDir);
                contexts = cdi.GetFiles(InputContextSearchPattern);
            }

            var di = new DirectoryInfo(SourceDir);
            var index = 0;
            foreach (var fi in di.GetFiles(SearchPattern))
            {
                Stream stream = StreamHelper.LoadFileToStream(fi.FullName);
                var inputMessage = MessageHelper.CreateFromStream(stream);

                if (!string.IsNullOrEmpty(SourceEncoding))
                {
                    inputMessage.BodyPart.Charset = SourceEncoding;
                }

                // Load context file, add to message context.
                if ((null != contexts) && (contexts.Length > index))
                {
                    var cf = contexts[index].FullName;
                    if (System.IO.File.Exists(cf))
                    {
                        var mi = MessageInfo.Deserialize(cf);
                        mi.MergeIntoMessage(inputMessage);
                    }
                }

                mc.Add(inputMessage);
                index++;
            }

            var outputMsg = pipelineWrapper.Execute(mc);
            PersistMessageHelper.PersistMessage(outputMsg, Destination);

            if (!string.IsNullOrEmpty(OutputContextFile))
            {
                var omi = BizTalkMessageInfoFactory.CreateMessageInfo(outputMsg, Destination);
                MessageInfo.Serialize(omi, OutputContextFile);
            }
        }

        /// <summary>
        ///     TestStepBase.Validate() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(PipelineTypeName, "pipelineTypeName");
            // pipelineAssemblyPath - optional

            Destination = context.SubstituteWildCards(Destination);
        }
    }
}