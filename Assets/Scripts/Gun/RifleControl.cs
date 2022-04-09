using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleControl : MonoBehaviour {

    const int ID_STAND = 0;
    const int ID_CROUCH = 1;
    const int ID_PRONE = 2;
    const int ID_MAIN_WEAPON1 = 1;
    const int ID_MAIN_WEAPON2 = 2;
    const int ID_UNARMED = 0;
    const int ID_AIM = 1;
    const int ID_UNAIM = 0;
    const int ID_SEMIAUTO = 0;
    const int ID_FULLAUTO = 1;
    const int ID_BURST = 2;
    const int ID_BOLT = 3;

    private Animator animator;
    private Animator playerAnimator;
    private PlayerController playerController;

    public AudioSource fireAu;
    public AudioSource reloadAu;

    private float[] FireRate = { 0.3f, 0.1f, 0.08f };

    private Transform CameraOrigin;
    public Transform MainCamrea;
    private Transform LeftHand;                        //左手托枪的位置
    private Transform RightHand;                       //右手装枪的位置
    public Transform LeftIKPos;
    public Transform RightIKPos;
    public Transform SilencerPos;
    public Transform OpticPos;
    public Transform InOpticPos;
    public Transform InOpticPos_Original;

    private GameObject player;
    public GameObject Self;
    public GameObject Silencer;
    public GameObject Optic;
    public GameObject Optic_Original;

    bool isSettled = false;
    public bool available = true;

    private float recoil = 0.5f;
    private PCameraOrigin originController;

    public int FireMode = 0; //0 -> SemiAuto 1 -> FullAuto 2 -> Burst -1 -> BoltAction 

    public MainWeapon Data;


    // Use this for initialization
    void Start () {
        animator = this.GetComponentInChildren<Animator>();
	}
    public void Drop()
    {
        GameObject collider = GameObject.Instantiate(Data.objectColliderPrefab, player.transform.position,Quaternion.identity);
        GameObject temp = Data.CreateWeapon(collider.transform);
        collider.GetComponentInChildren<Objects>().Data = temp.GetComponent<RifleControl>().Data;
        playerController.mainSystem.objects.Add(collider.GetComponentInChildren<Objects>());
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
        MainCamrea = playerController.Camera.transform;

        isSettled = true;
    }
    public bool AddOptic(GameObject optic)
    {
        if (Data.hasOptic != true)
        {
            Data.hasOptic = true;
            Optic = optic;
            InOpticPos = Optic.GetComponent<OpticControl>().inOpticPos;
            Optic_Original.SetActive(false);
            MainCamrea.GetComponent<PCamera>().P_Scope = InOpticPos;
        }
        else
        {
            Optic.GetComponent<OpticControl>().ToBackPack(playerController);
            Optic = optic;
            InOpticPos = Optic.GetComponent<OpticControl>().inOpticPos;
            MainCamrea.GetComponent<PCamera>().P_Scope = InOpticPos;
        }
        return true;
    }
    public bool AddSilencer(GameObject silencer)
    {
        if(Data.hasSilencer!=true)
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
    public void SwitchMode()
    {
        while(true)
        {
            FireMode++;
            if(FireMode >= Data.fireRate.Length)
                FireMode = 0;
            if (Data.bulletForOneShot[FireMode] != 0)
                return;
        }
    }
    public bool SwitchModeTo(int Mode)
    {
        if (Mode >= Data.fireRate.Length)
            return false;
        if (Data.bulletForOneShot[Mode] != 0)
        {
            FireMode = Mode;
            return true;
        }
        else
            return false;
    }
    public void DropOptic()
    {
        Data.hasOptic = false;
        Optic_Original.SetActive(true);
        Optic.GetComponent<OpticControl>().Drop(playerController);
        InOpticPos = InOpticPos_Original;
        MainCamrea.GetComponent<PCamera>().P_Scope = InOpticPos;
    }
    public void DropSilencer()
    {
        Data.hasSilencer = false;
        Silencer.GetComponent<SilenserControl>().Drop(playerController);
    }
    public void PutOpticToBackPack()
    {
        Data.hasOptic = false;
        Silencer.GetComponent<OpticControl>().ToBackPack(playerController);
    }
    public void PutSilencerToBackPack()
    {
        Data.hasSilencer = false;
        Silencer.GetComponent<SilenserControl>().ToBackPack(playerController);
    }
    public void AddMagpul(GameObject magpul)
    {

    }
    IEnumerator ReloadBullet(int num)
    {
        reloadAu.Play(0);
        playerController.animator.SetBool("Reload", true);
        available = false;
        Data.bulletNum += num;
        yield return new WaitForSeconds(Data.reLoadTime);
        available = true;
    }
    IEnumerator Semi()
    {
        yield return new WaitForSeconds(Data.fireRate[ID_FULLAUTO]);
    }
    IEnumerator Full()
    {

        yield return new WaitForSeconds(Data.fireRate[ID_FULLAUTO]);
    }
    public void Reload()
    {
        if (Data.maxBulletNum == Data.bulletNum)
            return;
        foreach (Objects o in playerController.objectsOwned)
        {
            if(Data.bulletName == o.Data.name)
            {
                if(Data.maxBulletNum-Data.bulletNum<o.Data.num)
                {
                    o.Data.num -= Data.maxBulletNum - Data.bulletNum;
                    StartCoroutine(ReloadBullet(Data.maxBulletNum - Data.bulletNum));
                }    
                else
                {
                    StartCoroutine(ReloadBullet(o.Data.num));
                    playerController.objectsOwned.Remove(o);
                    Destroy(o.Self);
                    break;
                }
            }
        }
    }
    public void Fire()
    {
        if (Data.bulletNum == 0)
        {
            return;
        }
        switch (FireMode)
        {
            case ID_SEMIAUTO:
                StartCoroutine(Semi());
                break;
            case ID_FULLAUTO:
                StartCoroutine(Full());
                break;
            default:
                break;
        }
    }
    // Update is called once per frame
    void Update () {
        if(!isSettled )
            return;
        if (playerController.weaponState != ID_MAIN_WEAPON1 && gameObject == playerController.MainWeapon1)
            return;
        if (playerController.weaponState != ID_MAIN_WEAPON2 && gameObject == playerController.MainWeapon2)
            return;

        if(playerAnimator.GetCurrentAnimatorStateInfo(3).IsName("EquipRifleR"))            //装备枪时让枪与右手绑定
        {
            transform.position = RightHand.position;
            transform.rotation = RightHand.rotation;
        }
        else if(playerController.bodyState == ID_STAND)                        
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
        else if(playerController.bodyState == ID_CROUCH)                                 //另外两种状态，类似
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
    }
}
