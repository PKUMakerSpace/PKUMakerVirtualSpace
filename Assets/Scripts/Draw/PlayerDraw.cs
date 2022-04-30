using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;


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

        private DrawCanvasController drawCanvas = null;

        private Camera drawCamera;

        [SerializeField]
        private GameObject DrawUI;

        private bool isDrawing = false;

        private bool no_drawing_on_current_drag = false;

        private bool mouse_was_previously_held_down = false;

        public LayerMask drawLayers;

        private Vector2 previous_drag_position;

        public float transparency = 1f;


        private void Start()
        {
            DrawUI.SetActive(false);
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

            drawCanvas = drawBoard.drawCanvas;

            drawCanvas.Init();

            // 保管玩家的camera
            playerCam = playerController.Camera;

            playerController.Camera = drawBoard.drawCamera;

            drawCamera = drawBoard.drawCamera.GetComponent<Camera>();

            playerCam.SetActive(false);

            drawBoard.drawCamera.SetActive(true);

            drawBoard.isInUsed = true;

            no_drawing_on_current_drag = mouse_was_previously_held_down = false;

            Debug.Log("Enter Draw Mode.");

            DrawUI.SetActive(true);
        }

        public void OnDrawModeExit()
        {
            Debug.Log("Leave Draw Mode.");

            DrawUI.SetActive(false);

            playerController.Camera = playerCam;

            drawBoard.drawCamera.SetActive(false);

            drawCamera = null;

            playerCam.SetActive(true);

            playerController.isDrawing = isDrawing = false;

            playerController.SetSpeedServerRpc(0, 0);

            drawBoard.isInUsed = false;
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
            SetBrushColor(new Color(255f, 255f, 255f, 0f));
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

