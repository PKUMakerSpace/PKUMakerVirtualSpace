using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using PKU.Item;

[RequireComponent(typeof(PlayerController))]
public class PlayerPickUpItem : NetworkBehaviour
{

    [SerializeField]
    private PlayerController playerController;

    /// <summary>
    /// 当前捡起的物品详细信息
    /// </summary>
    [SerializeField]
    private ItemData currentItemData;


    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Singleton.IsServer == false)
        {
            return;
        }

        // 注意是GetComponentInParent, 因为碰撞体在子物体里
        var itemController = other.gameObject.GetComponentInParent<ItemController>();
        if (itemController != null)
        {
            currentItemData = itemController.PickUpItem(playerController);
        }
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RandomlySpawnItemOnMap();
        }
    }

    private void RandomlySpawnItemOnMap()
    {
        // 按T键
        if (IsServer)
        {
            Vector3 randomPos = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
            // 随机丢下一个可以捡的正方体
            ItemGlobalManager.Instance.DropItemOnMap(1001, randomPos);
        }
    }*/
}
