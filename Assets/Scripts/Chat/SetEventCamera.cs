using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Canvas))]
public class SetEventCamera : NetworkBehaviour
{
    private Camera mainCamera = null;
    private bool isSetComplete = false;

    private void Update()
    {
        if (isSetComplete)
            return;

        if (mainCamera == null)
        {
            if (GameObject.FindGameObjectWithTag("PlayerCamera") != null)
                mainCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        }
        else
        {
            gameObject.GetComponent<Canvas>().worldCamera = mainCamera;
            isSetComplete = true;
        }
    }
}
