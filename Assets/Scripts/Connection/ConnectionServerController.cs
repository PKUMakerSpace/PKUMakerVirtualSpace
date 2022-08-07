using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace PKU.Connection
{
    class ConnectionServerController : NetworkBehaviour
    {
        [SerializeField]
        private string netcodeServerIPAddress = "127.0.0.1";

        [SerializeField]
        private short port = 7777;

        public void Start()
        {
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer || IsHost)
            {
                Debug.Log("start http server");
                ConnectionHandler();
            }
        }

        public void ConnectionHandler()
        {
            NetcodeServerInfo netcodeServerInfo = new NetcodeServerInfo() { IPAddress = netcodeServerIPAddress, Port = port};
            Task task = new Task(() => HttpConnectionManager.ConnectionHandler(new string[] { "http://localhost:8088/" }, netcodeServerInfo));
            task.Start();
        }
    }
}
