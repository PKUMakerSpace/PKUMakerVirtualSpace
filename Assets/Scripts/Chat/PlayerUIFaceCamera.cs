using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIFaceCamera : NetworkBehaviour
{
    private Camera mainCamera = null;

    private void FixedUpdate()
    {
        if (mainCamera == null)
        {
            if (GameObject.FindGameObjectWithTag("PlayerCamera") != null)
                mainCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        }
        else
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(Vector3.up, 180);
        }
    }


    /*private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


    }*/
}
