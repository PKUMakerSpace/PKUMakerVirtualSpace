using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Newtonsoft.Json;

namespace PKU.Connection
{
    enum ConnectionStatus
    {
        Accept, Refuse
    }

    enum SyncProtocol
    {
        Netcode
    }

    struct ConnectionRequest
    {
        public string UserEmail { get; set; }
        public string WorldID { get; set; }
        public SyncProtocol Protocol { get; set; }
        public byte[] Data { get; set; }
    }

    struct ConnectionResponse
    {
        public ConnectionStatus ConnectionStatus { get; set; }
        public byte[] Data { get; set; }
    }

    public struct NetcodeServerInfo
    {
        public string IPAddress { get; set; }
        public short Port { get; set; }
    }


    public class HttpConnectionManager
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async static Task<NetcodeServerInfo> RequestForConnection(string URL, string userEmail, string worldID)
        {
            var connectionRequest = new ConnectionRequest()
            {
                UserEmail = userEmail,
                WorldID = worldID,
                Protocol = SyncProtocol.Netcode,
                Data = new byte[] { }
            };
            var content = new StringContent(JsonConvert.SerializeObject(connectionRequest));

            var httpResponseMessage = await httpClient.PostAsync(URL, content).ConfigureAwait(false);
            Debug.Log("status code: " + httpResponseMessage.StatusCode);

            if(httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception();
            }

            string response = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var connectionResponse = JsonConvert.DeserializeObject<ConnectionResponse>(response);
            
            if(connectionResponse.ConnectionStatus != ConnectionStatus.Accept)
            {
                throw new Exception();
            }

            var netcodeServerInfo = JsonConvert.DeserializeObject<NetcodeServerInfo>(Encoding.UTF8.GetString(connectionResponse.Data));

            return netcodeServerInfo;

        }

        public static void ConnectionHandler(string[] prefixes, NetcodeServerInfo netcodeServerInfo)
        {
            if (!HttpListener.IsSupported)
            {
                return;
            }

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                HttpListenerResponse response = context.Response;

                var netcodeInfo = JsonConvert.SerializeObject(netcodeServerInfo);
                var data = Encoding.UTF8.GetBytes(netcodeInfo);

                var connectionResponse = new ConnectionResponse();
                connectionResponse.Data = data;
                connectionResponse.ConnectionStatus = ConnectionStatus.Accept;

                string responseString = JsonConvert.SerializeObject(connectionResponse);
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            
            listener.Stop();
        }
    }
}
