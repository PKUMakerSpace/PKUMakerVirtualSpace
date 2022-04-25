using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKU.Item;

[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Item/ItemDataList_SO")]
public class ItemDataList_SO : ScriptableObject
{
    public List<ItemData> itemDataList;
}
