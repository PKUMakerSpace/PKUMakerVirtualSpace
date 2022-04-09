using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollider : MonoBehaviour {

    public PlayerController playerController;
    public HealthControl healthControl;
    public double damagePercentage = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Hit(int damage)
    {
        healthControl.TakeDamage((int)(damage * damagePercentage),transform);
    }
}
