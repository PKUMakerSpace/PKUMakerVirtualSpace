using UnityEngine;
using Unity.Netcode;
using System;

namespace PKU.Draw
{
    [System.Serializable]
    public struct DrawData
    {
        public int drawID;
        public int pixelWidth;
        public int pixelHeight;
        public Texture2D drawTex;

        public DrawData(int id, int width, int height)
        {
            drawID = id;
            pixelWidth = width;
            pixelHeight = height;
            drawTex = new Texture2D(width, height);

            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    drawTex.SetPixel(w, h, new Color(255, 255, 255, 255));
                }
            }

            drawTex.Apply();
        }

        public DrawData(int id, Texture2D tex)
        {
            drawID = id;
            pixelWidth = tex.width;
            pixelHeight = tex.height;
            drawTex = tex;
        }

        //private byte[] drawPNG;

        //public Sprite drawSprite;

        /*public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref drawID);
            serializer.SerializeValue(ref pixelWidth);
            serializer.SerializeValue(ref pixelHeight);
            //serializer.SerializeValue(ref drawPNG);

            //serializer.SerializeValue(ref drawTex);
        }*/


    };

    public struct DrawData2 : INetworkSerializable
    {
        public int drawID;
        public int pixelWidth;
        public int pixelHeight;
        //public byte[] png;
        //public Texture2D drawTex;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref drawID);
            serializer.SerializeValue(ref pixelWidth);
            serializer.SerializeValue(ref pixelHeight);
            //serializer.SerializeValue(ref png);
            /*if (serializer.IsReader)
            {
                serializer.SerializeValue(ref png);
                drawTex = new Texture2D(pixelWidth, pixelHeight);
                drawTex.LoadImage(png);
            }
            else
            {
                if (drawTex != null)
                    png = drawTex.EncodeToPNG();
                serializer.SerializeValue(ref png);
            }*/
            // ~INetworkSerializable
        }
    }

    public static class SerializationExtensions
    {
        /*public static void ReadValueSafe(this FastBufferReader reader, out Texture2D tex)
        {
            reader.ReadValueSafe(out int width);
            reader.ReadValueSafe(out int height);
            reader.ReadValueSafe(out byte[] png);
            tex = new Texture2D(width, height);
            tex.LoadImage(png);
        }

        public static void WriteValueSafe(this FastBufferWriter writer, in Texture2D tex)
        {
            int width = tex.width;
            int height = tex.height;
            byte[] png = tex.EncodeToPNG();
            writer.WriteValueSafe(width);
            writer.WriteValueSafe(height);
            writer.WriteValueSafe(png);
        }*/

        public static void ReadValueSafe(this FastBufferReader reader, out DrawData dat)
        {
            reader.ReadValueSafe(out int id);
            reader.ReadValueSafe(out int width);
            reader.ReadValueSafe(out int height);
            //reader.ReadValueSafe(out byte[] png);
            reader.ReadValueSafe(out byte[] jpg);
            //reader.ReadValueSafe(out byte[] exr);

            //Debug.Log("Read png byte length: " + png.Length);

            //Debug.Log("Read jpg byte length: " + jpg.Length);

            Texture2D tex = new Texture2D(width, height);
            tex.LoadImage(jpg);

            dat = new DrawData(id, tex);
        }

        public static void WriteValueSafe(this FastBufferWriter writer, in DrawData dat)
        {
            int id = dat.drawID;
            int width = dat.pixelWidth;
            int height = dat.pixelHeight;
            if (dat.drawTex == null)
            {
                Debug.Log("drawTex is null in WriteValueSafe!");
            }

            byte[] png = dat.drawTex.EncodeToPNG();

            byte[] jpg = dat.drawTex.EncodeToJPG();

            //byte[] exr = dat.drawTex.EncodeToEXR();

            Debug.Log("Write png byte length: " + png.Length);

            Debug.Log("Write jpg byte length: " + jpg.Length);

            //Debug.Log("Write exr byte length: " + exr.Length);

            //Debug.Log("png" + png[0]);
            writer.WriteValueSafe(id);
            writer.WriteValueSafe(width);
            writer.WriteValueSafe(height);
            //writer.WriteValueSafe(png);
            writer.WriteValueSafe(jpg);
            //writer.WriteValueSafe(exr);
        }
    }
}