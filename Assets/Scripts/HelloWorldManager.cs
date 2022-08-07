
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UNET;
using PKU.Connection;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        public bool serverMode = true;
        public bool hostMode = false;
        void Start()
        {
            //GetComponent<UNetTransport>().ServerListenPort
            //    GetComponent<UNetTransport>().ConnectAddress

            /*if (hostMode)
            {
                NetworkManager.Singleton.StartHost();
            }
            else if (serverMode)
            {
                NetworkManager.Singleton.StartServer();
                Application.targetFrameRate = 60;
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }*/


            //NetworkManager.Singleton.StartClient();
        }
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client"))
            {
                // var netCodeServerInfoTask = HttpConnectionManager.RequestForConnection("http://localhost:8088/", "1800013008@pku.edu.cn", "PKUMainHub");


                GetComponent<UNetTransport>().ConnectAddress = NetcodeConnectionConfig.serverIPAddress;
                GetComponent<UNetTransport>().ConnectPort = NetcodeConnectionConfig.serverPort;

                Debug.Log(GetComponent<UNetTransport>().ConnectAddress);
                Debug.Log(""+ GetComponent<UNetTransport>().ConnectPort);
                NetworkManager.Singleton.StartClient();
            }
                
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            if (GUILayout.Button("Close"))
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
    }
}