using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

namespace PKU.Chat
{
    public enum PlayerChatState
    {
        Think, Speak, Leave
    }

    public class PlayerChat : NetworkBehaviour
    {
        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private Canvas canvasChat;

        [SerializeField]
        private TMP_Text speechTextIn;

        [SerializeField]
        private TMP_Text speechTextOut;

        [SerializeField]
        private RectTransform speechBubble;

        //private ChatGlobalManager chatGlobalManager;

        private void OnEnable()
        {
            //mainCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<Camera>();
            //EventHandler.DisplayChatMessage += OnDisplayChatMessage;
        }

        private void Start()
        {
            //chatGlobalManager = GameObject.FindGameObjectWithTag("ChatGlobalManager").GetComponent<ChatGlobalManager>();
        }

        private void Update()
        {
            /*if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }*/

            if (!IsOwner)
            {
                canvasChat.enabled = false;
            }
        }

        public void OnInputFieldSelected()
        {
            playerController.isChatSelected = true;
        }

        public void OnInputFieldDeselected()
        {
            playerController.isChatSelected = false;
        }

        public void OnSendButtonClicked()
        {
            Debug.Log("Canvas is belong to client id" + OwnerClientId);
            //chatGlobalManager.ReceiveChatMessageServerRpc(OwnerClientId, "Hello World");
            ReceiveChatMessageServerRpc(OwnerClientId, speechTextIn.text.ToString());
        }

        /*public void OnDisplayChatMessage(ulong clientID, string message)
        {
            Debug.Log(clientID + message);
        }*/

        [ServerRpc(RequireOwnership = false)]
        public void ReceiveChatMessageServerRpc(ulong clientID, string message)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (!IsServer)
                return;

            /*ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID }
                }
            };*/

            // Do something, check etc.


            DisplayChatMessageClientRpc(clientID, message);

        }

        [ClientRpc]
        public void DisplayChatMessageClientRpc(ulong clientID, string message)
        {
            //EventHandler.CallDisplayChatMessage(clientID, message);
            Debug.Log("ID " + clientID + " " + message);

            if (clientID != OwnerClientId)
            {
                return;
            }

            speechTextOut.text = message;

            LayoutRebuilder.ForceRebuildLayoutImmediate(speechBubble.GetComponent<RectTransform>());
        }

    }

}
