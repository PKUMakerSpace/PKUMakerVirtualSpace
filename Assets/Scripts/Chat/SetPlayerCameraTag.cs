using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
