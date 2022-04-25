using UnityEngine;

namespace PKU.Item
{
    [System.Serializable]
    public class ItemData
    {
        public int itemID;
        public string itemName;
        public string itemDescription;
        public bool canPickUp;

        public GameObject itemPrefab;

    }

    [System.Serializable]
    public class MapItem
    {
        public int itemID;
        public Vector3 position;

    }
}
