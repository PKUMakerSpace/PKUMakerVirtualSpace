using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolControl : MonoBehaviour
{

    const int ID_STAND = 0;
    const int ID_CROUCH = 1;
    const int ID_PRONE = 2;
    const int ID_MAIN_WEAPON1 = 1;
    const int ID_MAIN_WEAPON2 = 2;
    const int ID_UNARMED = 0;
    const int ID_AIM = 1;
    const int ID_UNAIM = 0;

    private Animator animator;
    private Animator playerAnimator;
    private PlayerController playerController;

    private float[] FireRate = { 0.3f, 0.1f, 0.08f };

    private Transform CameraOrigin;
    private Transform LeftHand;                        //左手托枪的位置
    private Transform RightHand;                       //右手装枪的位置
    public Transform LeftIKPos;
    public Transform RightIKPos;
    public Transform SilencerPos;
    public Transform ScopePos;

    private GameObject player;
    public GameObject Self;
    public GameObject Silencer;

    bool isSettled = false;
    private int FireID = Animator.StringToHash("Fire");
    private int FireModeID = Animator.StringToHash("FireMode");
    private int OnTriggerID = Animator.StringToHash("OnTrigger");

    private float recoil = 0.5f;
    private PCameraOrigin originController;

    private int FireMode = 1; //0 -> SemiAuto 1 -> FullAuto 2 -> Burst -1 -> BoltAction 

    public MainWeapon Data;


    // Use this for initialization
    void Start()
    {
        animator = this.GetComponentInChildren<Animator>();
    }
    public void Drop()
    {
        GameObject collider = GameObject.Instantiate(Data.objectColliderPrefab, player.transform.position, Quaternion.identity);
        GameObject temp = Data.CreateWeapon(collider.transform);
        collider.GetComponentInChildren<Objects>().Data = temp.GetComponent<PistolControl>().Data;
        Destroy(Self);
    }
    public void SetPlayer(GameObject p)
    {
        player = p;
        playerAnimator = player.GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController>();
        CameraOrigin = playerController.CameraOrigin;
        originController = CameraOrigin.gameObject.GetComponent<PCameraOrigin>();
        LeftHand = playerController.LeftHand;
        RightHand = playerController.RightHand;

        isSettled = true;
    }
    public void AddScope(GameObject scope)
    {

    }
    public bool AddSilencer(GameObject silencer)
    {
        if (Data.hasSilencer != true)
        {
            Data.hasSilencer = true;
            Silencer = silencer;
        }
        else
        {
            Silencer.GetComponent<SilenserControl>().ToBackPack(playerController);
            Silencer = silencer;
        }
        return true;
    }
    public void DropSilencer()
    {
        Data.hasSilencer = false;
        Silencer.GetComponent<SilenserControl>().Drop(playerController);
    }
    public void PutSilencerToBackPack()
    {
        Data.hasSilencer = false;
        Silencer.GetComponent<SilenserControl>().ToBackPack(playerController);
    }
    public void AddMagpul(GameObject magpul)
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (!isSettled)
            return;
        if (playerController.weaponState != ID_MAIN_WEAPON1 && gameObject == playerController.MainWeapon1)
            return;
        if (playerController.weaponState != ID_MAIN_WEAPON2 && gameObject == playerController.MainWeapon2)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(FireID);
            originController.SetRecoil(recoil);
        }

        if (FireMode == 1 && Input.GetMouseButton(0))
        {
            animator.SetBool(OnTriggerID, true);
            originController.SetRecoil(recoil);
        }
        else
        {
            animator.SetBool(OnTriggerID, false);
        }
        if (playerAnimator.GetCurrentAnimatorStateInfo(3).IsName("EquipRifleR"))            //装备枪时让枪与右手绑定
        {
            transform.position = RightHand.position;
            transform.rotation = RightHand.rotation;
        }
        else if (playerController.bodyState == ID_STAND)
        {
            if (playerAnimator.GetFloat("AimState") < 0.1)                               //没有瞄准时枪在右手，朝向左手
            {
                transform.position = RightHand.position;
                transform.LookAt(LeftHand);
            }
            else                                                                         //瞄准时枪在右手，朝向镜头方向，左手由IK控制
            {
                transform.position = RightHand.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(CameraOrigin.eulerAngles.x, CameraOrigin.eulerAngles.y, CameraOrigin.eulerAngles.z)), 0.3f);
            }
        }
        else if (playerController.bodyState == ID_CROUCH)                                 //另外两种状态，类似
        {
            if (playerAnimator.GetFloat("AimState") < 0.1)
            {
                transform.position = RightHand.position;
                transform.LookAt(LeftHand);
                transform.Rotate(new Vector3(20, 0, 0));
            }
            else
            {
                transform.position = RightHand.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(CameraOrigin.eulerAngles.x, CameraOrigin.eulerAngles.y, CameraOrigin.eulerAngles.z)), 0.3f);
            }
        }
        else if (playerController.bodyState == ID_PRONE)
        {
            if (playerAnimator.GetFloat("AimState") < 0.1)
            {
                transform.position = RightHand.position;
                transform.LookAt(LeftHand);
                transform.Rotate(new Vector3(20, 0, 0));
            }
            else
            {
                transform.position = RightHand.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(CameraOrigin.eulerAngles.x, CameraOrigin.eulerAngles.y, CameraOrigin.eulerAngles.z)), 0.3f);
            }
        }


        //  this.transform.rotation = CameraOrigin.rotation;
    }
}
