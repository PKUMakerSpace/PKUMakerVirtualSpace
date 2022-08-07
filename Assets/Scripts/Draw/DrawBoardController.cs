using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
namespace PKU.Draw
{

    public class DrawBoardController : NetworkBehaviour
    {

        public int drawID;

        public SpriteRenderer content;

        [SerializeField]
        private DrawCanvasController drawCanvas;

        public NetworkVariable<bool> isInUsed = new NetworkVariable<bool>(false);

        public NetworkVariable<DrawData2> drawData = new NetworkVariable<DrawData2>();

        [SerializeField]
        float interactRadius;

        List<PlayerDraw> playerInRadius = new List<PlayerDraw>();

        public List<PlayerDraw> playerLastFrame = new List<PlayerDraw>();

        bool hasInit = false;

        //private List<DrawData> drawDataList;

        // Start is called before the first frame update
        void Start()
        {
            //drawCanvas = GameObject.FindGameObjectWithTag("DrawCanvas").GetComponent<DrawCanvasController>();
            //drawDataList = drawCanvas.drawDataList;
            //drawCanvas.drawGlobalManager.RequestDrawDataServerRpc(this, drawID);
            //drawCamera.SetActive(false);

            // TODO 根据DrawDataList生成画板背景
        }

        private void Update()
        {
            /*if (!hasInit && drawCanvas != null)
            {
                var drawData = drawCanvas.drawDataList.Where(d => d.drawID == drawID);
                if (drawData != null)
                {
                    Sprite spr = Sprite.Create(drawData.drawTex, new Rect(0, 0, drawData.pixelWidth, drawData.pixelHeight), new Vector2(0.5f, 0.5f));

                    content.sprite = spr;
                }
            }*/

            FindPlayersInRadius();
        }

        private void FindPlayersInRadius()
        {

            playerInRadius.Clear();

            Collider[] colls = Physics.OverlapSphere(this.transform.position, interactRadius, 1 << LayerMask.NameToLayer("Player"));

            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    //Debug.Log("检测到玩家");
                    GameObject player = colls[i].gameObject;

                    PlayerDraw playerDraw = player.GetComponent<PlayerDraw>();

                    if (playerDraw != null)
                    {
                        playerInRadius.Add(playerDraw);

                        playerDraw.drawBoard = this;
                    }

                }
            }

            playerLastFrame = playerInRadius;

            /*var diff = playerLastFrame.Where(x => !playerInRadius.Any(a => x == a)).ToList();

            foreach (var pd in diff)
            {
                pd.drawBoard = null;
            }

            */

        }

        public void DrawBoardUpdate(DrawData drawData)
        {
            Sprite spr = Sprite.Create(drawData.drawTex, new Rect(0, 0, drawData.pixelWidth, drawData.pixelHeight), new Vector2(0.5f, 0.5f));

            content.sprite = spr;
        }

        /*[ServerRpc(RequireOwnership = false)]
        public void UpdateSpriteServerRpc(ulong clientID)
        {
            if (!IsServer)
            {
                return;
            }

            // ! 可优化

            UpdateSpriteClientRpc();
        }

        [ClientRpc]
        public void UpdateSpriteClientRpc()
        {
            DrawData drawData = drawCanvas.GetDrawData(drawID);

            // ! 可优化

            Sprite spr = Sprite.Create(drawData.drawTex, new Rect(0, 0, drawData.pixelWidth, drawData.pixelHeight), new Vector2(0.5f, 0.5f));

            content.sprite = spr;

        }

        [ServerRpc(RequireOwnership = false)]
        public void SetUsingStateServerRpc(bool state)
        {
            if (!IsServer)
            {
                return;
            }

            isInUsed.Value = state;
        }*/

        /*[ClientRpc]
        public void ReceiveDrawDataClientRpc(DrawData drawData)
        {
            Debug.Log("Receive!");
        }*/

    }
}
