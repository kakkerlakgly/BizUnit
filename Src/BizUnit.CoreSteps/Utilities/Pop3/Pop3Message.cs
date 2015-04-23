//---------------------------------------------------------------------
// File: Pop3Message.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    /// <summary>
    ///     DLM: Stores the From:, To:, Subject:, body and attachments
    ///     within an email. Binary attachments are Base64-decoded
    /// </summary>
    internal class Pop3Message : IEnumerable<Pop3Component>, IDisposable
    {
        private const int FromState = 0;
        private const int ToState = 1;
        private const int SubjectState = 2;
        private const int ContentTypeState = 3;
        private const int NotKnownState = -99;
        private const int EndOfHeader = -98;
        private readonly string _body;
        private readonly Socket _client;
        private readonly long _inboxPosition;
        // this array corresponds with above
        // enumerator ...

        private readonly string[] _lineTypeString =
        {
            "From",
            "To",
            "Subject",
            "Content-Type"
        };

        private readonly ManualResetEvent _manualEvent = new ManualResetEvent(false);
        private string _contentType;
        private Pop3MessageComponents _messageComponents;
        private string _multipartBoundary;
        private Pop3StateObject _pop3State;

        internal Pop3Message(long position, long size, Socket client)
        {
            _inboxPosition = position;
            _client = client;

            _pop3State = new Pop3StateObject {WorkSocket = _client, Sb = new StringBuilder()};

            // load email ...
            LoadEmail();

            // get body (if it exists) ...
            foreach (var multipart in this)
            {
                if (multipart.IsBody)
                {
                    _body = multipart.Data;
                    break;
                }
            }
        }

        internal bool IsMultipart { get; private set; }
        internal string From { get; private set; }
        internal string To { get; private set; }
        internal string Subject { get; private set; }

        internal string Body
        {
            get { return _body; }
        }

        internal long InboxPosition
        {
            get { return _inboxPosition; }
        }

        public IEnumerator<Pop3Component> GetEnumerator()
        {
            return _messageComponents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //send the data to server
        private void Send(string data)
        {
            try
            {
                // Convert the string data to byte data 
                // using ASCII encoding.

                var byteData = Encoding.ASCII.GetBytes(data + "\r\n");

                // Begin sending the data to the remote device.
                _client.Send(byteData);
            }
            catch (Exception e)
            {
                throw new Pop3SendException(e.ToString());
            }
        }

        private void StartReceiveAgain(string data)
        {
            // receive more data if we expect more.
            // note: a literal "." (or more) followed by
            // "\r\n" in an email is prefixed with "." ...

            if (!data.EndsWith("\r\n.\r\n"))
            {
                _client.BeginReceive(_pop3State.Buffer, 0,
                    Pop3StateObject.BufferSize, 0,
                    ReceiveCallback,
                    _pop3State);
            }
            else
            {
                // stop receiving data ...
                _manualEvent.Set();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.

                var stateObj = (Pop3StateObject) ar.AsyncState;
                var client = stateObj.WorkSocket;

                // Read data from the remote device.
                var bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, 
                    // so store the data received so far.

                    stateObj.Sb.Append(
                        Encoding.ASCII.GetString(stateObj.Buffer
                            , 0, bytesRead));

                    // read more data from pop3 server ...
                    StartReceiveAgain(stateObj.Sb.ToString());
                }
            }
            catch (Exception e)
            {
                _manualEvent.Set();

                throw new
                    Pop3ReceiveException("RecieveCallback error" +
                                         e);
            }
        }

        private void StartReceive()
        {
            // start receiving data ...
            _client.BeginReceive(_pop3State.Buffer, 0,
                Pop3StateObject.BufferSize, 0,
                ReceiveCallback,
                _pop3State);

            // wait until no more data to be read ...
            _manualEvent.WaitOne();
        }

        private int GetHeaderLineType(string line)
        {
            for (var i = 0; i < _lineTypeString.Length; i++)
            {
                var match = _lineTypeString[i];

                if (Regex.Match(line, "^" + match + ":" + ".*$").Success)
                {
                    return i;
                }

                if (line.Length == 0)
                {
                    return EndOfHeader;
                }
            }

            return NotKnownState;
        }

        private int ParseHeader(string[] lines)
        {
            var bodyStart = 0;

            for (var i = 0; i < lines.Length; i++)
            {
                var currentLine = lines[i].Replace("\n", "");

                var lineType = GetHeaderLineType(currentLine);

                switch (lineType)
                {
                    // From:
                    case FromState:
                        From = Pop3Parse.From(currentLine);
                        break;

                    // Subject:
                    case SubjectState:
                        Subject = Pop3Parse.Subject(currentLine);
                        break;

                    // To:
                    case ToState:
                        To = Pop3Parse.To(currentLine);
                        break;

                    // Content-Type
                    case ContentTypeState:

                        _contentType =
                            Pop3Parse.ContentType(currentLine);

                        IsMultipart =
                            Pop3Parse.IsMultipart(_contentType);

                        if (IsMultipart)
                        {
                            // if boundary definition is on next
                            // line ...

                            if (_contentType
                                .Substring(_contentType.Length - 1, 1).
                                Equals(";"))
                            {
                                ++i;

                                _multipartBoundary
                                    = Pop3Parse.
                                        MultipartBoundary(lines[i].
                                            Replace("\n", ""));
                            }
                            else
                            {
                                // boundary definition is on same
                                // line as "Content-Type" ...

                                _multipartBoundary =
                                    Pop3Parse
                                        .MultipartBoundary(_contentType);
                            }
                        }

                        break;

                    case EndOfHeader:
                        bodyStart = i + 1;
                        break;
                }

                if (bodyStart > 0)
                {
                    break;
                }
            }

            return (bodyStart);
        }

        private void ParseEmail(string[] lines)
        {
            var startOfBody = ParseHeader(lines);

            _messageComponents =
                new Pop3MessageComponents(lines, startOfBody
                    , _multipartBoundary, _contentType);
        }

        private void LoadEmail()
        {
            // tell pop3 server we want to start reading
            // email (m_inboxPosition) from inbox ...

            Send("retr " + _inboxPosition);

            // start receiving email ...
            StartReceive();

            // parse email ...
            ParseEmail(
                _pop3State.Sb.ToString().Split('\r'));

            // remove reading pop3State ...
            _pop3State = null;
        }

        public override string ToString()
        {
            var ret =
                "From    : " + From + "\r\n" +
                "To      : " + To + "\r\n" +
                "Subject : " + Subject + "\r\n";

            return this.Aggregate(ret, (current, enumerator) => current + (enumerator + "\r\n"));
        }

        public void Dispose()
        {
            if (_manualEvent != null) _manualEvent.Dispose();
            if (_client != null) _client.Dispose();
        }
    }
}