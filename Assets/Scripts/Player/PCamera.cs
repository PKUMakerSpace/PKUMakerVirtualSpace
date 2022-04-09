using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PCamera : NetworkBehaviour {

    const int ID_STAND_AIM = 1;
    const int ID_STAND_UNAIM = 0;
    const int ID_SCOPE = 2;
    const int ID_CROUCH_AIM = 3;
    const int ID_CROUCH_UNAIM = 4;
    const int ID_PRONE_AIM = 5;
    const int ID_PRONE_UNAIM = 6;

    public Transform P_Stand_UnAim;
    public Transform P_Stand_Aim;
    public Transform P_Crouch_UnAim;
    public Transform P_Crouch_Aim;
    public Transform P_Prone_UnAim;
    public Transform P_Prone_Aim;
    public Transform CameraOrigin;
    public Transform Target;
    public Transform P_Scope;

    private float smoothness = 0.5f;
    private int CameraPosIndex = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (!IsServer)
        {
            return;
        }
        switch (CameraPosIndex)
        {
            case ID_STAND_UNAIM:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Stand_UnAim.position, smoothness);
                this.transform.LookAt(CameraOrigin.position);
                break;
            case ID_STAND_AIM:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Stand_Aim.position, smoothness);
                this.transform.LookAt(Target.position);
                break;
            case ID_SCOPE:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Scope.position, smoothness);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, P_Scope.rotation, smoothness);
                break;
            case ID_CROUCH_AIM:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Crouch_Aim.position, smoothness);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, P_Crouch_Aim.rotation, smoothness);
                break;
            case ID_CROUCH_UNAIM:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Crouch_UnAim.position, smoothness);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, P_Crouch_UnAim.rotation, smoothness);
                break;
            case ID_PRONE_AIM:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Prone_Aim.position, smoothness);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, P_Prone_Aim.rotation, smoothness);
                break;
            case ID_PRONE_UNAIM:
                this.transform.position = Vector3.Lerp(this.transform.position, P_Prone_UnAim.position, smoothness);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, P_Prone_UnAim.rotation, smoothness);
                break;
        }
        
	}

    public void ChangePosTo(int index)
    {
        if (IsServer)
        {
            CameraPosIndex = index;
            switch (index)
            {
                case ID_STAND_UNAIM:
                    smoothness = 0.5f;
                    break;
                case ID_STAND_AIM:
                    smoothness = 2f;
                    break;
                case ID_PRONE_UNAIM:
                    smoothness = 0.5f;
                    break;
                case ID_PRONE_AIM:
                    smoothness = 2f;
                    break;
                case ID_CROUCH_UNAIM:
                    smoothness = 0.5f;
                    break;
                case ID_CROUCH_AIM:
                    smoothness = 2f;
                    break;
                case ID_SCOPE:
                    smoothness = 0.8f;
                    break;
            }
        }
        
    }
}
