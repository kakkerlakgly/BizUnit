//---------------------------------------------------------------------
// File: ExecuteReceivePipelineStep.cs
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
    ///             <term>Source</term>
    ///             <description>The file path of the source input file to execute in the pipeline</description>
    ///         </item>
    ///         <item>
    ///             <term>Source:Encoding</term>
    ///             <description>The excoding type of the input file to execute in the pipeline</description>
    ///         </item>
    ///         <item>
    ///             <term>DestinationDir</term>
    ///             <description>The destination directory to write the pipeline output file(s)</description>
    ///         </item>
    ///         <item>
    ///             <term>DestinationFileFormat</term>
    ///             <description>The file format of the output file(s)</description>
    ///         </item>
    ///         <item>
    ///             <term>ContextFileFormat</term>
    ///             <description>The file format of the output message context file(s)</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class ExecuteReceivePipelineStep : TestStepBase
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
        public string Source { get; set; }

        /// <summary>
        ///     Gets and sets the source encoding
        /// </summary>
        public string SourceEncoding { get; set; }

        /// <summary>
        ///     Gets and sets the destination of the pipeline output
        /// </summary>
        public string DestinationDir { get; set; }

        /// <summary>
        ///     Gets and sets the destination file format
        /// </summary>
        public string DestinationFileFormat { get; set; }

        /// <summary>
        ///     Gets and sets the message context for the input message
        /// </summary>
        public string InputContextFile { get; set; }

        /// <summary>
        ///     Gets and sets the message context file format for the output message
        /// </summary>
        public string OutputContextFileFormat { get; set; }

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

            var pipelineWrapper = PipelineFactory.CreateReceivePipeline(pipelineType);

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

            MessageCollection mc = null;
            using (Stream stream = new FileStream(Source, FileMode.Open, FileAccess.Read))
            {
                var inputMessage = MessageHelper.CreateFromStream(stream);
                if (!string.IsNullOrEmpty(SourceEncoding))
                {
                    inputMessage.BodyPart.Charset = SourceEncoding;
                }

                // Load context file, add to message context.
                if (!string.IsNullOrEmpty(InputContextFile) && new FileInfo(InputContextFile).Exists)
                {
                    var mi = MessageInfo.Deserialize(InputContextFile);
                    mi.MergeIntoMessage(inputMessage);
                }

                mc = pipelineWrapper.Execute(inputMessage);
            }

            for (var count = 0; count < mc.Count; count++)
            {
                string destination = null;
                if (!string.IsNullOrEmpty(DestinationFileFormat))
                {
                    destination = string.Format(DestinationFileFormat, count);
                    if (!string.IsNullOrEmpty(DestinationDir))
                    {
                        destination = Path.Combine(DestinationDir, destination);
                    }

                    PersistMessageHelper.PersistMessage(mc[count], destination);
                }

                if (!string.IsNullOrEmpty(OutputContextFileFormat))
                {
                    var contextDestination = string.Format(OutputContextFileFormat, count);
                    if (!string.IsNullOrEmpty(DestinationDir))
                    {
                        contextDestination = Path.Combine(DestinationDir, contextDestination);
                    }

                    var mi = BizTalkMessageInfoFactory.CreateMessageInfo(mc[count], destination);
                    MessageInfo.Serialize(mi, contextDestination);
                }
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

            Source = context.SubstituteWildCards(Source);
            if (!new FileInfo(Source).Exists)
            {
                throw new ArgumentException("Source file does not exist.", Source);
            }

            // destinationDir - optional
            if (!string.IsNullOrEmpty(DestinationDir))
            {
                DestinationDir = context.SubstituteWildCards(DestinationDir);
            }
        }
    }
}