//---------------------------------------------------------------------
// File: Pop3MessageComponents.cs
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

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    /// <summary>
    ///     Summary description for Pop3MessageBody.
    /// </summary>
    internal class Pop3MessageComponents : IEnumerable<Pop3Component>
    {
        private readonly IList<Pop3Component> _component = new List<Pop3Component>();

        internal Pop3MessageComponents(string[] lines, int startOfBody, string multipartBoundary, string mainContentType)
        {
            var stopOfBody = lines.Length;

            // if this email is a mixture of message
            // and attachments ...

            if (multipartBoundary == null)
            {
                string contentTransferEncoding = null;

                var sbText = new StringBuilder();
                for (long i = 0; i < startOfBody; i++)
                {
                    var line = lines[i].Replace("\n", "").Replace("\r", "");
                    var lineType = Pop3Parse.GetSubHeaderLineType(line);

                    switch (lineType)
                    {
                        case Pop3Parse.ContentTypeType:
                            Pop3Parse.ContentType(line);
                            break;

                        case Pop3Parse.ContentTransferEncodingType:
                            contentTransferEncoding = Pop3Parse.ContentTransferEncoding(line);
                            break;

                        case Pop3Parse.ContentDispositionType:
                            Pop3Parse.ContentDisposition(line);
                            break;

                        case Pop3Parse.ContentDescriptionType:
                            Pop3Parse.ContentDescription(line);
                            break;
                    }
                }

                for (var i = startOfBody; i < stopOfBody; i++)
                {
                    sbText.Append(lines[i]); //.Replace("\n","").Replace("\r",""));
                }

                // create a new component ...
                _component.Add(new Pop3Component(mainContentType, contentTransferEncoding, sbText.ToString()));
            }
            else
            {
                var boundary = multipartBoundary;

                var firstComponent = true;

                // loop through whole of email ...
                for (var i = startOfBody; i < stopOfBody;)
                {
                    var boundaryFound = true;

                    string contentType = null;
                    string name = null;
                    string filename = null;
                    string contentTransferEncoding = null;
                    string contentDescription = null;
                    string contentDisposition = null;
                    string data = null;

                    // if first block of multipart data ...
                    if (firstComponent)
                    {
                        boundaryFound = false;
                        firstComponent = false;

                        while (i < stopOfBody)
                        {
                            var line = lines[i].Replace("\n", "").Replace("\r", "");

                            // if multipart boundary found then
                            // exit loop ...

                            if (Pop3Parse.GetSubHeaderLineType(line, boundary) == Pop3Parse.MultipartBoundaryFound)
                            {
                                boundaryFound = true;
                                ++i;
                                break;
                            }
                            // ... else read next line ...

                            ++i;
                        }
                    }

                    // check to see whether multipart boundary
                    // was found ...

                    if (!boundaryFound)
                    {
                        throw new Pop3MissingBoundaryException("Missing multipart boundary: " + boundary);
                    }

                    var endOfHeader = false;

                    // read header information ...
                    while ((i < stopOfBody))
                    {
                        var line = lines[i].Replace("\n", "").Replace("\r", "");

                        var lineType = Pop3Parse.GetSubHeaderLineType(line, boundary);

                        switch (lineType)
                        {
                            case Pop3Parse.ContentTypeType:
                                contentType = Pop3Parse.ContentType(line);
                                break;

                            case Pop3Parse.ContentTransferEncodingType:
                                contentTransferEncoding = Pop3Parse.ContentTransferEncoding(line);
                                break;

                            case Pop3Parse.ContentDispositionType:
                                contentDisposition = Pop3Parse.ContentDisposition(line);
                                break;

                            case Pop3Parse.ContentDescriptionType:
                                contentDescription = Pop3Parse.ContentDescription(line);
                                break;

                            case Pop3Parse.EndOfHeader:
                                endOfHeader = true;
                                break;
                        }

                        ++i;

                        if (endOfHeader)
                        {
                            break;
                        }

                        while (i < stopOfBody)
                        {
                            // if more lines to read for this line ...
                            if (line.Substring(line.Length - 1, 1).Equals(";"))
                            {
                                var nextLine = lines[i].Replace("\r", "").Replace("\n", "");

                                switch (Pop3Parse.GetSubHeaderNextLineType(nextLine))
                                {
                                    case Pop3Parse.NameType:
                                        name = Pop3Parse.Name(nextLine);
                                        break;

                                    case Pop3Parse.FilenameType:
                                        filename = Pop3Parse.Filename(nextLine);
                                        break;

                                    case Pop3Parse.EndOfHeader:
                                        endOfHeader = true;
                                        break;
                                }

                                if (!endOfHeader)
                                {
                                    // point line to current line ...
                                    line = nextLine;
                                    ++i;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    var sbText = new StringBuilder();
                    var emailComposed = false;

                    // store the actual data ...
                    while (i < stopOfBody)
                    {
                        // get the next line ...
                        var line = lines[i].Replace("\n", "");

                        // if we've found the boundary ...
                        if (Pop3Parse.GetSubHeaderLineType(line, boundary) == Pop3Parse.MultipartBoundaryFound)
                        {
                            ++i;
                            break;
                        }

                        if (Pop3Parse.GetSubHeaderLineType(line, boundary) == Pop3Parse.ComponetsDone)
                        {
                            emailComposed = true;
                            break;
                        }

                        // add this line to data ...
                        sbText.Append(lines[i]);
                        ++i;
                    }

                    if (sbText.Length > 0)
                    {
                        data = sbText.ToString();
                    }

                    // create a new component ...
                    _component.Add(new Pop3Component(contentType, name, filename, contentTransferEncoding,
                        contentDescription, contentDisposition, data));

                    // if all multiparts have been
                    // composed then exit ..

                    if (emailComposed)
                    {
                        break;
                    }
                }
            }
        }

        internal int NumberOfComponents
        {
            get { return _component.Count; }
        }

        public IEnumerator<Pop3Component> GetEnumerator()
        {
            return _component.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}