//---------------------------------------------------------------------
// File: Pop3Component.cs
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
using System.Text;
using System.Text.RegularExpressions;

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    /// <summary>
    ///     Summary description for Pop3Attachment.
    /// </summary>
    internal class Pop3Component
    {
        private readonly string _contentDescription;
        private readonly string _contentDisposition;
        private readonly string _contentTransferEncoding;
        private readonly string _contentType;
        private readonly string _filename;
        private readonly string _name;
        internal byte[] BinaryData;

        internal Pop3Component(string contentType, string contentTransferEncoding, string data)
        {
            _contentTransferEncoding = contentTransferEncoding;
            _contentType = contentType;
            Data = data;

            Data = Data.Substring(0, Data.Length - 2);
            DecodeData();
        }

        internal Pop3Component(string contentType, string name, string filename,
            string contentTransferEncoding, string contentDescription,
            string contentDisposition, string data)
        {
            _contentType = contentType;
            _name = name;
            _filename = filename;
            _contentTransferEncoding = contentTransferEncoding;
            _contentDescription = contentDescription;
            _contentDisposition = contentDisposition;
            Data = data;

            DecodeData();
        }

        internal string FileExtension
        {
            get
            {
                string extension = null;

                // if file has a filename and the filename
                // has an extension ...

                if ((_filename != null) &&
                    Regex.Match(_filename, @"^.*\..*$").Success)
                {
                    // get extension ...
                    extension =
                        Regex.Replace(_name, @"^[^\.]*\.([^\.]+)$", "$1");
                }

                // NOTE: return null if extension
                // not found ...
                return extension;
            }
        }

        internal string FileNoExtension
        {
            get
            {
                // if file has a filename and the filename
                // has an extension ...

                if ((_filename != null) &&
                    Regex.Match(_filename, @"^.*\..*$").Success)
                {
                    // get extension ...
                    return
                        Regex.Replace(_name, @"^([^\.]*)\.[^\.]+$", "$1");
                }

                // NOTE: return null if extension
                // not found ...
                return null;
            }
        }

        internal string Filename
        {
            get { return _filename; }
        }

        internal string ContentType
        {
            get { return _contentType; }
        }

        internal string Name
        {
            get { return _name; }
        }

        internal string ContentTransferEncoding
        {
            get { return _contentTransferEncoding; }
        }

        internal string ContentDescription
        {
            get { return _contentDescription; }
        }

        internal string ContentDisposition
        {
            get { return _contentDisposition; }
        }

        internal string Data { get; private set; }

        internal bool IsBody
        {
            get
            {
                return
                    _contentDisposition == null;
            }
        }

        internal bool IsAttachment
        {
            get
            {
                if (_contentDisposition != null)
                {
                    return
                        Regex
                            .Match(_contentDisposition,
                                "^attachment.*$")
                            .Success;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return
                "Content-Type: " + _contentType + "\r\n" +
                "Name: " + _name + "\r\n" +
                "Filename: " + _filename + "\r\n" +
                "Content-Transfer-Encoding: " + _contentTransferEncoding + "\r\n" +
                "Content-Description: " + _contentDescription + "\r\n" +
                "Content-Disposition: " + _contentDisposition + "\r\n" +
                "Data :" + Data;
        }

        private void DecodeData()
        {
            // if this data is an attachment ...
            // if BASE-64 data ...
            if (_contentTransferEncoding != null)
            {
                if (_contentTransferEncoding.ToUpper()
                    .Equals("BASE64"))
                {
                    // convert attachment from BASE64 ...
                    BinaryData =
                        Convert.FromBase64String(Data.Replace("\n", ""));

                    Data = Encoding.ASCII.GetString(BinaryData);
                }
                else
                // if PRINTABLE ...
                    if (
                        _contentTransferEncoding.ToUpper()
                            .Equals("QUOTED-PRINTABLE"))
                    {
                        Data = Pop3Statics.FromQuotedPrintable(Data);
                    }
            }
        }
    }
}