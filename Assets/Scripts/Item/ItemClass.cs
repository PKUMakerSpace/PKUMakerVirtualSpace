using UnityEngine;

namespace PKU.Item
{
    [System.Serializable]
    public class ItemData
    {
        public int itemID;
        public int varID;
        public string itemName;
        public string itemDescription;
        //public bool canPickUp;

        public GameObject itemPrefab;

    }

    [System.Serializable]
    public class MapItem
    {
        public int itemID;
        public Vector3 position;
        public Quaternion rotation;

    }

    interface ICanInit
    {
        void Init(ItemData data);
    }

    /// <summary>
    /// 可以查看物品信息
    /// </summary>
    interface IReadable
    {
        /*void SetName(string name);
        void SetDescription(string dcp);
        string GetName();
        string GetDescription();*/
        void ShowReadBubble();
        void HideReadBubble();
        string GetName();
        /// <summary>
        /// 返回item与玩家间的距离
        /// </summary>
        /// <param name="playerPos"></param>
        /// <returns></returns>
        float GetDistance(Vector3 playerPos);
    }

    interface IDrawable
    {

    }
}
