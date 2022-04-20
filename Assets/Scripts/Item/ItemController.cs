using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PKU.Item
{
    public class ItemController : NetworkBehaviour
    {

        public int itemID;
        private ItemData itemData;

        //private Collider coll;

        /// <summary>
        /// 初始化item
        /// </summary>
        /// <param name="data"></param>
        public void Init(ItemData data)
        {
            itemData = data;

            //coll = transform.GetComponentInChildren<Collider>();
        }

        /// <summary>
        /// 判断能否捡起item, 如果能, 删除场景中那个item, 返回itemData.
        /// 否则返回null
        /// </summary>
        /// <param name="playerController"></param>
        /// <returns></returns>
        public ItemData PickUpItem(PlayerController playerController)
        {
            if (itemData.canPickUp)
            {
                foreach (Transform child in this.transform)
                {
                    child.gameObject.GetComponent<NetworkObject>().Despawn(true);
                    //NetworkObject.Despawn(true);
                }

                transform.DetachChildren();

                NetworkObject.Despawn(true);

                return itemData;
            }

            return null;
        }

        public bool isPickable(PlayerController playerController)
        {
            if (itemData.canPickUp)
            {
                return true;
            }
            return false;
        }

    }

}

