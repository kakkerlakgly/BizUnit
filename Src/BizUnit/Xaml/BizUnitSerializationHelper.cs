﻿//---------------------------------------------------------------------
// File: BizUnitSerializationHelper.cs
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

using System.IO;
using System.Xaml;

namespace BizUnit.Xaml
{
    internal static class BizUnitSerializationHelper
    {
        private const int FileBufferSize = 4048;

        internal static string Serialize(object toSerialize)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(toSerialize, ms);
                using (var reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        internal static object Deserialize(string xamlText)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    sw.Write(xamlText);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    return Deserialize(ms);
                }
            }
        }

        internal static void SaveToFile(object toSerialize, string filePath)
        {
            using (var ms = new MemoryStream())
            {
                var buff = new byte[FileBufferSize];
                Serialize(toSerialize, ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                using (var fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    var read = ms.Read(buff, 0, buff.Length);
                    do
                    {
                        fs.Write(buff, 0, read);
                        read = ms.Read(buff, 0, buff.Length);
                    } while (read > 0);

                    fs.Flush();
                }
            }
        }

        private static void Serialize(object toSerialize, Stream stream)
        {
            XamlServices.Save(stream, toSerialize);
            stream.Seek(0, SeekOrigin.Begin);
        }

        private static object Deserialize(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return XamlServices.Load(stream);
        }
    }
}