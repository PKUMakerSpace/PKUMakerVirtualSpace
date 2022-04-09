using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PCameraOrigin : NetworkBehaviour {

    public Transform CameraPos;
    private float normalDist;
    private Vector3 normalPos;
    private Vector3 initialDirection;
    public float mouseMoveSpeed = 20;
    public NetworkVariable<Vector2> mouseRotation = new NetworkVariable<Vector2>(new Vector2());
    private float limitationMax = 55.563f;
    private float limitationMin = -74.295f;
    private float rotationTransitionSmoothness = 0.2f;
    public int Mode = 0;    //0->玩家输入，1->AI控制

    // Use this for initialization
    private void Start () {
        normalPos = CameraPos.localPosition;
        normalDist = Vector3.Distance(Vector3.zero, normalPos);
        initialDirection = CameraPos.localPosition.normalized;
        if (IsServer)
        {
            mouseRotation.Value = new Vector2(0, 0);
        }
        
    }
	
	// Update is called once per frame
	private void Update () {
        if (IsOwner)
        {
            InputDetection();
        }
        if (IsServer)
        {
            PositionControl();
            CheckCameraPos();
        }
    }
    

    private void InputDetection()
    {
        Vector2 rotationOffset = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * mouseMoveSpeed * Time.deltaTime;
        //rotation.x = rotation.x > limitationMax ? limitationMax : rotation.x;
        //rotation.x = rotation.x < limitationMin ? limitationMin : rotation.x;
        MouseMoniterServerRpc(rotationOffset);
    }
    
    private void PositionControl()
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(mouseRotation.Value), rotationTransitionSmoothness);
    }

    //Monite Pmouse movement
    [ServerRpc]
    private void MouseMoniterServerRpc(Vector2 rotationOffset)
    {
        Vector2 rotation = rotationOffset + mouseRotation.Value;
        rotation.x = rotation.x > limitationMax ? limitationMax : rotation.x;
        rotation.x = rotation.x < limitationMin ? limitationMin : rotation.x;
        mouseRotation.Value = rotation;
    }

    private void CheckCameraPos()
    {
        Vector3 Direction = CameraPos.position - this.transform.position;
        //RaycastHit hit;
        CameraPos.localPosition = normalPos;
        /*if (Physics.Raycast(transform.position, Direction, out hit, normalDist))
        {
            if(hit.collider.gameObject.tag!="objects" && hit.collider.gameObject.tag !="PlayerBulletCollider" )
                CameraPos.position = hit.point;
            else
                CameraPos.localPosition = normalPos;
        }
        else
        {
            CameraPos.localPosition = normalPos;
        }*/
    }

    public void SetRecoil(float force)
    {
        mouseRotation.Value += new Vector2(-force, Random.Range(-0.2f, 0.2f) * force);
    }
}
