using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PKU.Draw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class DrawCanvasController : NetworkBehaviour
    {
        public Color brushColor = Color.red;

        public int brushWidth = 3;

        public Color resetColor = new Color(255, 255, 255, 0);

        public delegate void BrushFunc(Vector2 worldPos, ref Vector2 previousDragPos);

        public BrushFunc currentBrush;


        [SerializeField]
        private Sprite drawSprite;

        private Texture2D drawTex;

        Color[] cleanColors;

        Color32[] curColors;

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

        public void Init()
        {

            currentBrush = PenBrush;

            drawSprite = this.GetComponent<SpriteRenderer>().sprite;
            drawTex = drawSprite.texture;

            cleanColors = new Color[(int)drawSprite.rect.width * (int)drawSprite.rect.height];
            for (int x = 0; x < cleanColors.Length; x++)
                cleanColors[x] = resetColor;
            //draw_texture = new Texture2D(drawSprite.texture.width, drawSprite.texture.height);

            //ResetCanvas();

        }

    }
}
