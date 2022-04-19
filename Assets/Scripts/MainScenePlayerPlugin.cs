using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//绑定在Prefabs/Player中，位于Player物体下的Player子物体上
public class MainScenePlayerPlugin : NetworkBehaviour
{
    [SerializeField]
    private GameObject beaconPrefab; //信标的prefab，可以在Inspector中拖拽指定

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) //只有当前用户是本地时才执行后续的代码
        {
            return;
        }
        //以下填写用户逻辑
    }
}
