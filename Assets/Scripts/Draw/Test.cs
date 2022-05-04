using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PKU.Draw;

public class Test : NetworkBehaviour
{
    [SerializeField]
    private List<DrawData> drawDataList = new List<DrawData>();

    private void Sender(ulong clientID, FastBufferReader reader)
    {
        reader.ReadValueSafe(out int width);
        reader.ReadValueSafe(out int height);
    }

    private void Start()
    {
        //Receiving
        using FastBufferWriter writer = new FastBufferWriter();

        writer.WriteValueSafe(114514);

        ulong[] clientID = { 0 };

        //CustomMessagingManager.SendUnnamedMessage(clientID, writer);


    }



}
