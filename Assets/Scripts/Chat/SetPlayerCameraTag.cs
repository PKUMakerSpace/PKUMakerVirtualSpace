using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// 挂载在Main Camera上，用于确保该Main Camera是当前玩家的。
/// </summary>
[RequireComponent(typeof(Camera))]
public class SetPlayerCameraTag : NetworkBehaviour
{
    private void Start()
    {
        if (IsOwner)
        {
            gameObject.tag = "PlayerCamera";
        }
    }
}
