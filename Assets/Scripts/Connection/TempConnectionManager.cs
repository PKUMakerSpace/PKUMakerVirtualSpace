using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace PKU.Connection
{
    public class TempConnectionManager : MonoBehaviour
    {
        public TMPro.TMP_InputField inputField;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator Test()
        {
            Debug.Log(inputField.text);
            var netCodeServerInfoTask = HttpConnectionManager.RequestForConnection("http://localhost:8088/", "1800013008@pku.edu.cn", inputField.text);

            yield return new WaitUntil(() => netCodeServerInfoTask.IsCompleted);

            var netcodeServerInfo = netCodeServerInfoTask.Result;

            Debug.Log(netcodeServerInfo.IPAddress);
            Debug.Log(netcodeServerInfo.Port);

            NetcodeConnectionConfig.serverIPAddress = netcodeServerInfo.IPAddress;
            NetcodeConnectionConfig.serverPort = netcodeServerInfo.Port;

            SceneManager.LoadScene(inputField.text, LoadSceneMode.Single);

            //if (inputField.text.Equals("PKUMakers"))
            //{
            //    SceneManager.LoadScene("PKUMakers", LoadSceneMode.Single);
            //}
        }

        public void OnConnectionButtonClicked()
        {
            StartCoroutine(Test());
        }
    }
}

