using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform LeftIKPos;
    public Transform RightIKPos;

    private Animator animator;

    private int LtCtrlID = Animator.StringToHash("LtCtrl");
    private int KeyBoardZID = Animator.StringToHash("KeyBoardZ");
    private int SpaceID = Animator.StringToHash("Space");

    private float posture = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger(LtCtrlID);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger(KeyBoardZID);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(SpaceID);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftIKPos.position);
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightIKPos.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftIKPos.rotation);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RightIKPos.rotation);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    }
}
