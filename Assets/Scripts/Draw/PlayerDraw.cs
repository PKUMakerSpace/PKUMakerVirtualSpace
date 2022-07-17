using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Linq;

namespace PKU.Draw
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerDraw : NetworkBehaviour
    {
        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private Camera myCamera;

        private GameObject playerCam;

        [SerializeField]
        private TMP_Text interactHint;

        public DrawBoardController drawBoard = null;

        [SerializeField]
        private DrawCanvasController drawCanvas;

        private Camera drawCamera;

        [SerializeField]
        private GameObject DrawUI;

        private bool isDrawing = false;

        private bool no_drawing_on_current_drag = false;

        private bool mouse_was_previously_held_down = false;

        public LayerMask drawLayers;

        private Vector2 previous_drag_position;

        public float transparency = 1f;

        private List<DrawData> drawDataList;

        private GameObject[] drawBoards;

        //private Dictionary<int, DrawData> drawDataDict;

        private void Start()
        {

            drawCanvas = GameObject.FindGameObjectWithTag("DrawCanvas").GetComponent<DrawCanvasController>();

            drawDataList = drawCanvas.drawDataList;

            // drawboard 固定数量
            drawBoards = GameObject.FindGameObjectsWithTag("DrawBoard");

            if (!IsOwner)
            {
                return;
            }

            RequestListServerRpc(OwnerClientId);

            //drawCanvas.myID = OwnerClientId;

            //drawCanvas.RequestListServerRpc(OwnerClientId);

            //drawCanvas.enabled = true;
            //drawCanvas.GetComponent<NetworkObject>().ChangeOwnership(OwnerClientId);

            //drawCanvas.ListInit();

            /*GameObject[] drawBoards = GameObject.FindGameObjectsWithTag("DrawBoard");

            foreach (var board in drawBoards)
            {
                DrawBoardController dbc = board.GetComponent<DrawBoardController>();
                dbc.UpdateSpriteServerRpc(OwnerClientId);
            }*/

            DrawUI.SetActive(false);

        }

        public DrawData GetDrawData(int drawID)
        {
            return drawDataList.Where(d => d.drawID == drawID).FirstOrDefault();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestListServerRpc(ulong clientID)
        {
            if (!IsServer)//|| clientID != OwnerClientId)
            {
                return;
            }

            //TestClientRpc();

            Debug.Log("Client ID: " + clientID + " ServerID: " + OwnerClientId);

            if (clientID == 0)
            {
                drawDataList.Add(new DrawData(114514, 128, 128));

                drawDataList.Add(new DrawData(666, 128, 128));

                SyncDrawBoardClientRpc();

            }
            else
            {

                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientID }
                    }
                };

                foreach (var drawData in drawDataList)
                {

                    if (drawData.drawTex == null)
                    {
                        Debug.Log(drawData.drawID + " drawTex is null before WriteValueSafe!");
                    }
                    else
                    {
                        Debug.Log(drawData.drawID + " drawTex is not null before WriteValueSafe!");
                    }

                    //TestClientRpc();

                    SyncDrawDataClientRpc(drawData, clientRpcParams);
                }

                SyncDrawBoardClientRpc(-1, clientRpcParams);
            }

        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestMonoServerRpc(ulong clientID, int drawID)
        {
            if (!IsServer)//|| clientID != OwnerClientId)
            {
                return;
            }

            Debug.Log("Request " + drawID + " drawdata from " + clientID);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID }
                }
            };

            var drawData = drawDataList.Find(d => d.drawID == drawID);

            SyncDrawDataClientRpc(drawData, clientRpcParams);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateMonoServerRpc(ulong clientID, DrawData drawData)
        {
            if (!IsServer) //|| clientID != OwnerClientId)
            {
                return;
            }

            SyncDrawDataClientRpc(drawData);

            SyncDrawBoardClientRpc(drawData.drawID);
        }

        [ClientRpc]
        private void TestClientRpc()
        {
            Debug.Log("test client Rpc");
        }

        [ClientRpc]
        private void SyncDrawBoardClientRpc(int drawID = -1, ClientRpcParams clientRpcParams = default)
        {
            /*if (!IsOwner)
            {
                return;
            }*/

            foreach (var board in drawBoards)
            {
                Debug.Log("board found!");
                DrawBoardController dbc = board.GetComponent<DrawBoardController>();

                if (dbc.drawID == drawID || drawID == -1)
                {
                    dbc.DrawBoardUpdate(GetDrawData(dbc.drawID));
                }
            }
        }


        [ClientRpc]
        public void SyncDrawDataClientRpc(DrawData drawData, ClientRpcParams clientRpcParams = default)
        {

            Debug.Log("DrawData with Server! " + OwnerClientId);

            //drawDataList.Remove(drawDataList.Where(d => d.drawID == drawData.drawID).FirstOrDefault());

            //drawDataList.Add(drawData);

            var dd = drawDataList.Where(d => d.drawID == drawData.drawID).FirstOrDefault();

            if (dd.drawID != drawData.drawID)
            {
                Debug.Log("Add New DrawData");
                drawDataList.Add(drawData);
            }
            else
            {
                Debug.Log("Modify Old DrawData");
                // !
                drawDataList.Remove(dd);
                drawDataList.Add(drawData);
            }

            Debug.Log("sync success! board");

        }


        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.E))
            {
                if (drawBoard != null && !drawBoard.isInUsed)
                {
                    // 可以作画

                    Debug.Log("This DrawBoard Can be used.");
                }
            }*/

            if (!IsOwner)
            {
                return;
            }

            if (isDrawing)
            {
                interactHint.gameObject.SetActive(false);

                PlayerDrawDetect();

                return;
            }

            if (drawBoard != null)
            {
                if (drawBoard.isInUsed.Value)
                {
                    Debug.Log("This DrawBoard is occupied.");

                    interactHint.gameObject.SetActive(false);

                    return;
                }

                if (!drawBoard.playerLastFrame.Contains(this))
                {
                    drawBoard = null;

                    interactHint.gameObject.SetActive(false);

                    return;
                }

                Debug.Log("This DrawBoard Can be used.");

                interactHint.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    OnDrawModeEnter();
                }
            }

        }

        private void PlayerDrawDetect()
        {
            // Is the user holding down the left mouse button?
            bool mouse_held_down = Input.GetMouseButton(0);

            if (mouse_held_down && !no_drawing_on_current_drag)
            {
                // Convert mouse coordinates to world coordinates
                Vector2 mouse_world_position = drawCamera.ScreenToWorldPoint(Input.mousePosition);

                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, drawLayers.value);

                if (hit != null && hit.transform != null)
                {
                    Debug.Log("Player draw detected!");

                    // TODO 
                    drawCanvas.currentBrush(mouse_world_position, ref previous_drag_position);
                }
                else
                {
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;

                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    }
                }
            }
            else if (!mouse_held_down)
            {
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
            }
            mouse_was_previously_held_down = mouse_held_down;
        }

        private void OnDrawModeEnter()
        {

            if (drawBoard == null)
            {
                Debug.Log("DrawBoard is null!");
                return;
            }

            // TODO
            /*
            进入isDrawing模式
            禁止移动
            玩家速度归零
            切换摄像机
            禁止摄像机随鼠标旋转
            画板被占用 
            */
            playerController.isDrawing = isDrawing = true;

            playerController.SetSpeedServerRpc(0, 0);

            //drawCanvas = drawBoard.drawCanvas;

            // ! 更新

            RequestMonoServerRpc(OwnerClientId, drawBoard.drawID);

            drawCanvas.DrawInit(drawBoard.drawID);

            // 保管玩家的camera
            playerCam = playerController.Camera;

            playerController.Camera = drawCanvas.drawCamera;

            drawCamera = drawCanvas.drawCamera.GetComponent<Camera>();

            playerCam.SetActive(false);

            drawCanvas.drawCamera.SetActive(true);

            // ! board

            var board = drawBoard.GetComponent<DrawBoardController>();

            SetUsingStateServerRpc(board, true);

            no_drawing_on_current_drag = mouse_was_previously_held_down = false;

            Debug.Log("Enter Draw Mode.");

            DrawUI.SetActive(true);
        }

        public void OnDrawModeExit()
        {

            // ! 通知更新

            //drawCanvas.UpdateDrawData(drawBoard.drawID);
            UpdateMonoServerRpc(OwnerClientId, drawCanvas.GetCanvasDrawData(drawBoard.drawID));

            // drawBoard.UpdateSpriteServerRpc(OwnerClientId);

            // !

            Debug.Log("Leave Draw Mode.");

            DrawUI.SetActive(false);

            playerController.Camera = playerCam;

            drawCanvas.drawCamera.SetActive(false);

            drawCamera = null;

            playerCam.SetActive(true);

            playerController.isDrawing = isDrawing = false;

            playerController.SetSpeedServerRpc(0, 0);

            // ! board

            var board = drawBoard.GetComponent<DrawBoardController>();

            SetUsingStateServerRpc(board, false);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetUsingStateServerRpc(NetworkBehaviourReference board, bool state)
        {
            if (!IsServer)
            {
                return;
            }

            if (board.TryGet(out DrawBoardController drawBoardController))
            {
                Debug.Log("Set Board Using State to " + state);

                drawBoardController.isInUsed.Value = state;
            }
        }

        #region UI Widget

        public void SetBrushColor(Color color)
        {
            drawCanvas.brushColor = color;
        }

        public void SetBrushWidth(int width)
        {
            drawCanvas.brushWidth = width;
        }

        public void SetBrushWidth(float width)
        {
            SetBrushWidth((int)width);
        }

        public void SetTransparency(float amount)
        {
            transparency = amount;
            Color color = drawCanvas.brushColor;
            color.a = amount;
            drawCanvas.brushColor = color;
        }

        public void SetPenRed()
        {
            Color color = Color.red;
            color.a = transparency;
            SetBrushColor(color);
            drawCanvas.SetPenAsBrush();
        }

        public void SetPenGreen()
        {
            Color color = Color.green;
            color.a = transparency;
            SetBrushColor(color);
            drawCanvas.SetPenAsBrush();
        }

        public void SetPenBlue()
        {
            Color color = Color.blue;
            color.a = transparency;
            SetBrushColor(color);
            drawCanvas.SetPenAsBrush();
        }

        public void SetEraser()
        {
            SetBrushColor(new Color(255f, 255f, 255f, 255f));
        }

        #endregion


        /*private void OnTriggerEnter(Collider other)
        {
            if (!IsOwner)
            {
                return;
            }

            dbc = other.gameObject.GetComponent<DrawBoardController>();

            if (dbc != null)
            {
                Debug.Log("Enter DrawBoard!");

                interactHint.enabled = true;
                interactHint.gameObject.SetActive(true);
            }
        }*/

        /*private void OnTriggerExit(Collider other)
        {
            if (!IsOwner)
            {
                return;
            }

            if (dbc != null)
            {
                Debug.Log("Exit DrawBoard!");

                interactHint.enabled = false;
                interactHint.gameObject.SetActive(false);
                dbc = null;
            }
        }*/


    }
}

