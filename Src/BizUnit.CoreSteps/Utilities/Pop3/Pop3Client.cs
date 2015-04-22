//---------------------------------------------------------------------
// File: Pop3Client.cs
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace BizUnit.CoreSteps.Utilities.Pop3
{
    internal class Pop3Client : IEnumerable<Pop3Component>
    {
        private const int Pop3Port = 110;
        private const int MaxBufferReadSize = 256;
        private long _directPosition = -1;
        private long _inboxPosition;
        private Pop3Message _pop3Message;
        private Socket _socket;

        internal Pop3Client(string user, string pass, string server)
        {
            UserDetails = new Pop3Credential(user, pass, server);
        }

        internal Pop3Credential UserDetails { set; get; }

        internal string From
        {
            get { return _pop3Message.From; }
        }

        internal string To
        {
            get { return _pop3Message.To; }
        }

        internal string Subject
        {
            get { return _pop3Message.Subject; }
        }

        internal string Body
        {
            get { return _pop3Message.Body; }
        }

        internal bool IsMultipart
        {
            get { return _pop3Message.IsMultipart; }
        }

        internal long MessageCount
        {
            get
            {
                long count = 0;

                if (_socket == null)
                {
                    throw new Pop3MessageException("Pop3 server not connected");
                }

                Send("stat");

                var returned = GetPop3String();

                // if values returned ...
                if (Regex.Match(returned,
                    @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$").Success)
                {
                    // get number of emails ...
                    count = long.Parse(Regex
                        .Replace(returned.Replace("\r\n", "")
                            , @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$", "$1"));
                }

                return (count);
            }
        }

        public IEnumerator<Pop3Component> GetEnumerator()
        {
            return _pop3Message.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Socket GetClientSocket()
        {
            try
            {
                // Get host related information.
                var hostEntry = Dns.GetHostEntry(UserDetails.Server);

                // Loop through the AddressList to obtain the supported 
                // AddressFamily. This is to avoid an exception that 
                // occurs when the host IP Address is not compatible 
                // with the address family 
                // (typical in the IPv6 case).

                foreach (var address in hostEntry.AddressList)
                {
                    var ipe = new IPEndPoint(address, Pop3Port);

                    Socket tempSocket = null;
                    try
                    {
                        tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        tempSocket.Connect(ipe);

                        if (tempSocket.Connected)
                        {
                            // we have a connection.
                            // return this socket ...
                            return tempSocket;
                        }
                        tempSocket.Dispose();
                    }
                    catch
                    {
                        if (tempSocket != null) tempSocket.Dispose();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Pop3ConnectException(e.ToString());
            }

            // throw exception if can't connect ...
            throw new Pop3ConnectException("Error : connecting to "
                                           + UserDetails.Server);
        }

        //send the data to server
        private void Send(string data)
        {
            if (_socket == null)
            {
                throw new Pop3MessageException("Pop3 connection is closed");
            }

            try
            {
                // Convert the string data to byte data 
                // using ASCII encoding.

                var byteData = Encoding.ASCII.GetBytes(data + "\r\n");

                // Begin sending the data to the remote device.
                _socket.Send(byteData);
            }
            catch (Exception e)
            {
                throw new Pop3SendException(e.ToString());
            }
        }

        private string GetPop3String()
        {
            if (_socket == null)
            {
                throw new
                    Pop3MessageException("Connection to POP3 server is closed");
            }

            var buffer = new byte[MaxBufferReadSize];
            string line;

            try
            {
                var byteCount =
                    _socket.Receive(buffer, buffer.Length, 0);

                line =
                    Encoding.ASCII.GetString(buffer, 0, byteCount);
            }
            catch (Exception e)
            {
                throw new Pop3ReceiveException(e.ToString());
            }

            return line;
        }

        private void LoginToInbox()
        {
            // send username ...
            Send("user " + UserDetails.User);

            // get response ...
            var returned = GetPop3String();

            if (!returned.Substring(0, 3).Equals("+OK"))
            {
                throw new Pop3LoginException("login not excepted");
            }

            // send password ...
            Send("pass " + UserDetails.Pass);

            // get response ...
            returned = GetPop3String();

            if (!returned.Substring(0, 3).Equals("+OK"))
            {
                throw new
                    Pop3LoginException("login/password not accepted");
            }
        }

        internal void CloseConnection()
        {
            Send("quit");

            _socket = null;
            _pop3Message = null;
        }

        internal bool DeleteEmail()
        {
            var ret = false;

            Send("dele " + _inboxPosition);

            var returned = GetPop3String();

            if (Regex.Match(returned,
                @"^.*\+OK.*$").Success)
            {
                ret = true;
            }

            return ret;
        }

        internal bool NextEmail(long directPosition)
        {
            bool ret;

            if (directPosition >= 0)
            {
                _directPosition = directPosition;
                ret = NextEmail();
            }
            else
            {
                throw new Pop3MessageException("Position less than zero");
            }

            return ret;
        }

        internal bool NextEmail()
        {
            long pos;

            if (_directPosition == -1)
            {
                if (_inboxPosition == 0)
                {
                    pos = 1;
                }
                else
                {
                    pos = _inboxPosition + 1;
                }
            }
            else
            {
                pos = _directPosition + 1;
                _directPosition = -1;
            }

            // send username ...
            Send("list " + pos);

            // get response ...
            var returned = GetPop3String();

            // if email does not exist at this position
            // then return false ...

            if (returned.Substring(0, 4).ToUpper().Equals("-ERR"))
            {
                return false;
            }

            _inboxPosition = pos;

            // strip out CRLF ...
            var noCr = returned.Split('\r');

            // get size ...
            var elements = noCr[0].Split(' ');

            var size = long.Parse(elements[2]);

            // ... else read email data
            _pop3Message = new Pop3Message(_inboxPosition, size, _socket);

            return true;
        }

        internal void OpenInbox()
        {
            // get a socket ...
            _socket = GetClientSocket();

            // get initial header from POP3 server ...
            var header = GetPop3String();

            if (!header.Substring(0, 3).Equals("+OK"))
            {
                throw new InvalidOperationException("Invalid initial POP3 response");
            }

            // send login details ...
            LoginToInbox();
        }
    }
}