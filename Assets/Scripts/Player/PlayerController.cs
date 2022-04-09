using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

/* 控制玩家的大多数动作
 * 其他的控制主要在枪械的脚本及Healthcontrol中
 * 关于人类玩家的物品拾取则写在UIsystemcontroller中*/
[System.Serializable]
public class PlayerController : NetworkBehaviour
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
    const int ID_UNARMED = 0;
    const int ID_UNAIM = 0;
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
            animator.SetFloat(VerticalID, CurrentAxisState.y);
            animator.SetFloat(HorizontalID, CurrentAxisState.x);
        }
    }
    /*
     * 所有的输入检测都应当放在此处，并调用函数，保证角色所有的动作都可以在无用户输入的条件下进行
     */
    private void InputDetection()                     //检测用户的输入，每帧调用，调用的函数在AI接口中
    {

        if (bodyState == ID_STAND && Input.GetKey(KeyCode.LeftShift))
        {
            SetSpeedServerRpc(Input.GetAxis("Horizontal") * 2, Input.GetAxis("Vertical") * 2);
        }
        else
        {
            SetSpeedServerRpc(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
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

        //Weapon Index
        Debug.Log("update");
        Debug.Log("delta time:" + Time.deltaTime);

        CurrentAxisState = Vector2.Lerp(CurrentAxisState, TargetAxisState.Value, TransitionSmoothness / (Vector2.Distance(CurrentAxisState, TargetAxisState.Value) * 10 + 1));
        if (IsServer)
        {
            Debug.Log("CurrentAxis: " + CurrentAxisState.x + " " + CurrentAxisState.y);
            Debug.Log("speed:" + speed);
            GetComponent<CharacterController>().SimpleMove(
                (transform.TransformDirection(Vector3.forward) * CurrentAxisState.y +
                transform.TransformDirection(Vector3.right) * CurrentAxisState.x) * speed
                );

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
            if (IsOwner)
            {
                animator.SetBool("Dead", true);
            }
            mainSystem.players.Remove(this);
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

    public void LookAt(Transform target)                                        //正对着
    {
        CameraOrigin.LookAt(target);
    }

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
