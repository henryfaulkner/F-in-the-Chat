using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace WebSocketServer
{
    public class Program
    {
        private static void TCPHandshake(TcpClient client, ref NetworkStream stream) 
        {
            //wait for enough bytes to be available
            while(client.Available < 3);
            Byte[] bytes = new Byte[client.Available];
            stream.Read(bytes, 0, bytes.Length);
            String data = Encoding.UTF8.GetString(bytes);
            if(Regex.IsMatch(data, "^GET")) {
                Console.WriteLine("Matched GET");
                /*
                1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                3. Compute SHA-1 and Base64 hash of the new value
                4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                */
                string swk = Regex.Match(data, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                const string eol = "\r\n"; 
                Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                    + "Connection: Upgrade" + eol
                    + "Upgrade: websocket" + eol
                    + "Sec-WebSocket-Accept: " + swkaSha1Base64 + eol
                    + eol);
                Console.WriteLine("response");
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(response));

                stream.Write(response, 0, response.Length);
            } else {
                Console.WriteLine("Not matched GET");
                bool fin = (bytes[0] & 0b10000000) != 0,
                    mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
                int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                    offset = 2;
                ulong msglen = (ulong) bytes[1] & 0b01111111;

                if (msglen == 126) {
                    // bytes are reversed because websocket will print them in Big-Endian, whereas
                    // BitConverter will want them arranged in little-endian on windows
                    msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                    offset = 4;
                } else if (msglen == 127) {
                    // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering
                    // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                    // websocket frame available through client.Available).
                    msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] },0);
                    offset = 10;
                }

                if (msglen == 0) {
                    Console.WriteLine("msglen == 0");
                } else if (mask) {
                    byte[] decoded = new byte[msglen];
                    byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                    offset += 4;

                    for (ulong i = 0; i < msglen; ++i)
                        decoded[i] = (byte)(bytes[((ulong)offset) + i] ^ masks[i % 4]);

                    string text = Encoding.UTF8.GetString(decoded);
                    Console.WriteLine("{0}", text);
                } else {
                    Console.WriteLine("mask bit not set");
                }
            }
        }

        private static void RunPython(string pathToPythonFile) {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/usr/bin/python3";
            start.Arguments = pathToPythonFile;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using(Process process = Process.Start(start)) {
                using(StreamReader reader = process.StandardOutput) {
                    Console.Write(reader.ReadToEnd());
                }
            } 
        }

        private static void RunPython(string pathToPythonFile, string args) {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/usr/bin/python3";
            start.Arguments = string.Format("{0} {1}", pathToPythonFile, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using(Process process = Process.Start(start)) {
                using(StreamReader reader = process.StandardOutput) {
                    Console.Write(reader.ReadToEnd());
                }
            } 
        }

        public static void Main(string[] args)
        {
            //server implementation
            TcpListener server = new TcpListener(IPAddress.Parse("{lan}"), 80);
            server.Start();
            Console.WriteLine("Server has started on {lan}:80. Waiting for a connection…\n");
            //triggers when a client connects
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("A client connected.");
            NetworkStream stream = client.GetStream();
            
            bool on = false;
            //enter to an infinite cycle to be able to handle every change in stream
            while(true) {
                //traps here until some bytes of data have been sent
                while(!stream.DataAvailable);
                TCPHandshake(client, ref stream);
                if(on) {
                    // absolute path of file
                    //RunPython("/Users/henryfaulkner/Desktop/Projects/F-in-the-Chat/server/python/off.py");
                    //Console.WriteLine("Turn Off.");
                    on = false;
                } else {
                    // absolute path of file
                    //RunPython("/Users/henryfaulkner/Desktop/Projects/F-in-the-Chat/server/python/rainbow.py");
                    //Console.WriteLine("Turn on.");
                    on = true;
                }
            }
        }
    }
}
