using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

namespace PKU.Item
{
    public class ItemGlobalManager : Singleton<ItemGlobalManager>
    {
        [SerializeField]
        NetworkObjectPool objectPool;

        //[SerializeField]
        //private int initSpawnAmount;

        /// <summary>
        /// 是否已经初始化场景中的物品
        /// </summary>
        private bool hasInitMapItem = false;

        [SerializeField]
        private GameObject itemBase;

        /// <summary>
        /// 用于存储item的详细信息
        /// </summary>
        public ItemDataList_SO itemDataList_SO;

        /// <summary>
        /// 用于存储场景中的mapItem(暂时使用仅供测试)
        /// </summary>
        public MapItemList_SO mapItemList_SO;

        /*private void SpawnPrefab()
        {
            for (int i = 0; i < initSpawnAmount; i++)
            {
                GameObject go = objectPool.GetNetworkObject(itemPrefab).gameObject;
                //GameObject go = Instantiate(itemPrefab);
                go.transform.position = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
                go.GetComponent<NetworkObject>().Spawn(true);
            }
        }*/

        /// <summary>
        /// 在场景中丢下物品
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <param name="position">位置</param>
        public void DropItemOnMap(int itemID, Vector3 position)
        {
            // 从对象池中取出item基类物体
            GameObject item = objectPool.GetNetworkObject(itemBase).gameObject;

            // 获取item详细信息
            ItemData itemData = GetItemDataWithID(itemID);

            // 将item移动到相应位置
            item.transform.position = position;

            // 在场景中生成item基类物体
            item.GetComponent<NetworkObject>().Spawn(true);

            // 从对象池中取出item子类物体
            GameObject itemPrefab = objectPool.GetNetworkObject(itemData.itemPrefab).gameObject;

            // 在场景中生成item子类物体
            itemPrefab.GetComponent<NetworkObject>().Spawn(true);

            // 重定向item子类物体的parent
            itemPrefab.transform.SetParent(item.transform, false);

            // 对item进行初始化
            item.GetComponent<ItemController>().Init(itemData);
        }


        private void Update()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (!hasInitMapItem)
            {
                hasInitMapItem = true;

                InitMapItem();

                //SpawnPrefab();
            }
        }


        /// <summary>
        /// 初始化场景中所有的的MapItem
        /// </summary>
        private void InitMapItem()
        {
            List<MapItem> mapItemList = mapItemList_SO.mapItemList;

            foreach (var mapItem in mapItemList)
            {
                DropItemOnMap(mapItem.itemID, mapItem.position);
            }
        }

        /// <summary>
        /// 用itemID查找itemData, 即item的详细信息
        /// </summary>
        /// <returns></returns>
        public ItemData GetItemDataWithID(int itemID)
        {
            return itemDataList_SO.itemDataList.Find(i => i.itemID == itemID);
        }


        /*private void OnDisable()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            SaveMapItem();

        }*/


        /*/// <summary>
        /// 获取场景中的所有MapItem, 并将其写入mapItemList_SO中
        /// </summary>
        private void SaveMapItem()
        {

            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            List<MapItem> mapItemList = mapItemList_SO.mapItemList;

            mapItemList.Clear();

            // 场景中的每个item都有一个controller
            foreach (var item in FindObjectsOfType<ItemController>())
            {
                MapItem mapItem = new MapItem
                {
                    itemID = item.itemID,
                    position = item.transform.position
                };

                mapItemList.Add(mapItem);
            }

            //EditorUtility.SetDirty(mapItemList_SO);

        }*/

    }
}
