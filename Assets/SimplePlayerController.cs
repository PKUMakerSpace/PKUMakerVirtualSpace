using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

/* ������ҵĴ��������
 * �����Ŀ�����Ҫ��ǹе�Ľű���Healthcontrol��
 * ����������ҵ���Ʒʰȡ��д��UIsystemcontroller��*/
[System.Serializable]
public class SimplePlayerController : NetworkBehaviour
{

    public Transform CameraOrigin;                               //ӵ��camera��һЩ����
    public Transform LeftHand;                                   //����λ������
    public Transform RightHand;                                  //����λ������

    private Transform LeftIKPos;                                 //����IKλ�����ã�λ����ǹ��
    private Transform RightIKPos;                                //����IKλ�����ã�λ����ǹ��

    public GameObject Camera;                                    //�������
    public GameObject Self;                                      //��������
    public GameObject canvas;                                    //��ƷUIϵͳ����
    public GameObject MainWeapon1;                               //������1����
    public GameObject MainWeapon2;                               //������2����
    public GameObject DeputyWeapon;                              //����������ǹ������


    public List<Objects> objectsAround;                       //��Χ����
    public List<Objects> objectsOwned;                        //����������

    public PlayerSystem mainSystem;

    public Text WeaponShow;

    PCamera CameraControl;                                       //Main Camera��script����

    public Animator animator;                                 //����animator����

    //Animator Paras
    private int VerticalID = Animator.StringToHash("KeyboardInputVertical");               //��animator��parameters��ID��ȡ����������֮�����
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
    private Vector2 CurrentAxisState;                                   //Ŀǰ��λ����Ϣ
    private NetworkVariable<Vector2> TargetAxisState = new NetworkVariable<Vector2>(new Vector2(0, 0));                                    //Ŀ��λ����Ϣ��������Ϊ��ƽ���˶��Ĳ�ֵ��������ģ�����Ҫֱ���޸ģ�������޸��˾ͻᳯ���޸ĵķ���ı�animator
    private float TransitionSmoothness = 1;

    //Rrotate
    private float RotationSmoothness = 0.2f;

    //State_const
    const int ID_STAND = 0;                                            //������������д���൱��C��#define
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
    //״̬
    public int aimState { get; set; }                                           //��׼״̬��ID_AIM��ʾ��׼,ID_UNAIM��ʾδ��׼
    public int weaponState { get; set; }                                        //����״̬��ID_UNARMED��ʾ��������ID_MAIN_WEAPON1��ʾ��һ��������
    public int bodyState { get; set; }                                          //����״̬��ID_STAND��ʾվ����ID_CROUCH��ʾ�¶ף�ID_PRONE��ʾſ��
    private float isAiming;  //0 -> normal; 1 -> aimState           //���ڲ�ֵ����
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

    public int Mode = 0;    //0->��ҿ��ƣ�1->AI����

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            Camera.SetActive(false);
        }
    }

    // Use this for initialization                              ��ʼ��
    private void Start()
    {
        animator = this.GetComponent<Animator>();
        CurrentAxisState = new Vector2(0, 0);
        if (IsServer)
        {
            TargetAxisState.Value = new Vector2(0, 0);
        }

        //״̬��ʼ��
        aimState = ID_UNAIM;
        bodyState = ID_STAND;
        weaponState = ID_UNARMED;

        CameraControl = Camera.GetComponent<PCamera>();
    }

    // Update is called once per frame

    private void Standing()                              //վ��״̬ÿ֡ʱ����
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
    private void Crouching()                          //�¶�״̬ʱÿ֡����
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
    private void Proning()                           //ſ��״̬ʱÿ֡����
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
     * ���е������ⶼӦ�����ڴ˴��������ú�������֤��ɫ���еĶ��������������û�����������½���
     */
    private void InputDetection()                     //����û������룬ÿ֡���ã����õĺ�����AI�ӿ���
    {

        if (bodyState == ID_STAND && Input.GetKey(KeyCode.LeftShift))
        {
            SetSpeedServerRpc(Input.GetAxis("Horizontal") * 2 * speed, Input.GetAxis("Vertical") * 2 * speed);
        }
        else
        {
            SetSpeedServerRpc(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
        }
        //��Ʒ����O
        if (Input.GetKeyDown(KeyCode.O))
        {
            UIOn = !UIOn;
            canvas.SetActive(UIOn);
        }
        //������1,1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (hasMainWeapon1)
                MainWeapon1On();
        }
        //�л�����ģʽ��T
        if (Input.GetKeyDown(KeyCode.T))
            SwitchMode();
        //װ�R
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        //������2,2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (hasMainWeapon2)
                MainWeapon2On();
        }
        //��������3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasDeputyWeapon)
                DeputyWeaponOn();
        }
        //����������X
        if (Input.GetKeyDown(KeyCode.X))
            UnArm();
        //���£������л�����ctrl
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (bodyState != ID_CROUCH)
                Crouch();
            else if (bodyState == ID_CROUCH)
                StandUp();
        }
        //ſ�£������л���Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (bodyState != ID_PRONE)
                Prone();
            else if (bodyState == ID_PRONE)
                StandUp();
        }
        //��׼���Ҽ�
        if (weaponState != ID_UNARMED)
        {
            if (Input.GetMouseButtonDown(1))
                Aim();
            if (Input.GetMouseButtonUp(1))
                UnAim();
        }
        //��׼ģʽ�뿪��
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
    public void AIControl()//����AI����д�ڴ˴�����
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

    //AI�ӿڣ������������Ķ�������Ҫ�����е�һЩ�����ĳ�Э��
    [ServerRpc]
    public void SetSpeedServerRpc(float Horizontal, float Vertical)                    //����vertical��horizontal�ٶ�
    {
        /*if (Mathf.Abs(Horizontal) > 1 || Mathf.Abs(Vertical) > 1)
        {
            Debug.Log("�������õ��ٶ�ֻ����0��1֮��");
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
    public bool AddObject(Objects o)                                        //ʰȡ��Ʒ
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
    /*public void Turn(int DeltaX,int DeltaY)                                     //ת��
    {
        CameraOrigin.GetComponent<PCameraOrigin>().mouseRotation+= new Vector2(DeltaY, DeltaX);
    }*/
    public void LookAt(Transform target)                                        //������
    {
        CameraOrigin.LookAt(target);
    }
    public void Reload()                                                         //װ��
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
    public void DropObject(Objects o)                                          //������Ʒ
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
    public void SwitchMode()                                                 //�л�����ģʽ
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
    /*RifleOn Down,PistolOn Down�ĸ����������player����arms��ģ�
     * �ж�Ӧ������װ����������ʱ����Ϊtrue��֮��ͨ��animator�е�behavior���Զ����false*/
    public void MainWeapon1On()                                              //װ��������
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
    public void MainWeapon2On()                                              //װ��������
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
    public void DeputyWeaponOn()                                              //װ��������
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
    public void Fire()                                                      //����
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
    public void UnArm()                                                      //�����װ
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
    public void DropMainWeapon1()                                           //����������
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
    public void DropMainWeapon2()                                           //����������
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
    public void DropDeputyWeapon()                                           //����������
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
    public void SetMainWeapon1(GameObject weapon)                           //����������
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
    public void SetMainWeapon1(Objects o)                                  //����������
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
    public void SetMainWeapon2(GameObject weapon)                           //����������
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
    public void SetMainWeapon2(Objects o)                                  //����������
    {
        if (!objectsAround.Contains(o))
            return;
        DropMainWeapon2();
        SetMainWeapon2(((MainWeapon)o.Data).CreateWeapon(transform));
        mainSystem.objects.Remove(o);
        objectsAround.Remove(o);
        Destroy(o.Self);
    }
    public void AddSilencer(int id, Objects o)                              //Ϊ�������������
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
    public void AddOptic(int id, Objects o)                              //Ϊ���������׼��
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
    public void SetDeputyWeapon(GameObject weapon)                           //����������
    {
        DeputyWeapon = weapon;
        PistolControl temp = DeputyWeapon.GetComponent<PistolControl>();
        temp.SetPlayer(Self);
        DeputyWeapon.SetActive(false);
        hasDeputyWeapon = true;
    }
    public void Aim()                                                        //��׼
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
    public void Crouch()                                                  //����
    {
        if (IsOwner)
        {
            CameraControl.ChangePosTo(aimState == ID_AIM ? ID_CROUCH_AIM : ID_CROUCH_UNAIM);
            RotationSmoothness = 0.2f;
            bodyState = ID_CROUCH;
            animator.SetInteger(bodyStateID, bodyState);
        }
    }
    public void Prone()                                                  //ſ��
    {
        if (IsOwner)
        {
            CameraControl.ChangePosTo(aimState == ID_AIM ? ID_PRONE_AIM : ID_PRONE_UNAIM);
            RotationSmoothness = 0.2f;
            bodyState = ID_PRONE;
            animator.SetInteger(bodyStateID, bodyState);
        }
    }
    public void StandUp()                                               //����
    {
        if (IsOwner)
        {
            CameraControl.ChangePosTo(aimState == ID_AIM ? ID_STAND_AIM : ID_STAND_UNAIM);
            RotationSmoothness = 0.2f;
            bodyState = ID_STAND;
            animator.SetInteger(bodyStateID, bodyState);
        }
    }
    public void OpticShift()                                             //�л���׼״̬
    {
        CameraControl.ChangePosTo(InOptic ? 1 : 2);
        InOptic = !InOptic;
    }
    public void UnAim()                                                //�����׼
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
    //AI�ӿڽ���

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

    //IK,�ڲ�ͬ״̬��ʱ����Ҫ����һЩ�������رջ��ߴ�IK
    private void OnAnimatorIK(int layerIndex)
    {
        if (IsOwner)
        {
            if (!animator.GetCurrentAnimatorStateInfo(2).IsName("Unarmed"))  //�����������arm�еĻ�������������IK������Ȼ����˷���ʱ�ر�IK
            {
                Debug.Log("not unarm");
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                return;
            }
            if (animator.GetFloat(AimStateID) > 0.9f)                      //���ҽ�����������������׼ʱ��������ƥ�䵽������λ�ã�������RifleControl��һֱ�ǰ󶨵ģ�
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
        if (other.tag == "objects")//�ܱ���Ʒ����
            objectsAround.Add(other.gameObject.GetComponent<Objects>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "objects")//�ܱ���Ʒ����
            objectsAround.Remove(other.gameObject.GetComponent<Objects>());
    }
}
