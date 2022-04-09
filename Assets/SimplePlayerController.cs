using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

/* 控制玩家的大多数动作
 * 其他的控制主要在枪械的脚本及Healthcontrol中
 * 关于人类玩家的物品拾取则写在UIsystemcontroller中*/
[System.Serializable]
public class SimplePlayerController : NetworkBehaviour
{

    public Transform CameraOrigin;                               //拥有camera的一些坐标
    public Transform LeftHand;                                   //左手位置引用
    public Transform RightHand;                                  //右手位置引用

    private Transform LeftIKPos;                                 //左手IK位置引用，位置在枪上
    private Transform RightIKPos;                                //右手IK位置引用，位置在枪上

    public GameObject Camera;                                    //相机引用
    public GameObject Self;                                      //自身引用
    public GameObject canvas;                                    //物品UI系统引用
    public GameObject MainWeapon1;                               //主武器1引用
    public GameObject MainWeapon2;                               //主武器2引用
    public GameObject DeputyWeapon;                              //副武器（手枪）引用


    public List<Objects> objectsAround;                       //周围物体
    public List<Objects> objectsOwned;                        //背包中物体

    public PlayerSystem mainSystem;

    public Text WeaponShow;

    PCamera CameraControl;                                       //Main Camera的script引用

    public Animator animator;                                 //自身animator引用

    //Animator Paras
    private int VerticalID = Animator.StringToHash("KeyboardInputVertical");               //把animator中parameters的ID获取下来，方便之后更改
    private int HorizontalID = Animator.StringToHash("KeyboardInputHorizontal");
    private int CtrlID = Animator.StringToHash("KeyboardInputCtrl");
    private int CameraRotationID = Animator.StringToHash("CameraRotationY");
    private int SprintID = Animator.StringToHash("Sprint");
    private int AimStateID = Animator.StringToHash("AimState");
    private int SpeedID = Animator.StringToHash("Speed");
    private int ArmedID = Animator.StringToHash("Armed");
    private int WeaponChoiceID = Animator.StringToHash("WeaponChoice");
    private int FireID = Animator.StringToHash("Fire");
    private int bodyStateID = Animator.StringToHash("BodyState");

    //KeyboardAxis
    private Vector2 CurrentAxisState;                                   //目前的位移信息
    private NetworkVariable<Vector2> TargetAxisState = new NetworkVariable<Vector2>(new Vector2(0, 0));                                    //目标位移信息，上者是为了平滑运动的插值运算出来的，不需要直接修改，而这个修改了就会朝着修改的方向改变animator
    private float TransitionSmoothness = 1;

    //Rrotate
    private float RotationSmoothness = 0.2f;

    //State_const
    const int ID_STAND = 0;                                            //常量，方便书写，相当于C的#define
    const int ID_CROUCH = 1;
    const int ID_MAIN_WEAPON1 = 1;
    const int ID_MAIN_WEAPON2 = 2;
    const int ID_DEPUTY_WEAPON = 3;
    const int ID_UNARMED = 0;
    const int ID_AIM = 1;
    const int ID_UNAIM = 0;
    const int ID_PRONE = 2;
    const int ID_STAND_AIM = 1;
    const int ID_STAND_UNAIM = 0;
    const int ID_SCOPE = 2;
    const int ID_CROUCH_AIM = 3;
    const int ID_CROUCH_UNAIM = 4;
    const int ID_PRONE_AIM = 5;
    const int ID_PRONE_UNAIM = 6;
    const int ID_SEMIAUTO = 0;
    const int ID_FULLAUTO = 1;
    const int ID_BURST = 2;
    const int ID_BOLT = 3;
    //状态
    public int aimState { get; set; }                                           //瞄准状态，ID_AIM表示瞄准,ID_UNAIM表示未瞄准
    public int weaponState { get; set; }                                        //武器状态，ID_UNARMED表示无武器，ID_MAIN_WEAPON1表示第一个主武器
    public int bodyState { get; set; }                                          //身体状态，ID_STAND表示站立，ID_CROUCH表示下蹲，ID_PRONE表示趴下
    private float isAiming;  //0 -> normal; 1 -> aimState           //用于插值运算
    private float TargetState = 0;
    public float speed = 10;
    public bool hasMainWeapon1 = false;
    public bool hasMainWeapon2 = false;
    public bool hasDeputyWeapon = false;
    public bool isDead = false;
    //Fire
    /*    private float AddictiveFireRate = 0.1f;                       
        private float SingleFireRate = 0.3f;
        private float BurstFireRate = 0.08f;
        private float[] FireRate = { 0.3f, 0.1f, 0.08f };
        private int FireMode = 0; // 0 -> Single 1 -> Addictive 2 -> Burst
        private float timer = 0;         */

    //IK
    private float IKchangeFactor = 0;
    private float IKchangeSmoothness = 5f;

    //Camera
    private bool InOptic = false;
    private bool UIOn = false;

    public int Mode = 0;    //0->玩家控制，1->AI控制

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Camera.SetActive(false);
        }
    }

    // Use this for initialization                              初始化
    private void Start()
    {
        animator = this.GetComponent<Animator>();
        CurrentAxisState = new Vector2(0, 0);
        if (IsServer)
        {
            TargetAxisState.Value = new Vector2(0, 0);
        }

        //状态初始化
        aimState = ID_UNAIM;
        bodyState = ID_STAND;
        weaponState = ID_UNARMED;

        CameraControl = Camera.GetComponent<PCamera>();
    }

    // Update is called once per frame

    private void Standing()                              //站立状态每帧时调用
    {
        if (aimState == ID_UNAIM)
        {
            if (IsServer)
            {
                if (System.Math.Abs(CurrentAxisState.y) > 0.2f || System.Math.Abs(CurrentAxisState.x) > 0.2f)
                    RotationSync();
            }

            //animator.SetFloat(VerticalID, CurrentAxisState.y);
            //animator.SetFloat(HorizontalID, CurrentAxisState.x);
        }
        /*else
        {
            if (IsServer)
            {
                RotationSync();
                speed = System.Math.Abs(CurrentAxisState.x) > System.Math.Abs(CurrentAxisState.y) ? System.Math.Abs(CurrentAxisState.x) : System.Math.Abs(CurrentAxisState.y);
                this.transform.Translate(speed * new Vector3(CurrentAxisState.x, 0, CurrentAxisState.y) * 0.02f);
            }
            animator.SetFloat(CameraRotationID, CameraOrigin.rotation.eulerAngles.x > 90 ? CameraOrigin.rotation.eulerAngles.x - 360 : CameraOrigin.rotation.eulerAngles.x);
            animator.SetFloat(SpeedID, speed);

        }*/

    }
    private void Crouching()                          //下蹲状态时每帧调用
    {
        /*if (aimState == ID_UNAIM)
        {
            if (System.Math.Abs(CurrentAxisState.y) > 0.2f || System.Math.Abs(CurrentAxisState.x) > 0.2f)
                RotationSync();
            animator.SetFloat(VerticalID, CurrentAxisState.y);
            animator.SetFloat(HorizontalID, CurrentAxisState.x);
        }
        else
        {
            RotationSync();
            speed = System.Math.Abs(CurrentAxisState.x) > System.Math.Abs(CurrentAxisState.y) ? System.Math.Abs(CurrentAxisState.x) : System.Math.Abs(CurrentAxisState.y);
            this.transform.Translate(speed * new Vector3(CurrentAxisState.x, 0, CurrentAxisState.y) * 0.02f);
            animator.SetFloat(CameraRotationID, CameraOrigin.rotation.eulerAngles.x > 90 ? CameraOrigin.rotation.eulerAngles.x - 360 : CameraOrigin.rotation.eulerAngles.x);
            animator.SetFloat(SpeedID, speed);

        }*/
    }
    private void Proning()                           //趴下状态时每帧调用
    {
        /*if (aimState == ID_UNAIM)
        {
            if (System.Math.Abs(CurrentAxisState.y) > 0.2f || System.Math.Abs(CurrentAxisState.x) > 0.2f)
                RotationSync();
            animator.SetFloat(VerticalID, CurrentAxisState.y);
            animator.SetFloat(HorizontalID, CurrentAxisState.x);
        }
        else
        {
            RotationSync();
            speed = System.Math.Abs(CurrentAxisState.x) > System.Math.Abs(CurrentAxisState.y) ? System.Math.Abs(CurrentAxisState.x) : System.Math.Abs(CurrentAxisState.y);
            this.transform.Translate(speed * new Vector3(CurrentAxisState.x, 0, CurrentAxisState.y) * 0.02f);
            animator.SetFloat(CameraRotationID, CameraOrigin.rotation.eulerAngles.x > 90 ? CameraOrigin.rotation.eulerAngles.x - 360 : CameraOrigin.rotation.eulerAngles.x);
            animator.SetFloat(SpeedID, speed);
        }*/
    }
    /*
     * 所有的输入检测都应当放在此处，并调用函数，保证角色所有的动作都可以在无用户输入的条件下进行
     */
    private void InputDetection()                     //检测用户的输入，每帧调用，调用的函数在AI接口中
    {

        if (bodyState == ID_STAND && Input.GetKey(KeyCode.LeftShift))
        {
            SetSpeedServerRpc(Input.GetAxis("Horizontal") * 2 * speed, Input.GetAxis("Vertical") * 2 * speed);
        }
        else
        {
            SetSpeedServerRpc(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
        }
        //物品栏，O
        if (Input.GetKeyDown(KeyCode.O))
        {
            UIOn = !UIOn;
            canvas.SetActive(UIOn);
        }
        //主武器1,1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (hasMainWeapon1)
                MainWeapon1On();
        }
        //切换武器模式，T
        if (Input.GetKeyDown(KeyCode.T))
            SwitchMode();
        //装填，R
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        //主武器2,2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (hasMainWeapon2)
                MainWeapon2On();
        }
        //副武器，3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasDeputyWeapon)
                DeputyWeaponOn();
        }
        //收起武器，X
        if (Input.GetKeyDown(KeyCode.X))
            UnArm();
        //蹲下，起立切换，左ctrl
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (bodyState != ID_CROUCH)
                Crouch();
            else if (bodyState == ID_CROUCH)
                StandUp();
        }
        //趴下，起立切换，Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (bodyState != ID_PRONE)
                Prone();
            else if (bodyState == ID_PRONE)
                StandUp();
        }
        //瞄准，右键
        if (weaponState != ID_UNARMED)
        {
            if (Input.GetMouseButtonDown(1))
                Aim();
            if (Input.GetMouseButtonUp(1))
                UnAim();
        }
        //瞄准模式与开火
        if (aimState == ID_AIM)
        {
            if (Input.GetKeyDown(KeyCode.E))
                OpticShift();
            if (Input.GetMouseButtonDown(0))
            {
                if (weaponState == ID_MAIN_WEAPON1 && MainWeapon1.GetComponent<RifleControl>().FireMode == ID_SEMIAUTO)
                    Fire();
                else if (weaponState == ID_MAIN_WEAPON2 && MainWeapon2.GetComponent<RifleControl>().FireMode == ID_SEMIAUTO)
                    Fire();
            }
            if (Input.GetMouseButton(0))
            {
                if (weaponState == ID_MAIN_WEAPON1 && MainWeapon1.GetComponent<RifleControl>().FireMode == ID_FULLAUTO)
                    Fire();
                else if (weaponState == ID_MAIN_WEAPON2 && MainWeapon2.GetComponent<RifleControl>().FireMode == ID_FULLAUTO)
                    Fire();
            }
            if (Input.GetKeyUp(0))
                stopFullFireAnim();
        }
    }
    public void AIControl()//所有AI操作写在此处即可
    {

    }
    private void Update()
    {
        if (isDead)
            return;
        if (Mode == 0 && IsOwner)
            InputDetection();
        else
            AIControl();
        if (bodyState == ID_STAND)
            Standing();
        else if (bodyState == ID_CROUCH)
            Crouching();
        else if (bodyState == ID_PRONE)
            Proning();

        //Weapon Index
        Debug.Log("update");
        Debug.Log("delta time:" + Time.deltaTime);

        CurrentAxisState = Vector2.Lerp(CurrentAxisState, TargetAxisState.Value, TransitionSmoothness / (Vector2.Distance(CurrentAxisState, TargetAxisState.Value) * 10 + 1));
        if (IsServer)
        {
            /*Debug.Log("CurrentAxis: " + CurrentAxisState.x + " " + CurrentAxisState.y);
            Debug.Log("speed:" + speed);
            GetComponent<CharacterController>().SimpleMove(
                (transform.TransformDirection(Vector3.forward) * CurrentAxisState.y +
                transform.TransformDirection(Vector3.right) * CurrentAxisState.x)
                );*/

        }

        isAiming = Mathf.Lerp(isAiming, (float)aimState, 0.2f);

        if (IsOwner)
        {
            animator.SetFloat(AimStateID, isAiming);
        }

    }

    //AI接口，如果加入读条的动作，需要将其中的一些函数改成协程
    [ServerRpc]
    public void SetSpeedServerRpc(float Horizontal, float Vertical)                    //设置vertical和horizontal速度
    {
        /*if (Mathf.Abs(Horizontal) > 1 || Mathf.Abs(Vertical) > 1)
        {
            Debug.Log("错误，设置的速度只能在0到1之间");
            return;
        }*/
        Debug.Log("receive server RPC");
        TargetAxisState.Value = new Vector2(Horizontal, Vertical);
    }
    public void Die()
    {
        if (IsOwner)
        {
            isDead = true;
            foreach (Objects o in objectsOwned)
                DropObject(o);
            DropMainWeapon1();
            DropMainWeapon2();
            DropDeputyWeapon();
            if (IsOwner)
            {
                animator.SetBool("Dead", true);
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //mainSystem.players.Remove(this);
        }


    }
    public bool AddObject(Objects o)                                        //拾取物品
    {
        if (!objectsAround.Contains(o))
            return false;
        foreach (Objects element in objectsOwned)
        {
            if (o.Data is Silenser || o.Data is Optic)
                break;
            if (o.Data.name == element.Data.name)
            {
                if (o == element)
                    return false;
                element.Data.num += o.Data.num;
                objectsAround.Remove(o);
                Destroy(o.Self);
                return true;
            }
        }
        GameObject temp = GameObject.Instantiate(o.Self);
        objectsOwned.Add(temp.GetComponentInChildren<Objects>());
        if (o.Data is Silenser)
            temp.GetComponentInChildren<Objects>().Data = temp.GetComponentInChildren<Objects>().Data.objectPrefab.GetComponent<SilenserControl>().Data;
        if (o.Data is Optic)
            temp.GetComponentInChildren<Objects>().Data = temp.GetComponentInChildren<Objects>().Data.objectPrefab.GetComponent<OpticControl>().Data;
        temp.SetActive(false);
        mainSystem.objects.Remove(o);
        objectsAround.Remove(o);
        Destroy(o.Self);
        return true;
    }
    public void playSingleAnim()
    {
        if (IsOwner)
        {
            animator.SetTrigger("fireSingle");
        }

    }
    public void playFullFireAnim()
    {
        if (IsOwner)
        {
            animator.SetBool("fireFull", true);
        }
    }
    public void stopFullFireAnim()
    {
        if (IsOwner)
        {
            animator.SetBool("fireFull", false);
        }
    }
    /*public void Turn(int DeltaX,int DeltaY)                                     //转身
    {
        CameraOrigin.GetComponent<PCameraOrigin>().mouseRotation+= new Vector2(DeltaY, DeltaX);
    }*/
    public void LookAt(Transform target)                                        //正对着
    {
        CameraOrigin.LookAt(target);
    }
    public void Reload()                                                         //装弹
    {
        switch (weaponState)
        {
            case ID_MAIN_WEAPON1:
                MainWeapon1.GetComponent<RifleControl>().Reload();
                break;
            case ID_MAIN_WEAPON2:
                MainWeapon2.GetComponent<RifleControl>().Reload();
                break;
            default:
                break;
        }
    }
    public void DropObject(Objects o)                                          //丢弃物品
    {
        if (objectsOwned.Contains(o))
        {
            GameObject temp = GameObject.Instantiate(o.Self, transform.position, Quaternion.identity);
            if (o.Data is Silenser)
                temp.GetComponentInChildren<Objects>().Data = temp.GetComponentInChildren<Objects>().Data.objectPrefab.GetComponent<SilenserControl>().Data;
            if (o.Data is Optic)
                temp.GetComponentInChildren<Objects>().Data = temp.GetComponentInChildren<Objects>().Data.objectPrefab.GetComponent<OpticControl>().Data;
            temp.SetActive(true);
            mainSystem.objects.Add(temp.GetComponentInChildren<Objects>());
            objectsOwned.Remove(o);
            Destroy(o.Self);
        }
    }
    public void SwitchMode()                                                 //切换武器模式
    {
        switch (weaponState)
        {
            case ID_MAIN_WEAPON1:
                MainWeapon1.GetComponent<RifleControl>().SwitchMode();
                break;
            case ID_MAIN_WEAPON2:
                MainWeapon2.GetComponent<RifleControl>().SwitchMode();
                break;
        }
    }
    public bool SwitchModeTo(int Mode)
    {
        switch (weaponState)
        {
            case ID_MAIN_WEAPON1:
                return MainWeapon1.GetComponent<RifleControl>().SwitchModeTo(Mode);
            case ID_MAIN_WEAPON2:
                return MainWeapon2.GetComponent<RifleControl>().SwitchModeTo(Mode);
            default:
                return false;
        }
    }
    /*RifleOn Down,PistolOn Down四个标记是用于player动画arms层的，
     * 有对应的收起、装备武器动作时会设为true，之后通过animator中的behavior会自动变回false*/
    public void MainWeapon1On()                                              //装备主武器
    {
        if (IsOwner)
        {
            if (weaponState == ID_MAIN_WEAPON2)
                animator.SetBool("RifleDown", true);
            else if (weaponState == ID_DEPUTY_WEAPON)
                animator.SetBool("PistolDown", true);
            weaponState = ID_MAIN_WEAPON1;
            animator.SetInteger(WeaponChoiceID, 1);
            MainWeapon1.SetActive(true);
            LeftIKPos = MainWeapon1.GetComponent<RifleControl>().LeftIKPos;
            RightIKPos = MainWeapon1.GetComponent<RifleControl>().RightIKPos;
            Camera.GetComponent<PCamera>().P_Scope = MainWeapon1.GetComponent<RifleControl>().InOpticPos;
            if (hasMainWeapon2)
                MainWeapon2.SetActive(false);
            if (hasDeputyWeapon)
                DeputyWeapon.SetActive(false);
            animator.SetBool("RifleOn", true);
            WeaponShow.text = MainWeapon1.GetComponent<RifleControl>().Data.name + " " + MainWeapon1.GetComponent<RifleControl>().Data.bulletNum;

        }
    }
    public void MainWeapon2On()                                              //装备主武器
    {
        if (IsOwner)
        {
            if (weaponState == ID_MAIN_WEAPON1)
                animator.SetBool("RifleDown", true);
            else if (weaponState == ID_DEPUTY_WEAPON)
                animator.SetBool("PistolDown", true);
            weaponState = ID_MAIN_WEAPON2;
            animator.SetInteger(WeaponChoiceID, 1);
            MainWeapon2.SetActive(true);
            LeftIKPos = MainWeapon2.GetComponent<RifleControl>().LeftIKPos;
            RightIKPos = MainWeapon2.GetComponent<RifleControl>().RightIKPos;
            Camera.GetComponent<PCamera>().P_Scope = MainWeapon2.GetComponent<RifleControl>().InOpticPos;
            if (hasMainWeapon1)
            {
                animator.SetBool("RifleDown", true);
                MainWeapon1.SetActive(false);
            }
            if (hasDeputyWeapon)
            {
                animator.SetBool("PistolDown", true);
                DeputyWeapon.SetActive(false);
            }
            animator.SetBool("RifleOn", true);
        }
    }
    public void DeputyWeaponOn()                                              //装备主武器
    {
        if (IsOwner)
        {
            if (weaponState == ID_MAIN_WEAPON1)
                animator.SetBool("RifleDown", true);
            else if (weaponState == ID_MAIN_WEAPON2)
                animator.SetBool("RifleDown", true);
            weaponState = ID_DEPUTY_WEAPON;
            animator.SetInteger(WeaponChoiceID, 3);

            DeputyWeapon.SetActive(true);
            LeftIKPos = DeputyWeapon.GetComponent<PistolControl>().LeftIKPos;
            RightIKPos = DeputyWeapon.GetComponent<PistolControl>().RightIKPos;
            if (hasMainWeapon1)
            {
                MainWeapon1.SetActive(false);
                animator.SetBool("RifleDown", true);
            }
            if (hasMainWeapon2)
            {
                MainWeapon1.SetActive(false);
                animator.SetBool("RifleDown", true);
            }
            animator.SetBool("PistolOn", true);
        }
    }
    public void Fire()                                                      //开火
    {
        if (IsOwner)
        {
            if (weaponState == ID_MAIN_WEAPON1)
            {
                if (MainWeapon1.GetComponent<RifleControl>().available)
                    MainWeapon1.GetComponent<RifleControl>().Fire();
            }
            if (weaponState == ID_MAIN_WEAPON2)
            {
                if (MainWeapon2.GetComponent<RifleControl>().available)
                    MainWeapon2.GetComponent<RifleControl>().Fire();
            }
        }
    }
    public void UnArm()                                                      //解除武装
    {
        if (IsOwner)
        {
            weaponState = ID_UNARMED;
            animator.SetInteger(WeaponChoiceID, weaponState);
            if (hasMainWeapon1)
            {
                MainWeapon1.SetActive(false);
                animator.SetBool("RifleDown", true);
            }
            if (hasMainWeapon2)
            {
                MainWeapon2.SetActive(false);
                animator.SetBool("RifleDown", true);
            }
        }

    }
    public void DropMainWeapon1()                                           //丢弃主武器
    {
        if (IsOwner)
        {
            if (hasMainWeapon1)
                MainWeapon1.GetComponent<RifleControl>().Drop();
            if (weaponState == ID_MAIN_WEAPON1)
                UnArm();
            hasMainWeapon1 = false;
        }
    }
    public void DropMainWeapon2()                                           //丢弃主武器
    {
        if (IsOwner)
        {
            if (hasMainWeapon2)
                MainWeapon2.GetComponent<RifleControl>().Drop();
            if (weaponState == ID_MAIN_WEAPON2)
                UnArm();
            hasMainWeapon2 = false;
        }
    }
    public void DropDeputyWeapon()                                           //丢弃主武器
    {
        if (IsOwner)
        {
            if (hasDeputyWeapon)
                DeputyWeapon.GetComponent<PistolControl>().Drop();
            if (weaponState == ID_DEPUTY_WEAPON)
                UnArm();
            hasDeputyWeapon = false;
        }
    }
    public void SetMainWeapon1(GameObject weapon)                           //设置主武器
    {
        if (IsOwner)
        {
            MainWeapon1 = weapon;
            RifleControl temp = MainWeapon1.GetComponent<RifleControl>();
            temp.SetPlayer(Self);
            MainWeapon1.SetActive(false);
            hasMainWeapon1 = true;
        }
    }
    public void SetMainWeapon1(Objects o)                                  //设置主武器
    {
        if (IsOwner)
        {
            if (!objectsAround.Contains(o))
                return;
            DropMainWeapon1();
            SetMainWeapon1(((MainWeapon)o.Data).CreateWeapon(transform));
            mainSystem.objects.Remove(o);
            objectsAround.Remove(o);
            Destroy(o.Self);
        }
    }
    public void SetMainWeapon2(GameObject weapon)                           //设置主武器
    {
        if (IsOwner)
        {
            MainWeapon2 = weapon;
            RifleControl temp = MainWeapon2.GetComponent<RifleControl>();
            temp.SetPlayer(Self);
            MainWeapon2.SetActive(false);
            hasMainWeapon2 = true;
        }
    }
    public void SetMainWeapon2(Objects o)                                  //设置主武器
    {
        if (!objectsAround.Contains(o))
            return;
        DropMainWeapon2();
        SetMainWeapon2(((MainWeapon)o.Data).CreateWeapon(transform));
        mainSystem.objects.Remove(o);
        objectsAround.Remove(o);
        Destroy(o.Self);
    }
    public void AddSilencer(int id, Objects o)                              //为武器添加消声器
    {
        if ((!objectsOwned.Contains(o)) && (!objectsAround.Contains(o)))
            return;
        if (!(o.Data is Silenser))
            return;
        GameObject temp;
        switch (id)
        {
            case ID_MAIN_WEAPON1:
                if (!hasMainWeapon1)
                    return;
                temp = ((Silenser)o.Data).CreateSilenser(MainWeapon1.GetComponent<RifleControl>().SilencerPos);
                if (MainWeapon1.GetComponent<RifleControl>().AddSilencer(temp))
                {
                    objectsAround.Remove(o);
                    objectsOwned.Remove(o);
                    mainSystem.objects.Remove(o);
                    Destroy(o.Self);
                }
                else
                    Destroy(temp);
                break;
            case ID_MAIN_WEAPON2:
                if (!hasMainWeapon2)
                    return;
                temp = ((Silenser)o.Data).CreateSilenser(MainWeapon2.GetComponent<RifleControl>().SilencerPos);
                if (MainWeapon2.GetComponent<RifleControl>().AddSilencer(temp))
                {
                    objectsAround.Remove(o);
                    objectsOwned.Remove(o);
                    mainSystem.objects.Remove(o);
                    Destroy(o.Self);
                }
                else
                    Destroy(temp);
                break;
        }
    }
    public void AddOptic(int id, Objects o)                              //为武器添加瞄准镜
    {
        if ((!objectsOwned.Contains(o)) && (!objectsAround.Contains(o)))
            return;
        if (!(o.Data is Optic))
            return;
        GameObject temp;
        switch (id)
        {
            case ID_MAIN_WEAPON1:
                if (!hasMainWeapon1)
                    return;
                temp = ((Optic)o.Data).CreateOptic(MainWeapon1.GetComponent<RifleControl>().OpticPos);
                if (MainWeapon1.GetComponent<RifleControl>().AddOptic(temp))
                {
                    objectsAround.Remove(o);
                    objectsOwned.Remove(o);
                    mainSystem.objects.Remove(o);
                    Destroy(o.Self);
                }
                else
                    Destroy(temp);
                break;
            case ID_MAIN_WEAPON2:
                if (!hasMainWeapon2)
                    return;
                temp = ((Optic)o.Data).CreateOptic(MainWeapon2.GetComponent<RifleControl>().OpticPos);
                if (MainWeapon2.GetComponent<RifleControl>().AddOptic(temp))
                {
                    objectsAround.Remove(o);
                    objectsOwned.Remove(o);
                    mainSystem.objects.Remove(o);
                    Destroy(o.Self);
                }
                else
                    Destroy(temp);
                break;
        }
    }
    public void SetDeputyWeapon(GameObject weapon)                           //设置主武器
    {
        DeputyWeapon = weapon;
        PistolControl temp = DeputyWeapon.GetComponent<PistolControl>();
        temp.SetPlayer(Self);
        DeputyWeapon.SetActive(false);
        hasDeputyWeapon = true;
    }
    public void Aim()                                                        //瞄准
    {
        aimState = ID_AIM;
        switch (bodyState)
        {
            case ID_STAND:
                CameraControl.ChangePosTo(ID_STAND_AIM);
                break;
            case ID_PRONE:
                CameraControl.ChangePosTo(ID_PRONE_AIM);
                break;
            case ID_CROUCH:
                CameraControl.ChangePosTo(ID_CROUCH_AIM);
                break;
        }
        RotationSmoothness = 1;
    }
    public void Crouch()                                                  //蹲下
    {
        if (IsOwner)
        {
            CameraControl.ChangePosTo(aimState == ID_AIM ? ID_CROUCH_AIM : ID_CROUCH_UNAIM);
            RotationSmoothness = 0.2f;
            bodyState = ID_CROUCH;
            animator.SetInteger(bodyStateID, bodyState);
        }
    }
    public void Prone()                                                  //趴下
    {
        if (IsOwner)
        {
            CameraControl.ChangePosTo(aimState == ID_AIM ? ID_PRONE_AIM : ID_PRONE_UNAIM);
            RotationSmoothness = 0.2f;
            bodyState = ID_PRONE;
            animator.SetInteger(bodyStateID, bodyState);
        }
    }
    public void StandUp()                                               //起立
    {
        if (IsOwner)
        {
            CameraControl.ChangePosTo(aimState == ID_AIM ? ID_STAND_AIM : ID_STAND_UNAIM);
            RotationSmoothness = 0.2f;
            bodyState = ID_STAND;
            animator.SetInteger(bodyStateID, bodyState);
        }
    }
    public void OpticShift()                                             //切换瞄准状态
    {
        CameraControl.ChangePosTo(InOptic ? 1 : 2);
        InOptic = !InOptic;
    }
    public void UnAim()                                                //解除瞄准
    {
        if (IsOwner)
        {
            aimState = ID_UNAIM;
            switch (bodyState)
            {
                case ID_STAND:
                    CameraControl.ChangePosTo(ID_STAND_UNAIM);
                    break;
                case ID_PRONE:
                    CameraControl.ChangePosTo(ID_PRONE_UNAIM);
                    break;
                case ID_CROUCH:
                    CameraControl.ChangePosTo(ID_CROUCH_UNAIM);
                    break;
            }
            RotationSmoothness = 0.2f;
        }
    }
    //AI接口结束

    private void RotationSync()
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(new Vector3(0, CameraOrigin.eulerAngles.y, 0)), RotationSmoothness);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit1");
        if (collision.gameObject.tag == "bullet")
            Debug.Log("Hit");
    }

    //IK,在不同状态的时候需要进行一些调整，关闭或者打开IK
    private void OnAnimatorIK(int layerIndex)
    {
        if (IsOwner)
        {
            if (!animator.GetCurrentAnimatorStateInfo(2).IsName("Unarmed"))  //动画层第三层arm中的换武器、换弹用IK都不自然，因此发生时关闭IK
            {
                Debug.Log("not unarm");
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                return;
            }
            if (animator.GetFloat(AimStateID) > 0.9f)                      //当且仅当有武器并且在瞄准时，让左手匹配到武器的位置（右手在RifleControl中一直是绑定的）
            {
                Debug.Log("left");
                animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftIKPos.position);
                //         animator.SetIKPosition(AvatarIKGoal.RightHand, RightIKPos.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftIKPos.rotation);
                //        animator.SetIKRotation(AvatarIKGoal.RightHand, RightIKPos.rotation);
                IKchangeFactor = Mathf.Lerp(IKchangeFactor, 1, Time.deltaTime * IKchangeSmoothness);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKchangeFactor);
                //        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IKchangeFactor);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IKchangeFactor);
                //        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, IKchangeFactor);
            }
        }

        /*      
              switch (layerIndex)
              {
                  case 0:
                      break;
                  case 1:

                      else
                          IKchangeFactor = 0;
                      break;
                  case 2:
                      break;
                  case 3:
                      animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                      animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                      break;
                  default:
                      break;
              }*/

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "objects")//周边物品更新
            objectsAround.Add(other.gameObject.GetComponent<Objects>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "objects")//周边物品更新
            objectsAround.Remove(other.gameObject.GetComponent<Objects>());
    }
}
