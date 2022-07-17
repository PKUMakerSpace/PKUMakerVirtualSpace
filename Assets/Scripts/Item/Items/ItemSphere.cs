using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKU.Item;
using PKU.Readable;

public class ItemSphere : MonoBehaviour, ICanInit, IReadable
{

    private ItemData itemData;

    #region ICanInit

    public void Init(ItemData data)
    {
        itemData = data;

    }

    #endregion

    #region IReadableItem

    private ReadBubbleController readBubble = null;

    public void ShowReadBubble()
    {
        if (readBubble == null)
        {
            readBubble = GetComponentInChildren<ReadBubbleController>();
            if (readBubble == null)
            {
                Debug.LogError("ReadBubbleController is null!");
                return;
            }
        }

        readBubble.ShowReadBubble(itemData.itemName, itemData.itemDescription);
    }

    public void HideReadBubble()
    {
        if (readBubble == null)
        {
            readBubble = GetComponentInChildren<ReadBubbleController>();
            if (readBubble == null)
            {
                Debug.LogError("ReadBubbleController is null!");
                return;
            }
        }

        readBubble.HideReadBubble();

    }

    public string GetName()
    {
        return itemData.itemName;
    }

    public float GetDistance(Vector3 playerPos)
    {
        Vector3 itemPos = this.transform.position;
        return (itemPos - playerPos).magnitude;
    }

    #endregion



}
