using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlockControl : MonoBehaviour
{

    public Transform NeckPos;
    private Vector3 offsetPos;

    private Animator animator;
    private int TriggerID = Animator.StringToHash("Trigger");
    private int EmptyID = Animator.StringToHash("Empty");
    private int PostureID = Animator.StringToHash("posture");
    private int AimStateID = Animator.StringToHash("AimState");
    private int ReloadID = Animator.StringToHash("Reload");

    public int clipsize = 17; //mag capacity

    private int ammoCount = 0; //bullet count in the mag

    private float CurrentPosture = 0;
    private float TargetPosture = 0;

    // Start is called before the first frame update
    void Start()
    {
        offsetPos = NeckPos.position - this.transform.position;
        animator = this.GetComponent<Animator>();

        //change a new mag
        ammoCount = clipsize;
    }

    // Update is called once per frame
    void Update()
    {
        //sync pos
        this.transform.position = NeckPos.position - offsetPos;

        //InputControl
        

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool(AimStateID, true);
        }
        else if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("Aim"))
            {
                animator.SetTrigger(TriggerID);
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool(AimStateID, false);
        }

        
        if(ammoCount < 17)
        {
            if (ammoCount == 0)
            {
                animator.SetBool(EmptyID, true);
                Debug.Log("Empty");

            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                animator.SetTrigger(ReloadID);
            }
        }

        //posture
        CurrentPosture = Mathf.Lerp(CurrentPosture, TargetPosture, 1f);
        animator.SetFloat(PostureID, CurrentPosture);

    }

    public void Fire()
    {
        ammoCount--;
        Debug.Log(ammoCount);
    }

    public void SetPosture(float value)
    {
        TargetPosture = value; // 0 -> stand 0.5 -> crouch 1 -> prone
    }

    public void Reload(int num)
    {
        ammoCount = num;
        animator.SetBool(EmptyID, false);
    }
}
