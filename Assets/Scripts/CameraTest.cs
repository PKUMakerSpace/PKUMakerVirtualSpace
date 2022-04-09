using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour {

    public Transform CameraInOpticPose;

    private float transitionSmoothness = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        this.transform.position = CameraInOpticPose.position;
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, CameraInOpticPose.rotation, transitionSmoothness);
        this.transform.rotation = CameraInOpticPose.rotation;
        Debug.Log(Vector3.Distance(this.transform.position, CameraInOpticPose.position));
	}
}
