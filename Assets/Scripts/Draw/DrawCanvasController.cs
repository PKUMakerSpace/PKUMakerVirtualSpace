using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

namespace PKU.Draw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class DrawCanvasController : NetworkBehaviour
    {

        public Color brushColor = Color.red;

        public int brushWidth = 3;

        public Color resetColor = new Color(255, 255, 255, 255);

        public delegate void BrushFunc(Vector2 worldPos, ref Vector2 previousDragPos);

        public BrushFunc currentBrush;

        Sprite drawSprite;

        Texture2D drawTex;

        Color[] cleanColors;

        Color32[] curColors;

        public GameObject drawCamera;

        //[SerializeField]
        //private DrawDataManager drawDataManager;

        public List<DrawData> drawDataList = new List<DrawData>();

        // public DrawGlobalManager drawGlobalManager;

        #region 数据初始化

        public void Start()
        {

            drawCamera.SetActive(false);

            Debug.Log("Ready to request");

            // RequestListServerRpc(myID);



        }

        /*[ServerRpc(RequireOwnership = false)]
        public void RequestListServerRpc(ulong clientID)
        {
            if (myID != 0)
            {
                return;
            }

            Debug.Log("RequestCanvasInitServerRpc " + "clientID: " + clientID);

            Debug.Log("Server Init List");

            drawDataList.Clear();

            drawDataList.Add(new DrawData(114514, 1000, 750));

            drawDataList.Add(new DrawData(666, 1000, 750));

            /*foreach (var drawData in drawDataList)
            {
                if (drawData.drawTex == null)
                {
                    drawData.drawTex = new Texture2D(drawData.pixelWidth, drawData.pixelHeight);

                    int width = drawData.pixelWidth;
                    int height = drawData.pixelHeight;

                    //drawData.drawTex = new Texture2D(width, height);

                    for (int w = 0; w < width; w++)
                    {
                        for (int h = 0; h < height; h++)
                        {
                            drawData.drawTex.SetPixel(w, h, resetColor);
                        }
                    }

                    drawData.drawTex.Apply();
                }
            }

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID }
                }
            };

            //drawDataList.Add(new DrawData(114514, 1000, 750));
            Debug.Log("Sync DrawData with Server!");

            //SyncDrawDataClientRpc(new DrawData(114514, 1000, 750), clientRpcParams);


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

                //SyncDrawDataClientRpc(drawData, clientRpcParams);
            }

        }*/

        /*[ServerRpc(RequireOwnership = false)]
        private void RequestAllDrawDataServerRpc(ulong clientID)
        {
            if (!IsServer)
            {
                Debug.Log("This is not server.");
                return;
            }

            Debug.Log("Request all drawdata from " + clientID);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID }
                }
            };
        }*/

        /*[ClientRpc]
        public void SyncDrawDataClientRpc(DrawData drawData, ClientRpcParams clientRpcParams = default)
        {

            Debug.Log("Sync DrawData with Server! " + myID);

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
                dd = drawData;
            }

        }*/

        #endregion


        #region 笔刷实现

        public void PenBrush(Vector2 worldPos, ref Vector2 previousDragPos)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixelPos = WorldToPixelCoordinates(worldPos);

            // ? 2. Make sure our variable for pixel array is updated in this frame
            curColors = drawTex.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)


            // If you do care about dragging, use the below if/else structure
            if (previousDragPos == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HEREbrush
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixelPos, brushWidth, brushColor);
            }
            else
            {
                // THE USER IS DRAGGINGbrush
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previousDragPos, pixelPos, brushWidth, brushColor);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();

            // 4. If dragging, update where we were previously
            previousDragPos = pixelPos;

        }



        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenAsBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            currentBrush = PenBrush;
        }

        #endregion

        #region 作画函数封装

        public Vector2 WorldToPixelCoordinates(Vector2 worldPos)
        {
            // Change coordinates to local coordinates of this image
            Vector3 localPos = transform.InverseTransformPoint(worldPos);

            // Change these to coordinates of pixels
            float pixelWidth = drawSprite.rect.width;
            float pixelHeight = drawSprite.rect.height;
            float unitsToPixels = pixelWidth / drawSprite.bounds.size.x;// /* transform.localScale.x;

            // Need to center our coordinates
            float centered_x = localPos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = localPos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixelPos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixelPos;
        }

        public void MarkPixelsToColour(Vector2 centerPixel, int thickness, Color color)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)centerPixel.x;
            int center_y = (int)centerPixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - thickness; x <= center_x + thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawSprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - thickness; y <= center_y + thickness; y++)
                {
                    MarkPixelToChange(x, y, color);
                }
            }
        }

        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int arrPos = y * (int)drawSprite.rect.width + x;

            // Check if this is a valid position
            if (arrPos > curColors.Length || arrPos < 0)
                return;

            curColors[arrPos] = color;
        }

        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 startPoint, Vector2 endPoint, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(startPoint, endPoint);
            Vector2 direction = (startPoint - endPoint).normalized;

            Vector2 curPos = startPoint;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerpSteps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerpSteps)
            {
                curPos = Vector2.Lerp(startPoint, endPoint, lerp);
                MarkPixelsToColour(curPos, width, color);
            }
        }

        public void ApplyMarkedPixelChanges()
        {
            drawTex.SetPixels32(curColors);
            drawTex.Apply();
        }

        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawTex.SetPixels(cleanColors);
            drawTex.Apply();
        }

        #endregion


        /// <summary>
        /// 初始化Canvas
        /// </summary>
        /// <param name="drawID"></param>
        public void DrawInit(int drawID)
        {
            currentBrush = PenBrush;

            // TODO 

            //DrawData drawData = GetDrawData(drawID);

            //RequestMonoDrawDataServerRpc(myID, drawID);

            var drawData = drawDataList.Find(d => d.drawID == drawID);

            //drawData.drawTex = new Texture2D(drawData.pixelWidth, drawData.pixelHeight);

            Debug.Log("size " + drawData.pixelWidth + " " + drawData.pixelHeight);

            /*if (drawData.drawTex == null)
            {
                Debug.Log("DrawTex is null.");

                //RequestNewDrawTextureServerRpc(drawID);
            }*/

            drawSprite = Sprite.Create(drawData.drawTex, new Rect(0, 0, drawData.pixelWidth, drawData.pixelHeight), new Vector2(0.5f, 0.5f));

            this.GetComponent<SpriteRenderer>().sprite = drawSprite;

            drawTex = drawData.drawTex;

            //TestDataServerRpc(drawData);

            //drawSprite = this.GetComponent<SpriteRenderer>().sprite;
            //drawTex = drawSprite.texture;

            /*cleanColors = new Color[(int)drawSprite.rect.width * (int)drawSprite.rect.height];
            for (int x = 0; x < cleanColors.Length; x++)
                cleanColors[x] = resetColor;
            //draw_texture = new Texture2D(drawSprite.texture.width, drawSprite.texture.height);

            //ResetCanvas();*/

        }

        /*[ServerRpc(RequireOwnership = false)]
        private void RequestMonoDrawDataServerRpc(ulong clientID, int drawID)
        {
            if (myID != 0)
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

        public void UpdateDrawData(int drawID)
        {
            UpdateDrawDataServerRpc(new DrawData(drawID, drawTex));
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateDrawDataServerRpc(DrawData drawData)
        {
            if (myID != 0)
            {
                return;
            }

            SyncDrawDataClientRpc(drawData);
        }*/

        public DrawData GetCanvasDrawData(int drawID)
        {
            return new DrawData(drawID, drawTex);
        }

        /*[ServerRpc]
        private void RequestNewDrawTextureServerRpc(int drawID)
        {
            if (!IsServer)
                return;

            SpawnNewDrawTextureClientRpc(drawID);
        }

        [ServerRpc]
        private void TestDataServerRpc(DrawData dat)
        {
            if (!IsServer)
                return;

            Debug.Log("dat ok!");
        }

        [ClientRpc]
        private void SpawnNewDrawTextureClientRpc(int drawID)
        {
            var drawData = drawDataList.Where(d => d.drawID == drawID).FirstOrDefault();

            drawData.drawTex = new Texture2D(drawData.pixelWidth, drawData.pixelHeight);

            int width = drawData.pixelWidth;
            int height = drawData.pixelHeight;

            //drawData.drawTex = new Texture2D(width, height);

            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    drawData.drawTex.SetPixel(w, h, resetColor);
                }
            }

            drawData.drawTex.Apply();
        }*/



        /*
        public DrawData GetDrawData(int drawID)
        {
            return drawGlobalManager.drawDataList.Find(d => d.drawID == drawID);
        }*/

    }
}
