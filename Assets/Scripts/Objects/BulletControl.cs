using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour {

    // Use this for initialization
    Rigidbody rig;
    public GameObject Self;
    public GameObject test;
    public int damage;
    public bool settled = false;
    private float Timer;

    Vector3 pos_old;
	void Start () {
        rig = gameObject.GetComponent<Rigidbody>();
        pos_old = transform.position;
        settled = true;
	}
    // Update is called once per frame
    private void FixedUpdate()
    {
        Timer -= Time.fixedDeltaTime;
        RaycastHit hit;
        if (pos_old != transform.position)
        {
            if (Physics.Raycast(pos_old, transform.position - pos_old, out hit, Vector3.Distance(pos_old, transform.position), (int)1))
            {
                if (hit.collider.tag == "PlayerBulletCollider")
                {
                    hit.collider.gameObject.GetComponent<BulletCollider>().Hit(damage);
                }
                Debug.Log("enter" + hit.collider.gameObject.name);
                if (hit.collider.tag != "bullet")
                    Destroy(Self);
            }
        }
        if(Timer<0)
            Destroy(Self);
        pos_old = transform.position;
     //   GameObject temp = GameObject.Instantiate(test);
     //   temp.transform.position = transform.position;
    }
    public void SetVelocity(int velocity,float range,int maxDistance)
    {
        rig = gameObject.GetComponent<Rigidbody>();
        transform.Rotate(new Vector3((Random.value-0.5f) * range, (Random.value-0.5f) * range, (Random.value-0.5f) * range));
        rig.velocity = transform.TransformDirection(Vector3.forward)*velocity;
        Timer = (float)maxDistance / velocity;
    }
    void Update () {
		
	}
}
