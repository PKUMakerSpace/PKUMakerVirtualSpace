using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PKU.Item;
using TMPro;
using PKU.Event;
using System;
using System.Linq;

public class PlayerInteract : NetworkBehaviour
{

    [SerializeField]
    private float interactRadius = 2f;

    [SerializeField]
    private TMP_Text interactHint;

    private IReadable readableItem = null;

    /// <summary>
    /// 离我最近的readable的距离
    /// </summary>
    private float minReadableDistance = 114514f;

    private List<IReadable> readableList = new List<IReadable>();

    private void Start()
    {
        if (IsOwner)
        {
            PKU.Event.EventHandler.PlayerReadEvent += OnPlayerRead;
        }
    }

    private void OnPlayerRead()
    {
        // 弹出泡泡
        if (readableItem != null)
        {
            if (!readableList.Contains(readableItem))
            {
                readableList.Add(readableItem);
            }

            readableItem.ShowReadBubble();
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectItemsInRadius();
        DisplayInteractHint();

        CheckReadableList();
    }

    /// <summary>
    /// 将那些不在范围内且已经显示的气泡隐藏
    /// </summary>
    private void CheckReadableList()
    {
        for (int i = readableList.Count - 1; i >= 0; i--)
        {
            var readable = readableList[i];
            float distance = readable.GetDistance(transform.position);
            Debug.Log("bubble distance: " + distance);
            if (distance > interactRadius)
            {
                Debug.Log(readable.GetName() + " is out of sight!");
                readable.HideReadBubble();
                readableList.Remove(readable);
            }
        }
    }

    private void DisplayInteractHint()
    {
        if (readableItem != null)
        {
            interactHint.gameObject.SetActive(true);
            interactHint.SetText("按[E]查看" + readableItem.GetName() + "信息");
        }
        else
        {
            interactHint.gameObject.SetActive(false);
        }
    }

    private void DetectItemsInRadius()
    {
        Collider[] colls = Physics.OverlapSphere(this.transform.position, interactRadius, 1 << LayerMask.NameToLayer("ItemBase"));

        //readableItem = null;
        if (readableItem != null)
        {
            float distance = readableItem.GetDistance(transform.position);
            if (distance > interactRadius)
            {
                readableItem = null;
                minReadableDistance = 114514f;
            }
        }
        else
        {
            minReadableDistance = 114514f;
        }

        Debug.Log("colls.Length: " + colls.Length);

        for (int i = 0; i < colls.Length; i++)
        {
            GameObject itemBase = colls[i].gameObject;

            GameObject childItem = itemBase.GetComponent<ItemController>().childItem;

            IReadable readable = childItem.GetComponent<IReadable>();

            // 如果可以查看物品信息
            if (readable != null)
            {
                Debug.Log("Readable detected!!");

                float distance = (itemBase.transform.position - transform.position).magnitude;

                if (distance < minReadableDistance)
                {
                    minReadableDistance = distance;

                    readableItem = readable;

                }

            }

            /*PlayerDraw playerDraw = player.GetComponent<PlayerDraw>();

            if (playerDraw != null)
            {
                playerInRadius.Add(playerDraw);

                playerDraw.drawBoard = this;
            }*/

        }

    }
}
