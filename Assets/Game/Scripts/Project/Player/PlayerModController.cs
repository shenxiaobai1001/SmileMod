using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerModController : MonoBehaviour
{
    public static PlayerModController Instance;
    public PlayerController playerController;
    public Rigidbody2D rigidbody;
    public BoxCollider2D box;
    public GameObject Center;
    public Transform spriteTrans;
    public GameObject BoomPre;
    public Transform createPos;

    string[] gaiYas=new string[7] {"bishi","chaodan","huotui","mifan","jidan","jirou","mifen"};

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {

        EventManager.Instance.AddListener(Events.OnQLMove, OnModMoveQL);
        EventManager.Instance.AddListener(Events.OnTCMove, OnModMoveTC);
        EventManager.Instance.AddListener(Events.OneFingerMove, OnModMoveFinger);
        StartCoroutine(OnCheckGround());
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            rigidbody.isKinematic = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = new Vector3(transform.position.x, 5);
        }
        if (Input.GetKey(KeyCode.L))
        {
            transform.Translate(Vector3.up * 15 * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            if (isPassivityMove <= 0)
            {
                rigidbody.isKinematic = false;
            }
        }
        if (canTomto)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject obj = SimplePool.Spawn(Tomato, transform.position, Quaternion.identity);
                obj.transform.parent = createPos;
                obj.SetActive(true);
                obj.GetComponent<Tomate>().OnStartMove();
                int index=Random.Range(0, 7);
                Sound.PlaySound($"Sound/Mod/fanqie{gaiYas[index]}");
            }
        }

   
    }
    IEnumerator OnCheckGround()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (transform.position.y < -4)
            {
                transform.position = new Vector3(transform.position.x, 0);
            }
        }
    }

    bool isHitPlayerBack = false;
    public float backTime = 0;
    bool canAddHitTime = true;

    bool isHitPlayerForward = false;
    public float ForwardTime = 0;
    bool canAddHitTime1 = true;

    int isPassivityMove = 0;
    IEnumerator OnHitPlayerBack()
    {
        while (backTime > 0)
        {
            if (transform.position.y < 5)
            {
                transform.Translate(Vector2.up * 8 * Time.deltaTime);
            }
            transform.Translate(Vector2.left * 16 * Time.deltaTime);
            backTime -= Time.deltaTime;
            yield return null;
        }
        isPassivityMove--;
        isHitPlayerBack = false;
        if (isPassivityMove <= 0)
        {
            backTime = 0;
            OnChangeState(true);
        }
    }
    public void OnLeftHitPlayer()
    {
        backTime += 0.1f;

        if (!isHitPlayerBack)
        {
            isPassivityMove++;
            OnChangeState(false);
            isHitPlayerBack = true;
            StartCoroutine(OnHitPlayerBack());
        }
    }
    public void OnRightHitPlayer()
    {
        ForwardTime += 0.1f;

        if (!isHitPlayerForward)
        {
            isPassivityMove++;
            OnChangeState(false);
            isHitPlayerForward = true;
            StartCoroutine(OnHitPlayerForward());
        }
    }


    IEnumerator OnHitPlayerForward()
    {
        while (ForwardTime > 0)
        {
            if (transform.position.y < 5)
            {
                transform.Translate(Vector2.up * 8 * Time.deltaTime);
            }
            transform.Translate(Vector2.right * 16 * Time.deltaTime);
            ForwardTime -= Time.deltaTime;
            yield return null;
        }
        isPassivityMove--;
        isHitPlayerForward = false;
        if (isPassivityMove <= 0)
        {
            backTime = 0;
            OnChangeState(true);
        }
    }

    public void OnKickPlayer(Vector3 force, bool boom = false)
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
        rigidbody.AddForce(force, ForceMode2D.Impulse);
        if (boom)
        {
            Instantiate(BoomPre, transform.position,Quaternion.identity);
        }
    }

    void OnChangeState(bool open)
    {
        box.enabled = open;
        Center.SetActive(open);
        playerController.isHit = !open;
        rigidbody.isKinematic = !open;
    }

    bool isCloaking = false;
    public float cloakingTime = 0;
    public void OnCloaking(DataInfo dataInfo)
    {
        cloakingTime += dataInfo.count * dataInfo.time;
    }

    #region Ìì²Ð½Å÷è÷ë±Û
    MoveType moveType = MoveType.None;
    bool isMove = false;
    bool toMove = false;
    Vector3 startPos = Vector3.zero;
    int tcCount = 0;
    int qlCount = 0;
    int fingerCount = 0;
    void OnModMoveTC(object msg)
    {
        bool show = (bool)msg;
        startPos = transform.position;
        if (show)
            tcCount++;
        else
            tcCount--;

        OnCheckMoveType();
        if (toMove)
        {
            OnChangeState(false);
            if (!isMove)
            {
                isPassivityMove++;
                if (rigidbody) rigidbody.isKinematic = true;
                isMove = true;
                StartCoroutine(OnModMove());
            }
        }
    }
    void OnModMoveFinger(object msg)
    {
        bool show = (bool)msg;
        startPos = transform.position;
        if (show)
            fingerCount++;
        else
            fingerCount--;

        OnCheckMoveType();
        if (toMove)
        {
            OnChangeState(false);
            if (!isMove)
            {
                isPassivityMove++;
                if (rigidbody) rigidbody.isKinematic = true;
                isMove = true;
                StartCoroutine(OnModMove());
            }
        }
    }
    void OnModMoveQL(object msg)
    {
        bool show = (bool)msg;
        startPos = transform.position;
        if (show)
            qlCount++;
        else
            qlCount--;

        OnCheckMoveType();
        if (toMove)
        {
            OnChangeState(false);
            if (!isMove)
            {
                isPassivityMove++;
                if (rigidbody) rigidbody.isKinematic = true;
                isMove = true;
                StartCoroutine(OnModMove());
            }
        }
    }
    void OnCheckMoveType()
    {
        if (tcCount > 0 && qlCount <= 0)
        {
            moveType = MoveType.TC;
        }
        else if (tcCount <= 0 && qlCount > 0)
        {
            moveType = MoveType.QL;
        }
        else if (tcCount > 0 && qlCount > 0)
        {
            moveType = MoveType.DuiKang;
        }
        else if (fingerCount>0)
        {
            moveType = MoveType.Finger;
        }
        else
        {
            moveType = MoveType.None;
        }
        toMove = qlCount > 0 || tcCount > 0|| fingerCount > 0;
    }
    IEnumerator OnModMove()
    {
        while (toMove)
        {
            switch (moveType)
            {
                case MoveType.None:
                    toMove = false;
                    yield return null;
                    break;
                case MoveType.DuiKang:
                    Camera.main.DOShakePosition(0.5f, new Vector3(2, 2, 2), 3, 50, true);
                    yield return new WaitForSeconds(0.7f);
                    transform.position = startPos;
                    yield return null;
                    break;
                case MoveType.TC:
                    transform.position += new Vector3(-1, 0.3f) * 20 * Time.deltaTime;
                    startPos = transform.position;
                    yield return null;
                    break;
                case MoveType.Finger:
                    transform.position += new Vector3(-1, 0.1f) * 30 * Time.deltaTime;
                    startPos = transform.position;
                    yield return null;
                    break;
                case MoveType.QL:
                    transform.position += new Vector3(1, 0.3f) * 20 * Time.deltaTime;
                    startPos = transform.position;
                    yield return null;
                    break;
            }
        }
        isPassivityMove--;
        if (rigidbody) rigidbody.isKinematic = false;
        isMove = false;
        if(isPassivityMove<=0)
            OnChangeState(true);
    }
    enum MoveType
    {
        None,
        DuiKang,
        TC,
        QL,
        Finger,
    }
    #endregion

    bool isBigBetaForward = false;
    bool isBigBetaBack = false;
    public float BigBetaTime = 17;
    public float BigBetaBackTime = 17;
    public void OnBigBetaForward(bool forward)
    {
        if (forward)
        {
            if (!isBigBetaForward)
            {
                isPassivityMove++;
                OnChangeState(false);
                isBigBetaForward = true;
                StartCoroutine(BigBetaForward());
            }
        }
        else 
        {
            if (!isBigBetaBack)
            {
                isPassivityMove++;
                OnChangeState(false);
                isBigBetaBack = true;
                StartCoroutine(BigBetaBack());
            }
        }
    }

    IEnumerator BigBetaForward()
    {
        while (BigBetaTime > 0)
        {
            if (transform.position.y < 5)
            {
                transform.Translate(Vector2.up * 4 * Time.deltaTime);
            }
            transform.Translate(Vector2.right * 8 * Time.deltaTime);
            spriteTrans.Rotate(new Vector3(0,0,360)*10* Time.deltaTime);
            BigBetaTime -= Time.deltaTime;
            yield return null;
        }
        isBigBetaForward = false;
        BigBetaTime = 17;
        isPassivityMove--;
        spriteTrans.localEulerAngles = Vector3.zero;
        if (isPassivityMove<=0)
            OnChangeState(true);
    }
    IEnumerator BigBetaBack()
    {
        while (BigBetaBackTime > 0)
        {
            if (transform.position.y < 5)
            {
                transform.Translate(Vector2.up * 4 * Time.deltaTime);
            }
            transform.Translate(Vector2.left * 8 * Time.deltaTime);
            spriteTrans.Rotate(new Vector3(0, 0, -360) * 10 * Time.deltaTime);
            BigBetaBackTime -= Time.deltaTime;
            yield return null;
        }
        spriteTrans.localEulerAngles = Vector3.zero;
        isBigBetaBack = false;
        BigBetaTime = 17;
        isPassivityMove--;
        if (isPassivityMove <= 0)
            OnChangeState(true);
    }


    public GameObject Tomato;
    bool canTomto = false;
    float tomateTime = 0;
    public void OnClickToCreateTomaTo(DataInfo data)
    {
        tomateTime += data.count * data.time;
        if (!canTomto)
        {
            Sound.PlaySound("Sound/Mod/gaiya");
            EventManager.Instance.SendMessage(Events.GaiyaTomato);
            Invoke("OnReadyTomato",3);
        }
    }
    void OnReadyTomato()
    {
        canTomto = true;
        StartCoroutine(onCheckTomato());
    }
    IEnumerator onCheckTomato()
    {
        while (tomateTime>0)
        {
            tomateTime-=Time.deltaTime;
            yield return null;
        }
        EventManager.Instance.SendMessage(Events.GaiyaTomatoEnd);
        canTomto = false;
        tomateTime = 0;
    }
    #region ÒþÉí
    bool invisibility = false;
    float visibilityTime = 0;
    public void OnInvisibility(DataInfo dataInfo)
    {
        visibilityTime += dataInfo.count * dataInfo.time; ;
        if (!invisibility) {
            invisibility = true;
            spriteTrans.gameObject.SetActive(false);
            StartCoroutine(OnCheckVisibility());
        }
    }
    IEnumerator OnCheckVisibility()
    {
        while (visibilityTime > 0)
        {
            visibilityTime -= Time.deltaTime;
            yield return null;
        }
        spriteTrans.gameObject.SetActive(true);
        invisibility = false;
        visibilityTime = 0;
    }
    #endregion

    bool fastSpeed = false;
    float fastSpeedTime = 0;
    public void OnFastSpeed(DataInfo dataInfo)
    {
        fastSpeedTime += dataInfo.count * 10;
        PlayerData.Instance.moveSpeed += 5;
        PlayerData.Instance.fmoveSpeed += 5;
        if (!fastSpeed)
        {
            fastSpeed = true;
            StartCoroutine(OnCheckFastSpeed());
        }
    }
    public void OnMainSpeed(DataInfo dataInfo)
    {
        fastSpeedTime += dataInfo.count * 10;
        PlayerData.Instance.moveSpeed -= 5;
        PlayerData.Instance.fmoveSpeed -= 5;
        if (PlayerData.Instance.moveSpeed < 0)
        {
            PlayerData.Instance.moveSpeed = 1;
        }
        if (PlayerData.Instance.fmoveSpeed < 0)
        {
            PlayerData.Instance.fmoveSpeed = 1;
        }
        if (!fastSpeed)
        {
            fastSpeed = true;
            StartCoroutine(OnCheckFastSpeed());
        }
    }
    IEnumerator OnCheckFastSpeed()
    {
        while (fastSpeedTime > 0)
        {
            fastSpeedTime -= Time.deltaTime;
            yield return null;
        }
        PlayerData.Instance.moveSpeed = 6.5f;
        PlayerData.Instance.fmoveSpeed = 12.4f;
        fastSpeed = false;
        fastSpeedTime = 0;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnTCMove, OnModMoveTC);
        EventManager.Instance.RemoveListener(Events.OnQLMove, OnModMoveQL);
    } 

}
