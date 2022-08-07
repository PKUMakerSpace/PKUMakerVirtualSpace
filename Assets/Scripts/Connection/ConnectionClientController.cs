using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace PKU.Connection
{
    public class ConnectionClientController : NetworkBehaviour
    {
        public bool connected = false;
        double time = 0;
        IEnumerator Test()
        {
            var netCodeServerInfoTask = HttpConnectionManager.RequestForConnection("http://localhost:8088/", "1800013008@pku.edu.cn", "PKUMainHub");

            yield return new WaitUntil(() => netCodeServerInfoTask.IsCompleted);

            var netcodeServerInfo = netCodeServerInfoTask.Result;

            Debug.Log(netcodeServerInfo.IPAddress);
            Debug.Log(netcodeServerInfo.Port);

            
        }

        private void Update()
        {
            time += Time.deltaTime;
            if (IsClient && time > 2)
            {
                Test();
                time = 0;
                StartCoroutine(Test());
            }
        }
    }
}
