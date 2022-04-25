using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKU.Item;

[CreateAssetMenu(fileName = "MapItemList_SO", menuName = "Item/MapItemList_SO")]
public class MapItemList_SO : ScriptableObject
{
    public List<MapItem> mapItemList;
}