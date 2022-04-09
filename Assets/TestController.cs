using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TestController : NetworkBehaviour
{
    // Start is called before the first frame update
    private NetworkVariable<bool> isIdle = new NetworkVariable<bool>(false);

    void Start()
    {
        
    }

    [ServerRpc]
    public void StateServerRpc(bool clientIsIdle) { 
        isIdle.Value = clientIsIdle;
    }

    public void AnimationControl()
    {
        //GetComponent<Animator>().SetBool("Idle", isIdle.Value);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKey(KeyCode.A))
            {
                GetComponent<Animator>().SetBool("Idle", true);
                //StateServerRpc(true);
            }
            else
            {
                GetComponent<Animator>().SetBool("Idle", false);
                //StateServerRpc(false);
            }
        }
        AnimationControl();
        
    }
}
