//---------------------------------------------------------------------
// File: Pop3Parse.cs
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
    ///     Summary description for Pop3ParseMessage.
    /// </summary>
    internal class Pop3Parse
    {
        // Mapping to lineSubTypeString ...
        internal const int ContentTypeType = 0;
        internal const int ContentTransferEncodingType = 1;
        internal const int ContentDescriptionType = 2;
        internal const int ContentDispositionType = 3;
        // Mapping to nextLineTypeString ...
        internal const int NameType = 0;
        internal const int FilenameType = 1;
        // Non-string mappers ...
        internal const int UnknownType = -99;
        internal const int EndOfHeader = -98;
        internal const int MultipartBoundaryFound = -97;
        internal const int ComponetsDone = -96;

        private static readonly string[] _lineUpperTypeString =
        {
            "From",
            "To",
            "Subject",
            "Content-Type"
        };

        private static readonly string[] _lineSubTypeString =
        {
            "Content-Type",
            "Content-Transfer-Encoding",
            "Content-Description",
            "Content-Disposition"
        };

        private static readonly string[] _nextLineTypeString =
        {
            "name",
            "filename"
        };

        internal static string[] LineUpperTypeString
        {
            get { return _lineUpperTypeString; }
        }

        internal static string[] LineSubTypeString
        {
            get { return _lineSubTypeString; }
        }

        internal static string[] NextLineTypeString
        {
            get { return _nextLineTypeString; }
        }

        internal static string From(string line)
        {
            return
                Regex.Replace(line
                    , @"^From:.*[ |<]([a-z|A-Z|0-9|\.|\-|_]+@[a-z|A-Z|0-9|\.|\-|_]+).*$"
                    , "$1");
        }

        internal static string Subject(string line)
        {
            var subject =
                Regex.Replace(line
                    , @"^Subject: (.*)$"
                    , "$1");
            var match = Regex.Match(subject, @"=\?(?<charset>[^\?]+)\?(?<encoding>[BQ])\?(?<data>[^\?]+)\?=");
            if (match.Success)
            {
                if (match.Groups["encoding"].Value == "B")
                {
                    var data = Convert.FromBase64String(match.Groups["data"].Value);
                    subject = Encoding.ASCII.GetString(data);
                }
                else
                {
                    subject = Pop3Statics.FromQuotedPrintable(match.Groups["data"].Value);
                }
            }
            return subject;
        }

        internal static string To(string line)
        {
            return
                Regex.Replace(line
                    , @"^To:.*[ |<]([a-z|A-Z|0-9|\.|\-|_]+@[a-z|A-Z|0-9|\.|\-|_]+).*$"
                    , "$1");
        }

        internal static string ContentType(string line)
        {
            return
                Regex.Replace(line
                    , @"^Content-Type: (.*)$"
                    , "$1");
        }

        internal static string ContentTransferEncoding(string line)
        {
            return
                Regex.Replace(line
                    , @"^Content-Transfer-Encoding: (.*)$"
                    , "$1");
        }

        internal static string ContentDescription(string line)
        {
            return
                Regex.Replace(line
                    , @"^Content-Description: (.*)$"
                    , "$1");
        }

        internal static string ContentDisposition(string line)
        {
            return
                Regex.Replace(line
                    , @"^Content-Disposition: (.*)$"
                    , "$1");
        }

        internal static bool IsMultipart(string line)
        {
            return
                Regex.Match(line, "^multipart/.*").Success;
        }

        internal static string MultipartBoundary(string line)
        {
            return
                Regex.Replace(line
                    , "^.*boundary=[\"]*([^\"]*).*$"
                    , "$1");
        }

        internal static string Name(string line)
        {
            return Regex.Replace(line,
                "^[ |	]+name=[\"]*([^\"]*).*$", "$1");
        }

        internal static string Filename(string line)
        {
            return Regex.Replace(line,
                "^[ |	]+filename=[\"]*([^\"]*).*$", "$1");
        }

        internal static int GetSubHeaderNextLineType(string line)
        {
            var lineType = UnknownType;

            for (var i = 0; i < NextLineTypeString.Length; i++)
            {
                var match = NextLineTypeString[i];

                if (Regex.Match(line, "^[ |	]+" + match + "=" + ".*$").Success)
                {
                    lineType = i;
                    break;
                }
                if (line.Length == 0)
                {
                    lineType = EndOfHeader;
                    break;
                }
            }

            return lineType;
        }

        internal static int GetSubHeaderLineType(string line)
        {
            var lineType = UnknownType;

            for (var i = 0; i < LineSubTypeString.Length; i++)
            {
                var match = LineSubTypeString[i];

                if (Regex.Match(line, "^" + match + ":" + ".*$").Success)
                {
                    lineType = i;
                    break;
                }
                if (line.Length == 0)
                {
                    lineType = EndOfHeader;
                    break;
                }
            }

            return lineType;
        }

        internal static int GetSubHeaderLineType(string line, string boundary)
        {
            var lineType = UnknownType;

            for (var i = 0; i < LineSubTypeString.Length; i++)
            {
                var match = LineSubTypeString[i];

                if (Regex.Match(line, "^" + match + ":" + ".*$").Success)
                {
                    lineType = i;
                    break;
                }

                if (line.Equals("--" + boundary))
                {
                    lineType = MultipartBoundaryFound;
                    break;
                }

                if (line.Equals("--" + boundary + "--"))
                {
                    lineType = ComponetsDone;
                    break;
                }

                if (line.Length == 0)
                {
                    lineType = EndOfHeader;
                    break;
                }
            }

            return lineType;
        }
    }
}