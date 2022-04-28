using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

namespace PKU.Chat
{
    /*public enum PlayerChatState
    {
        Think, Speak, Leave
    }*/

    public class PlayerChat : NetworkBehaviour
    {
        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private Canvas canvasChat;

        [SerializeField]
        private TMP_InputField speechTextIn;

        [SerializeField]
        private TMP_Text speechTextOut;

        [SerializeField]
        private RectTransform speechBubble;

        //private PlayerChatState chatState;
        private Coroutine speechBubbleAnim;

        private bool isAnimRunning;

        [SerializeField]
        private CanvasGroup fadeCanvasGroup;

        [SerializeField]
        private float fadeInTime;

        [SerializeField]
        private float displayTime;

        [SerializeField]
        private float fadeOutTime;

        //private ChatGlobalManager chatGlobalManager;

        private void OnEnable()
        {
            //mainCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<Camera>();
            //EventHandler.DisplayChatMessage += OnDisplayChatMessage;
        }

        private void Start()
        {
            if (!IsOwner)
            {
                canvasChat.enabled = false;
            }

            fadeCanvasGroup.alpha = 0;
            speechTextIn.text = "";
            //chatGlobalManager = GameObject.FindGameObjectWithTag("ChatGlobalManager").GetComponent<ChatGlobalManager>();
        }

        public void OnInputFieldSelected()
        {
            playerController.isChatSelected = true;
            playerController.SetSpeedServerRpc(0, 0);
            //chatState = PlayerChatState.Think;
        }

        public void OnInputFieldDeselected()
        {
            playerController.isChatSelected = false;
            playerController.SetSpeedServerRpc(0, 0);
        }

        /*private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Debug.Log("Press Enter to speak!");
                if (speechTextIn.text != "")
                {
                    ReceiveChatMessageServerRpc(OwnerClientId, speechTextIn.text);
                    speechTextIn.text = "";
                }
            }
        }*/

        public void OnSendButtonClicked()
        {
            Debug.Log("Canvas is belong to client id" + OwnerClientId);
            if (speechTextIn.text != "") // 如果输入值为空 
            {
                ReceiveChatMessageServerRpc(OwnerClientId, speechTextIn.text);
                speechTextIn.text = "";
            }
            //chatGlobalManager.ReceiveChatMessageServerRpc(OwnerClientId, "Hello World");

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

            // bubble协程动画还在进行中
            if (isAnimRunning)//(chatState == PlayerChatState.Speak)
            {
                StopCoroutine(speechBubbleAnim);

                isAnimRunning = false;
            }

            speechBubbleAnim = StartCoroutine(SpeechBubbleAnim(message));

        }

        private IEnumerator SpeechBubbleAnim(string message)
        {
            isAnimRunning = true;

            // 准备工作

            fadeCanvasGroup.alpha = 0;

            speechTextOut.text = message;

            LayoutRebuilder.ForceRebuildLayoutImmediate(speechBubble.GetComponent<RectTransform>());

            // 淡入

            yield return fadeAnim(1.0f, fadeInTime);

            yield return holdAnim(displayTime);

            yield return fadeAnim(0f, fadeOutTime);

            fadeCanvasGroup.alpha = 0;

            isAnimRunning = false;

            yield return null;

        }

        private IEnumerator fadeAnim(float targetAlpha, float fadeTime)
        {
            float fadeSpeed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeTime;

            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }

        }

        private IEnumerator holdAnim(float holdTime)
        {
            for (float timer = 0; timer < holdTime; timer += Time.deltaTime)
            {
                yield return null;
            }
        }

    }

}
