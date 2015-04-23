//---------------------------------------------------------------------
// File: StreamHelper.cs
// 
// Summary: 
//
// Author: Kevin B. Smith
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
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace BizUnit.CoreSteps.Utilities
{
    /// <summary>
    ///     Helper class for stream opperations
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        ///     Performs a binary comparison between two streams
        /// </summary>
        /// <param name="s1">The 1st stream to compare aginst the 2nd</param>
        /// <param name="s2">The 2nd stream to compare aginst the 1st</param>
        public static void CompareStreams(Stream s1, Stream s2)
        {
            var buff1 = new byte[4096];
            var buff2 = new byte[4096];
            int read1;

            do
            {
                read1 = s1.Read(buff1, 0, 4096);
                var read2 = s2.Read(buff2, 0, 4096);

                if (read1 != read2)
                {
                    throw new InvalidOperationException("Streams do not contain identical data!");
                }

                if (0 == read1)
                {
                    break;
                }

                for (var c = 0; c < read1; c++)
                {
                    if (buff1[c] != buff2[c])
                    {
                        throw new InvalidOperationException("Streams do not contain identical data!");
                    }
                }
            } while (read1 > 0);
        }

        /// <summary>
        ///     Helper method to load a disc FILE into a MemoryStream
        /// </summary>
        /// <param name="filePath">The path to the FILE containing the data</param>
        /// <param name="timeout">The timeout afterwhich if the FILE is not found the method will fail</param>
        /// <returns>MemoryStream containing the data in the FILE</returns>
        public static Stream LoadFileToStream(string filePath, double timeout)
        {
            var now = DateTime.Now;

            do
            {
                try
                {
                    return LoadFileToStream(filePath);
                }
                catch (Exception)
                {
                    if (DateTime.Now < now.AddMilliseconds(timeout))
                    {
                        Thread.Sleep(500);
                    }
                }
            } while (DateTime.Now < now.AddMilliseconds(timeout));

            throw new InvalidOperationException(
                string.Format("The file: {0} was not found within the timeout period!", filePath));
        }

        /// <summary>
        ///     Helper method to load a disc FILE into a MemoryStream
        /// </summary>
        /// <param name="filePath">The path to the FILE containing the data</param>
        /// <returns>MemoryStream containing the data in the FILE</returns>
        public static MemoryStream LoadFileToStream(string filePath)
        {
            MemoryStream s = null;
            try
            {
                s = new MemoryStream();

                // Get the match data...
                using (var fs = File.OpenRead(filePath))
                {
                    var buff = new byte[1024];
                    var read = fs.Read(buff, 0, 1024);

                    while (0 < read)
                    {
                        s.Write(buff, 0, read);
                        read = fs.Read(buff, 0, 1024);
                    }
                }
                s.Flush();
                s.Seek(0, SeekOrigin.Begin);

                return s;
            }
            catch
            {
                if (s != null) s.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Helper method to write the data in a stream to the console
        /// </summary>
        /// <param name="description">The description text that will be written before the stream data</param>
        /// <param name="ms">Stream containing the data to write</param>
        /// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
        public static void WriteStreamToConsole(string description, Stream ms, Context context)
        {
            ms.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(ms);
            context.LogData(description, sr.ReadToEnd());
            ms.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        ///     Helper method to load a forward only stream into a seekable MemoryStream
        /// </summary>
        /// <param name="s">The forward only stream to read the data from</param>
        /// <returns>MemoryStream containg the data as read from s</returns>
        public static Stream LoadMemoryStream(Stream s)
        {
            Stream ms = null;
            try
            {
                ms = new MemoryStream();
                var buff = new byte[1024];
                var read = s.Read(buff, 0, 1024);

                while (0 < read)
                {
                    ms.Write(buff, 0, read);
                    read = s.Read(buff, 0, 1024);
                }
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                return ms;
            }
            catch
            {
                if (ms != null) ms.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Helper method to load a string into a MemoryStream
        /// </summary>
        /// <param name="s">The string containing the data that will be loaded into the stream</param>
        /// <returns>MemoryStream containg the data read from the string</returns>
        public static Stream LoadMemoryStream(string s)
        {
            var utf8 = Encoding.UTF8;
            var bytes = utf8.GetBytes(s);
            Stream ms = null;
            try
            {
                ms = new MemoryStream(bytes);

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                return ms;
            }
            catch
            {
                if (ms != null) ms.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Helper method to compare two Xml documents from streams
        /// </summary>
        /// <param name="s1">Stream containing the 1st Xml document</param>
        /// <param name="s2">Stream containing the 2nd Xml document</param>
        /// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
        public static void CompareXmlDocs(Stream s1, Stream s2, Context context)
        {
            var doc = new XmlDocument();
            doc.Load(new XmlTextReader(s1));
            var root = doc.DocumentElement;
            var data1 = root.OuterXml;

            doc = new XmlDocument();
            doc.Load(new XmlTextReader(s2));
            root = doc.DocumentElement;
            var data2 = root.OuterXml;

            context.LogInfo("About to compare the following Xml documents:\r\nDocument1: {0},\r\nDocument2: {1}", data1,
                data2);
            using (Stream stream1 = LoadMemoryStream(data1), stream2 = LoadMemoryStream(data2))
            {
                CompareStreams(stream1, stream2);
            }
        }

        /// <summary>
        ///     Helper method to encode a stream
        /// </summary>
        /// <param name="rawData">Stream containing data to be encoded</param>
        /// <param name="encoding">The encoding to be used for the data</param>
        /// <returns>Encoded MemoryStream</returns>
        public static Stream EncodeStream(Stream rawData, Encoding encoding)
        {
            rawData.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(rawData);
            var data = sr.ReadToEnd();
            var e = encoding;
            var bytes = e.GetBytes(data);
            Stream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                return stream;
            }
            catch
            {
                if (stream != null) stream.Dispose();
                throw;
            }
        }
    }
}