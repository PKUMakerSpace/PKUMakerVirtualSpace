using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*玩家生命值控制*/
public class HealthControl : MonoBehaviour {
    public int HP = 500;
    public Animator animator;
    public PlayerController controller;
    public GameObject bloodEffect;
    public int damageID = Animator.StringToHash("Damage");
    public Slider healthShow;
	// Use this for initialization
	void Start () {
        healthShow.value = HP;
	}
	
	// Update is called once per frame
	void Update () {
	}
    public void TakeDamage(int damage,Transform HitPos)
    {
        HP -= damage;
        healthShow.value = Mathf.Max(0, HP);
        animator.SetInteger(damageID, 1);
     //   Debug.Log("hp=" + HP);
        GameObject.Instantiate(bloodEffect, HitPos);//血液效果需改进
        if (HP <= 0)
            controller.Die();
    }
    
}
