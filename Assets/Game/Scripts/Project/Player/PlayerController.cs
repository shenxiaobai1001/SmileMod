using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public Animator animator;
    public Transform spriteTrans;
    public Rigidbody2D rigidbody2D;
    public BoxCollider2D boxCollider;
    public PlayerCheckGround playerCheckGround; 
    public PlayerCheckGround playerCheckRight; 
    public PlayerCheckGround playerCheckLeft;
    public PlayerCheckGround playerCheckHit;
    public PlayerCheckGround playerCheckUpHit;

    public GameObject runRightEffect;
    public GameObject runLeftEffect;

    public GameObject boostRightEffect;
    public GameObject boostLeftEffect;
    public GameObject horHitEffect;
    public GameObject vecHitEffect;

    public GameObject dizzard;
    public Transform createPos;

    float moveSpeed = PlayerData.Instance.moveSpeed;  // 普通移动速度
    float fmoveSpeed = PlayerData.Instance.fmoveSpeed;      // 快速移动速度
    float acceleration = 16;    // 加速系数
    float deceleration = 25;    // 减速系数
    float currentSpeed = 0f;     // 当前水平速度
    Vector3 moveVec = Vector3.zero;    // 移动方向、

    // 跳跃相关参数
    public float jumpSpeed = 10f;          // 跳跃上升速度
    float JumpHeight = 0;        // 最大跳跃高度
    float FastJumpHeight = 7;        // 最大跳跃高度
    float normalJumpHeight = 5;        // 最大跳跃高度
    float targetSpeed = 0;
    float jumpForce = 12f;        // 跳跃初速度
    float FastjumpForce = 14;        // 跳跃初速度
    PlayerControState playerControState= PlayerControState.None;
    public PLState pLState;
    PLState oldState = PLState.None;

    bool jPressed = false;
    bool aPressed = false;
    bool dPressed = false; 
    public bool isCheckVec = false;

    public float keyCombinationWindow = 0.1f; // 允许的时间差

    float stickFTime = 0;
    float brakeTime = 0;
    float damageTime = 0;
    public bool isHit = false;

    public BoostImageContro boostImageContro;
    public PCteateHit pCteateHit;
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
        if (dizzard) dizzard.SetActive(false);
        pLState = PLState.Idel;
        StartCoroutine(OnCreateEffect());
        StartCoroutine(OnCreateBoostEffect());
        StartCoroutine(OnCreateHorHitEffect());
        StartCoroutine(OnCreateVecHitEffect());
    }
    void Update()
    {
        OnClickState();
        OnPlayerMove();
    }

    public void OnRest()
    {
        if (dizzard) dizzard.SetActive(false);
        moveVec = Vector3.zero;
        currentSpeed = 0;
        pLState = PLState.Idel;
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("BrakeF", false);
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Brake", false);
            animator.SetBool("Stick", false);
            animator.SetBool("StickF", false);
            animator.SetBool("Boost", false);
            animator.SetBool("DamageOver", false);
        }
        gameObject.SetActive(true);
        isHit = false;
    }

    void OnClickState()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            EventManager.Instance.SendMessage(Events.PlayerRestToSavePos);
            EventManager.Instance.SendMessage(Events.GameRest);
        }
        if (GameController.Instance.isAutomatic) return;
        if (isHit) return;
   
        //if (Input.GetKey(KeyCode.D))
        //{
        //    OnPlayerControStateChange(PlayerControState.RRuning);
        //}
        //if (Input.GetKeyUp(KeyCode.D))
        //{
        //    dPressed = false;
        //    OnPlayerControStateChange(PlayerControState.CanelRRun);
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    aPressed = true;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    OnPlayerControStateChange(PlayerControState.LRuning);
        //}

        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    aPressed = false;
        //    OnPlayerControStateChange(PlayerControState.CanelLRun);
        //}

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnPlayerControStateChange(PlayerControState.ToFast);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            OnPlayerControStateChange(PlayerControState.Fast);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            OnPlayerControStateChange(PlayerControState.CancelFast);
        }

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    OnPlayerControStateChange(PlayerControState.Boost);
        //}

        //if (Input.GetKeyUp(KeyCode.K))
        //{
        //    OnPlayerControStateChange(PlayerControState.CanelBoost);
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    jPressed = true;
        //}
        //if (Input.GetKey(KeyCode.J))
        //{
        //    OnPlayerControStateChange(PlayerControState.Jumping);
        //}
        //if (Input.GetKeyUp(KeyCode.J))
        //{
        //    jPressed = false;
        //    OnPlayerControStateChange(PlayerControState.CJump);
        //}

        //if (jPressed)
        // {
        //     //PFunc.Log("单独跳跃");
        //     OnPlayerControStateChange(PlayerControState.Jump);
        //     jPressed=false;
        // }
        //else if (aPressed)
        //{
        //    //PFunc.Log("单独左移");
        //    OnPlayerControStateChange(PlayerControState.LRun);
        //    aPressed=false;
        //}
        //else if (dPressed)
        //{
        //    //PFunc.Log("单独右移");
        //    OnPlayerControStateChange(PlayerControState.RRun);
        //    dPressed = false;
        //}
    }

    public void OnPlayerControStateChange(PlayerControState nplayerCState)
    {
        playerControState = nplayerCState;
       // PFunc.Log("OnPlayerControStateChange", playerControState);
        switch (playerControState)
        {
            case PlayerControState.LRun://按下A键
                switch (pLState)
                {
                    case PLState.Idel:
                        if (animator)
                        {
                            animator.SetBool("Jump", false);
                            animator.SetBool("Run", true);
                        }
                        targetSpeed = moveSpeed;
                        OnLRun();
                        pLState = PLState.LRun;
                        break;
                    case PLState.RRun:
                        targetSpeed = moveSpeed;
                        OnLRun();
                        pLState = PLState.LRun;
                        break;
                    case PLState.FastRRun:
                        targetSpeed = fmoveSpeed;
                        currentSpeed = 0;
                        moveVec = Vector3.left;
                        spriteTrans.localScale = new Vector3(-1, 1, 1);
                        if (animator)
                        {
                            animator.SetBool("BrakeF", false);
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", true);
                            animator.SetBool("Brake", true);
                        }
                        brakeTime = 0;
                        pLState = PLState.FLBrake;
                        break;
                    case PLState.BrakeF:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("BrakeF", false);
                        }
                        OnLRun();
                        pLState = PLState.LRun;
                        break;
                    case PLState.Jump:
                    case PLState.RJumping:
                        OnLRun();
                        pLState = PLState.LJumping;
                        break;
                    case PLState.Drop:
                    case PLState.RDroping:
                        OnLRun();
                        pLState = PLState.LDroping;
                        break;
                    case PLState.Stick:
                        if (spriteTrans.localScale.x == -1) return;
                        OnLRun();
                        if (animator)
                        {
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", true);
                        }
                        rigidbody2D.gravityScale = 3.5f;
                        pLState = PLState.LDroping;
                        break;
                    case PLState.RStick:
                        OnLRun();
                        if (animator)
                        {
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", true);
                        }
                        rigidbody2D.gravityScale = 3.5f;
                        pLState = PLState.LDroping;
                        break;
                }
                break;
            case PlayerControState.LRuning://按住A键
                break;
            case PlayerControState.CanelLRun://松开A键
                switch (pLState)
                {
                    case PLState.LRun:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        if (animator) animator.SetBool("Run", false);
                        pLState = PLState.Idel;
                        break;
                    case PLState.FastLRun:
                        if (animator)
                        {
                            animator.SetBool("Brake", false);
                            animator.SetBool("Run", false);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("BrakeF", true);
                        }
                        pLState = PLState.BrakeF;
                        break;
                    case PLState.LJumping:
                    case PLState.FLJumping:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        pLState = PLState.Jump;
                        break;
                    case PLState.LDroping:
                    case PLState.FLDroping:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        pLState = PLState.Drop;
                        break;
                    case PLState.FLBrake:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("BrakeF", false);
                            animator.SetBool("Brake", false);
                            animator.SetBool("Jump", false);
                        }
                        pLState = PLState.Idel;
                        break;
                    case PLState.LStick:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        pLState = PLState.Stick;
                        break;
                }
                break;
            case PlayerControState.RRun://按下D键
                switch (pLState)
                {
                    case PLState.Idel:
                        if (animator)
                        {
                            animator.SetBool("Jump", false);
                            animator.SetBool("Run", true);
                        }
                        OnRRun();
                        pLState = PLState.RRun;
                        break;
                    case PLState.LRun:
                        OnRRun();
                        pLState = PLState.RRun;
                        break;
                    case PLState.FastLRun:
                        currentSpeed = 0;
                        moveVec = Vector3.right;
                        spriteTrans.localScale = new Vector3(1, 1, 1);
                        if (animator)
                        {
                            animator.SetBool("BrakeF", false);
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", true);
                            animator.SetBool("Brake", true);
                        }
                        brakeTime = 0;
                        pLState = PLState.FRBrake;
                        break;
                    case PLState.BrakeF:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("BrakeF", false);
                        }
                        OnRRun();
                        pLState = PLState.RRun;
                        break;
                    case PLState.Jump:
                    case PLState.LJumping:
                        OnRRun();
                        pLState = PLState.RJumping;
                        break;
                    case PLState.Drop:
                    case PLState.LDroping:
                        OnRRun();
                        pLState = PLState.RDroping;
                        break;
                    case PLState.Stick:
                        if (spriteTrans.localScale.x == 1) return;
                        OnRRun();
                        if (animator)
                        {
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", true);
                        }
                        rigidbody2D.gravityScale = 3.5f;
                        pLState = PLState.RDroping;
                        break; 
                    case PLState.LStick:
                        OnRRun();
                        if (animator)
                        {
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", true);
                        }
                        rigidbody2D.gravityScale = 3.5f;
                        pLState = PLState.RDroping;
                        break;
                }
                break;
            case PlayerControState.RRuning:
                break;
            case PlayerControState.CanelRRun://松开D键
                switch (pLState)
                {
                    case PLState.RRun:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        if (animator) animator.SetBool("Run", false);
                        pLState = PLState.Idel;
                        break;
                    case PLState.FastRRun:
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("BrakeF", true);
                            animator.SetBool("Brake", false);
                        }
                        pLState = PLState.BrakeF;
                        break;
                    case PLState.RJumping:
                    case PLState.FRJumping:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        pLState = PLState.Jump;
                        break;
                    case PLState.RDroping:
                    case PLState.FRDroping:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        pLState = PLState.Drop;
                        break;
                    case PLState.FRBrake:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("BrakeF", false);
                            animator.SetBool("Brake", false);
                            animator.SetBool("Jump", false);
                        }
                        pLState = PLState.Idel;
                        break;
                    case PLState.RStick:
                        currentSpeed = 0;
                        moveVec = Vector3.zero;
                        pLState = PLState.Stick;
                        break;
                }
                break;
            case PlayerControState.ToFast://按下加速
                switch (pLState)
                {
                    case PLState.LRun:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", true);
                        }
                        targetSpeed = fmoveSpeed;
                        pLState = PLState.FastLRun;
                        break;
                    case PLState.RRun:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", true);
                        }
                        targetSpeed = fmoveSpeed;
                        pLState = PLState.FastRRun;
                        break;
                }
                break;
            case PlayerControState.Fast:
                switch (pLState)
                {
                    case PLState.LRun:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", true);
                        }
                        targetSpeed = fmoveSpeed;
                        pLState = PLState.FastLRun;
                        break;
                    case PLState.RRun:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", true);
                        }
                        targetSpeed = fmoveSpeed;
                        pLState = PLState.FastRRun;
                        break;
                }
                break;
            case PlayerControState.CancelFast:
                switch (pLState)
                {
                    case PLState.FastLRun:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", false);
                        }
                        targetSpeed = moveSpeed;
                        currentSpeed = moveSpeed;
                        pLState = PLState.LRun;
                        break;
                    case PLState.FastRRun:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", false);
                        }
                        OnRRun();
                        targetSpeed = moveSpeed;
                        currentSpeed = moveSpeed;
                        pLState = PLState.RRun;
                        break;
                    case PLState.FLBrake:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("Brake", false);
                        }
                        OnLRun();
                        pLState = PLState.LRun;
                        break;
                    case PLState.FRBrake:
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("Brake", false);
                        }
                        OnRRun();
                        pLState = PLState.RRun;
                        break;
                }
                break;
            case PlayerControState.Jump://按下跳跃
                rigidbody2D.gravityScale = 3.5f;
                stickFTime = 0;
                switch (pLState)
                {
                    case PLState.Stick:
                        moveVec = spriteTrans.localScale.x == -1 ? new Vector3(1,1): new Vector3(-1, 1);
                        currentSpeed = 6;
                        JumpHeight = normalJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", false);
                            animator.SetBool("StickF", true);
                        }
                        pLState = PLState.StickF;
                        break;
                    case PLState.LStick:
                        moveVec = new Vector3(1, 1);
                        currentSpeed = 6;
                        JumpHeight = normalJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", false);
                            animator.SetBool("StickF", true);
                        }
                        pLState = PLState.LStickF;
                        break;
                    case PLState.RStick:
                        moveVec = new Vector3(-1, 1);
                        currentSpeed = 6;
                        JumpHeight = normalJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("Stick", false);
                            animator.SetBool("Drop", false);
                            animator.SetBool("StickF", true);
                        }
                        pLState = PLState.RStickF;
                        break;
                }
                if (!playerCheckGround.isGround) return;
                OnJump();
                switch (pLState)
                {
                    case PLState.None:
                        break;
                    case PLState.Idel:
                        JumpHeight = normalJumpHeight;
                        if (animator) animator.SetBool("Jump", true);
                        pLState = PLState.Jump;
                        break;
                    case PLState.LRun:
                        JumpHeight = normalJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.LJumping;
                        break;
                    case PLState.RRun:
                        JumpHeight = normalJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.RJumping;
                        break;
                    case PLState.FastLRun:
                        JumpHeight = FastJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("FastRun", false);
                            animator.SetBool("Run", false);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.FLJumping;
                        break;
                    case PLState.FastRRun:
                        JumpHeight = FastJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("FastRun", false);
                            animator.SetBool("Run", false);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.FRJumping;
                        break;
                    case PLState.BrakeF:
                        JumpHeight = normalJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("BrakeF", false);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.Jump;
                        break;
                    case PLState.FLBrake:
                        JumpHeight = FastJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("Brake", false);
                            animator.SetBool("FastRun", true);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.FLJumping;
                        break;
                    case PLState.FRBrake:
                        JumpHeight = FastJumpHeight;
                        if (animator)
                        {
                            animator.SetBool("Run", true);
                            animator.SetBool("Brake", false);
                            animator.SetBool("FastRun", true);
                            animator.SetBool("Jump", true);
                        }
                        pLState = PLState.FRJumping;
                        break;
                }
                break;
            case PlayerControState.CJump://松开跳跃
                playerCheckGround.gameObject.SetActive(true);
                PFunc.Log("松开跳跃", pLState);
                if (pLState != PLState.LStick && pLState != PLState.RStick)
                {
                    if (animator)
                    {
                        animator.SetBool("Drop", true);
                        animator.SetBool("Jump", false);
                    }
                }
                switch (pLState)
                {
                    case PLState.StickF:
                        moveVec = Vector2.zero;
                        currentSpeed = 0;
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                        pLState = PLState.Drop;
                        break;
                    case PLState.Jump:
                    case PLState.StickFJump:
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                        pLState = PLState.Drop;
                        break;
                    case PLState.LJumping:
                    case PLState.LStickFJump:
                        moveVec = Vector3.left;
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                        pLState = PLState.LDroping;

                        break;
                    case PLState.FLJumping:
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                        moveVec = Vector3.left;
                        pLState = PLState.FLDroping;
                        break;
                    case PLState.RJumping:
                    case PLState.RStickFJump:
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                        pLState = PLState.RDroping;
                        moveVec = Vector3.right;
                        break;
                    case PLState.FRJumping:
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                        pLState = PLState.FRDroping;
                        moveVec = Vector3.right;
                        break;
                }
                break;
            case PlayerControState.RStickJump:
                rigidbody2D.gravityScale = 3.5f;
                switch (pLState)
                {
                    case PLState.Stick:
                        if (spriteTrans.localScale.x == -1)
                        {
                            spriteTrans.localScale = new Vector3(1, 1, 1);
                            OnJump();
                            if (animator)
                            {
                                animator.SetBool("Run", false);
                                animator.SetBool("FastRun", false);
                                animator.SetBool("Jump", true);
                                animator.SetBool("Drop", false);
                                animator.SetBool("Stick", false);
                                animator.SetBool("StickF", false);
                            }
                            moveVec = Vector3.right;
                            currentSpeed = 8;
                            pLState = PLState.RJumping;
                        }
                        break;
                    case PLState.LStick:
                        spriteTrans.localScale = new Vector3(1, 1, 1);
                        OnJump();
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("Jump", true);
                            animator.SetBool("Drop", false);
                            animator.SetBool("Stick", false);
                            animator.SetBool("StickF", false);
                        }
                        moveVec = Vector3.right;
                        currentSpeed = 8;
                        pLState = PLState.RJumping;
                        break;
                }
                break;
            case PlayerControState.LStickJump:
                rigidbody2D.gravityScale = 3.5f;
                switch (pLState)
                {
                    case PLState.Stick:
                        PFunc.Log("LStickJump", playerControState);
                        if (spriteTrans.localScale.x == 1)
                        {
                            spriteTrans.localScale = new Vector3(-1, 1, 1);
                            OnJump();
                            if (animator)
                            {
                                animator.SetBool("Run", false);
                                animator.SetBool("FastRun", false);
                                animator.SetBool("Jump", true);
                                animator.SetBool("Drop", false);
                                animator.SetBool("Stick", false);
                                animator.SetBool("StickF", false);
                            }
                            moveVec = Vector3.left;
                            currentSpeed = 8;
                            pLState = PLState.LJumping;
                        }
                        break;
                    case PLState.RStick:
                        spriteTrans.localScale = new Vector3(-1, 1, 1);
                        OnJump();
                        if (animator)
                        {
                            animator.SetBool("Run", false);
                            animator.SetBool("FastRun", false);
                            animator.SetBool("Jump", true);
                            animator.SetBool("Drop", false);
                            animator.SetBool("Stick", false);
                            animator.SetBool("StickF", false);
                        }
                        moveVec = Vector3.left;
                        currentSpeed = 8;
                        pLState = PLState.LJumping;
                        break;
                }
            break;
            case PlayerControState.Boost:
                PFunc.Log("按下冲刺",boostImageContro.targetImage.fillAmount);
                if (boostImageContro.targetImage.fillAmount <=0) return;
                if (animator)
                {
                    animator.SetBool("Run", false);
                    animator.SetBool("FastRun", false);
                    animator.SetBool("Jump", false);
                    animator.SetBool("Drop", false);
                    animator.SetBool("Stick", false);
                    animator.SetBool("StickF", false);
                    animator.SetBool("Bake", false);
                    animator.SetBool("BakeF", false);
                    animator.SetBool("Boost", true);
                }
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                if (spriteTrans.localScale.x == -1)
                {
                    moveVec = Vector3.left;
                    pLState= PLState.LBoost;
                }
                else
                {
                    moveVec = Vector3.right;
                    pLState = PLState.RBoost;
                }
                currentSpeed = 13;
                boostImageContro.StartReduction();
                break;
            case PlayerControState.CanelBoost:
                switch (pLState)
                {
                    case PLState.LBoost:
                    case PLState.RBoost:
                        if (playerCheckGround.isGround)
                        {
                            if (animator)
                            {
                                animator.SetBool("Run", false);
                                animator.SetBool("FastRun", false);
                                animator.SetBool("Jump", false);
                                animator.SetBool("Drop", false);
                                animator.SetBool("Stick", false);
                                animator.SetBool("StickF", false);
                                animator.SetBool("Bake", false);
                                animator.SetBool("BakeF", false);
                                animator.SetBool("Boost", false);
                            }
                            pLState = PLState.Idel;
                        }
                        else
                        {
                            if (animator)
                            {
                                animator.SetBool("Run", false);
                                animator.SetBool("FastRun", false);
                                animator.SetBool("Jump", false);
                                animator.SetBool("Drop", true);
                                animator.SetBool("Stick", false);
                                animator.SetBool("StickF", false);
                                animator.SetBool("Bake", false);
                                animator.SetBool("BakeF", false);
                                animator.SetBool("Boost", false);
                            }
                            pLState = PLState.Drop;
                        }
                         currentSpeed = 0;
                        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                        boostImageContro.StartRecovery();
                        break;

                }
             break;
        }
    }

    /// <summary> 按下往左跑 </summary>
    void OnLRun()
    {
        currentSpeed = PlayerData.Instance.moveSpeed;
        targetSpeed = moveSpeed;
        moveVec = Vector3.left;
        spriteTrans.localScale = new Vector3(-1, 1, 1);
    }

    /// <summary> 按下往右跑 </summary>
    void OnRRun()
    {
        currentSpeed = PlayerData.Instance.moveSpeed;
        targetSpeed = moveSpeed;
        moveVec = Vector3.right;
        spriteTrans.localScale = new Vector3(1, 1, 1);
    }

    void OnIdel()
    {
        moveVec = Vector3.zero;
        currentSpeed = 0;
        pLState = PLState.Idel;
    }

    void OnJump()
    {
        Sound.PlaySound("Sound/PlayerJump");
        playerCheckGround.isGround = false;
        playerCheckGround.gameObject.SetActive(false);
        brakeTime = 0; 
        rigidbody2D.gravityScale = 3.5f;
        var jumpvalue = pLState == PLState.FastLRun ||
                                pLState == PLState.FastRRun? FastjumpForce : jumpForce;
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpvalue); // 设置初始跳跃速度
    }

    void OnPlayerMove()
    {
        if (oldState != pLState)
        {
           // PFunc.Log(pLState,moveVec,currentSpeed,JumpHeight);
            oldState = pLState;
        }
        switch (pLState)
        {
            case PLState.None:
                break;
            case PLState.Idel:
                rigidbody2D.gravityScale = 3.5f;
                OnCheckDrop();
                break;
            case PLState.LRun:
            case PLState.RRun: 
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);
                OnCheckDrop();
                break;
            case PLState.FastLRun:
            case PLState.FastRRun:
                OnCheckDrop();
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);
                break;
            case PLState.BrakeF:
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
                if (currentSpeed < 0.1f)
                {
                    if (animator)
                        animator.SetBool("BrakeF", false);
                    OnIdel();
                }
                OnCheckDrop();
                break;
            case PLState.FLBrake:
                brakeTime += Time.deltaTime;
                if (brakeTime > 0.3f)
                {
                    if (animator)
                    {
                        animator.SetBool("Run", true);
                        animator.SetBool("FastRun", true);
                        animator.SetBool("Brake", false); 
                        animator.SetBool("Jump", false);
                    }
                    pLState = PLState.FastLRun;
                }
                break;
            case PLState.FRBrake:
                brakeTime += Time.deltaTime;
                if (brakeTime > 0.3f)
                {
                    if (animator)
                    {
                        animator.SetBool("Run", true);
                        animator.SetBool("FastRun", true);
                        animator.SetBool("Brake", false);
                    }
                    pLState = PLState.FastRRun;
                }
                break;
            case PLState.Jump:
            case PLState.StickJump:
            case PLState.LJumping:
            case PLState.FLJumping:
            case PLState.RJumping:
            case PLState.FRJumping:
            case PLState.LStickJump:
            case PLState.RStickJump:
            case PLState.StickFJump:
            case PLState.LStickFJump:
            case PLState.RStickFJump:
                HandleJump();
                break;
            case PLState.Drop:
            case PLState.LDroping:
            case PLState.FLDroping:
            case PLState.RDroping:
            case PLState.FRDroping:
            case PLState.LStickDrop:
            case PLState.RStickDrop:
                OnHandleDrop();
                break;
            case PLState.Stick:
            case PLState.LStick:
            case PLState.RStick:
                OnCheckStickDown();
                break;
            case PLState.StickF:
            case PLState.RStickF:
            case PLState.LStickF:
                OnLRStickF();
                break;
            case PLState.LBoost:
            case PLState.RBoost:
                OnCheckBoostOver();
                OnCheckStick();
                break;
            case PLState.HorHit:
                CheckHintOver();
                break;
            case PLState.VecHit:
                transform.Translate(moveVec * currentSpeed * Time.deltaTime);
                OnChekcVecHit();
                break;
            case PLState.DownHit:
                OnChekcUpHit();
                break;
            case PLState.HitOver:
                OnCheckHitOverTime();
                break;
        }
        // 应用水平移动
        if (!playerCheckRight.isGround && moveVec!=Vector3.left
            || !playerCheckLeft.isGround && moveVec != Vector3.right)
        {
            transform.Translate(moveVec * currentSpeed * Time.deltaTime);
        } 
    }

    //跳跃时检测
    void HandleJump()
    {
        // 跳跃上升阶段
        if (rigidbody2D.velocity.y <= 0)
        {
            OnToDrop();
        }
    }

    bool OnCheckStick()
    {
        bool isStick = false;
        switch (pLState)
        {
            case PLState.Jump:
            case PLState.Drop:
                if (playerCheckLeft.isGround || playerCheckRight.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stick", true);
                    }
                    pLState = playerCheckLeft.isGround? PLState.LStick: PLState.RStick;
                    int x = pLState == PLState.LStick ? -1 : 1;
                    spriteTrans.localScale = new Vector3(x,1,1);
                    PFunc.Log("pLState = PLState.LStick");
                }
                break;
            case PLState.LJumping:
            case PLState.FLJumping:
                if (playerCheckLeft.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false); 
                        animator.SetBool("Stick", true);
                    }
                    pLState = PLState.LStick;
                    PFunc.Log("pLState = PLState.LStick");
                }
                break;
            case PLState.RJumping:
            case PLState.FRJumping:
                if (playerCheckRight.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stick", true);
                    }
                    pLState = PLState.RStick;
                }
                break;
            case PLState.LDroping:
            case PLState.FLDroping:
                if (playerCheckLeft.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Stick", true);
                    }
                    pLState = PLState.LStick;
                    PFunc.Log("pLState = PLState.LStick");
                }
                break;
            case PLState.FRDroping:
            case PLState.RDroping:
                if (playerCheckRight.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Stick", true);
                    }
                    pLState = PLState.RStick;
                }
                break;
            case PLState.LBoost:
                if (playerCheckLeft.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Boost", false);
                        animator.SetBool("Stick", true);
                    }
                    pLState = PLState.LStick;
                    PFunc.Log("pLState = PLState.LStick");
                    boostImageContro.StartRecovery();
                }
                break;
            case PLState.RBoost:
                if (playerCheckRight.isGround)
                {
                    isStick = true;
                    if (animator)
                    {
                        animator.SetBool("Run", false);
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Boost", false);
                        animator.SetBool("Stick", true);
                    }
                    pLState = PLState.RStick;
                    boostImageContro.StartRecovery();
                }
                break;
        }

        if (isStick)
        {
            rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
            currentSpeed = 0;
            targetSpeed = 0;
            moveVec = Vector3.zero;
            rigidbody2D.gravityScale = 0.1f;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        return isStick;
    }

    //坠落时检测
    void OnHandleDrop()
    {
        if (OnCheckStick()) return;
        if (!playerCheckGround.gameObject.activeSelf) return;
        if (playerCheckGround.isGround)
        {
            switch (pLState)
            {
                case PLState.Drop:
                    if (animator)
                    {
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Run", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false); 
                        animator.SetBool("Stick", false);
                    }
                    pLState = PLState.Idel;
                    break;
                case PLState.LDroping:
                    if (animator)
                    {
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Run", true);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stick", false);
                    }
                    OnLRun();
                    pLState = PLState.LRun;
                    break;
                case PLState.FLDroping:
                    if (animator)
                    {
                        animator.SetBool("FastRun", true); 
                        animator.SetBool("Run", true);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stick", false);
                    }
                    OnLRun();
                    currentSpeed = fmoveSpeed;
                    targetSpeed = fmoveSpeed;
                    pLState = PLState.FastLRun;
                    break;
                case PLState.RDroping:
                    if (animator)
                    {
                        animator.SetBool("Run", true); 
                        animator.SetBool("FastRun", false);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stick", false);
                    }
                    OnRRun();
                    pLState = PLState.RRun;
                    break;
                case PLState.FRDroping:
                    if (animator)
                    {
                        animator.SetBool("Run", true);
                        animator.SetBool("FastRun", true);
                        animator.SetBool("Drop", false);
                        animator.SetBool("Jump", false);
                        animator.SetBool("Stick", false);
                    }
                    OnRRun();
                    currentSpeed = fmoveSpeed;
                    targetSpeed = fmoveSpeed;
                    pLState = PLState.FastRRun;
                    break;
            }
            //rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        }
    }
    void OnCheckDrop()
    {
        if (!playerCheckGround.isGround)
        {
            OnToDrop();
        }
    }

    void OnToDrop()
    {
        if (OnCheckStick()) return;
        //rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
        playerCheckGround.gameObject.SetActive(true);
        if (animator)
        {
            animator.SetBool("Drop", true);
            animator.SetBool("Jump", false);
            animator.SetBool("Stick", false);
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
        }
        switch (pLState)
        {
            case PLState.Idel:
            case PLState.Jump:
                pLState = PLState.Drop;
                break;
            case PLState.LJumping:
            case PLState.LRun:
                pLState = PLState.LDroping;
                break;
            case PLState.FLJumping:
            case PLState.FastLRun:
                pLState = PLState.FLDroping;
                break;
            case PLState.RJumping:
            case PLState.RRun:
                pLState = PLState.RDroping;
                break;
            case PLState.FRJumping:
            case PLState.FastRRun:
                pLState = PLState.FRDroping;
                break;
            case PLState.StickFJump:
                currentSpeed = 0;
                pLState = PLState.Drop;
                break;
            case PLState.LStickFJump:
                moveVec = Vector3.left;
                pLState = PLState.LDroping;
                break;
            case PLState.RStickFJump:
                moveVec = Vector3.right;
                pLState = PLState.RDroping;
                break;
        }
    }

    void OnCheckStickDown()
    {
        switch (pLState)
        {
            case PLState.Stick:
                if (!playerCheckLeft.isGround&& !playerCheckRight.isGround)
                {
                    if (animator)
                    {
                        animator.SetBool("Drop", false);
                        animator.SetBool("StickF", false);
                        animator.SetBool("Stick", false);
                    }
                    pLState = PLState.Idel;
                }
                break;
            case PLState.LStick:
                if (!playerCheckLeft.isGround)
                {
                    if (animator)
                    {
                        animator.SetBool("Drop", false);
                        animator.SetBool("StickF", false);
                        animator.SetBool("Stick", false);
                    }
                    pLState = PLState.Idel;
                }
                break;
            case PLState.RStick:
                if (!playerCheckRight.isGround)
                {
                    if (animator)
                    {
                        animator.SetBool("Drop", false);
                        animator.SetBool("StickF", false);
                        animator.SetBool("Stick", false);
                    }
                    pLState = PLState.Idel;
                }
                break;
        }
        if (playerCheckGround.isGround)
        {
            if (animator)
            {
                animator.SetBool("Drop", false);
                animator.SetBool("StickF", false);
                animator.SetBool("Stick", false);
            }
            currentSpeed = 0;
            targetSpeed = 0;
            moveVec = Vector3.zero;
            rigidbody2D.gravityScale = 3.5f;
            pLState = PLState.Idel;
        }
    }

    void OnLRStickF()
    {
       // PFunc.Log(moveVec, currentSpeed, stickFTime, pLState);
        if (stickFTime < 0.03f)
        {
            stickFTime += Time.deltaTime;  
        }
        else
        {
            if (animator)
            {
                animator.SetBool("Jump", true);
                animator.SetBool("Stick", false);
                animator.SetBool("Drop", false);
                animator.SetBool("StickF", false);
            }
            OnJump();
            stickFTime = 0;
            moveVec = Vector3.zero;
            switch (pLState)
            {
                case PLState.StickF:
                    pLState = PLState.StickFJump;
                    break;
                case PLState.LStickF:
                    pLState = PLState.LStickFJump;
                    break;
                case PLState.RStickF:
                    pLState = PLState.RStickFJump;
                    break;
            }
        }
    }

    IEnumerator OnCreateEffect()
    {
        while (true)
        {
            if (playerCheckGround.isGround && pLState == PLState.FastRRun)
            {
                chitobj = SimplePool.Spawn(runRightEffect, transform.position, Quaternion.identity);
                chitobj.transform.parent = createPos;
                chitobj.SetActive(true);
            }
            if (playerCheckGround.isGround && pLState == PLState.FastLRun)
            {
                chitobj = SimplePool.Spawn(runLeftEffect, transform.position, Quaternion.identity);
                chitobj.transform.parent = createPos;
                chitobj.SetActive(true);
            }

            // 每0.1秒检查一次（避免频繁生成）
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator OnCreateBoostEffect()
    {
        while (true)
        {
            if (pLState == PLState.RBoost)
            {
                chitobj = SimplePool.Spawn(boostRightEffect, transform.position, Quaternion.identity);
                chitobj.transform.parent = createPos;
                chitobj.SetActive(true);
            }
            if (pLState == PLState.LBoost)
            {
                chitobj = SimplePool.Spawn(boostLeftEffect, transform.position, Quaternion.identity);
                chitobj.transform.parent = createPos;
                chitobj.SetActive(true);
            }
            // 每0.1秒检查一次（避免频繁生成）
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator OnCreateHorHitEffect()
    {
        while (true)
        {
            if (pLState == PLState.HorHit)
            {
                chitobj = SimplePool.Spawn(horHitEffect, transform.position, Quaternion.identity);
                chitobj.transform.parent = createPos;
                chitobj.SetActive(true);
            }
            // 每0.1秒检查一次（避免频繁生成）
            yield return new WaitForSeconds(0.08f);
        }
    }
    GameObject chitobj = null;
    IEnumerator OnCreateVecHitEffect()
    {
        while (true)
        {
            if (pLState == PLState.VecHit)
            {
                chitobj = SimplePool.Spawn(vecHitEffect, transform.position, Quaternion.identity);
                chitobj.transform.parent = createPos;
                chitobj.SetActive(true);
            }

            // 每0.1秒检查一次（避免频繁生成）
            yield return new WaitForSeconds(0.05f);
        }
    }

    bool isHitLeft = false;
    /// <summary> 处理水平碰撞（HorHit） </summary>
    public void HandleHorHitCollision()
    {
        isHit = true;
        rigidbody2D.gravityScale = 3.5f;
        currentSpeed = 0;
        moveVec = Vector3.zero;
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("BrakeF", false);
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Brake", false);
            animator.SetBool("Stick", false);
            animator.SetBool("StickF", false);
            animator.SetBool("Boost", false);
            animator.SetBool("DamageOver", false);
            animator.SetTrigger("Demage");
        }
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        Vector2 force = new Vector2(-10, 10);
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        spriteTrans.localScale = new Vector3(1, 1, 1);
        playerCheckGround.gameObject.SetActive(true);
        pLState = PLState.HorHit;
        if (playerCheckHit.isHit)
        {
            pCteateHit.OnCreateHitEffect(3);
        }
        else
        {
            pCteateHit.OnCreateHitEffect(4);
        }
    }
    public void HandleVecHitCollision()
    {
        isHit = true;
        rigidbody2D.gravityScale = 3.5f;
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("BrakeF", false);
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Brake", false);
            animator.SetBool("Stick", false);
            animator.SetBool("StickF", false);
            animator.SetBool("Boost", false);
            animator.SetBool("DamageOver", false);
            animator.SetTrigger("Demage");
        }
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        moveVec = Vector3.up;
        currentSpeed = 20;
        playerCheckGround.gameObject.SetActive(true);
        pLState = PLState.VecHit;
        pCteateHit.OnCreateHitEffect(1);
    }
    public void HandleDownHitCollision()
    {
        isHit = true;
        rigidbody2D.gravityScale = 3.5f;
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("BrakeF", false);
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Brake", false);
            animator.SetBool("Stick", false);
            animator.SetBool("StickF", false);
            animator.SetBool("Boost", false);
            animator.SetBool("DamageOver", false);
            animator.SetTrigger("Demage");
        }
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        moveVec = Vector3.left;
        currentSpeed = 20;
        playerCheckGround.gameObject.SetActive(true);
        pLState = PLState.DownHit;
        pCteateHit.OnCreateHitEffect(2);
    }
    void CheckHintOver()
    {
        if (!playerCheckGround.isGround)
            return;
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("BrakeF", false);
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Brake", false);
            animator.SetBool("Stick", false);
            animator.SetBool("StickF", false);
            animator.SetBool("Boost", false);
            animator.SetBool("DemageOver", true);
        }
        if (!IsForceNearlyGone())
            return;

        damageTime = 0;
        pLState = PLState.HitOver;
        if(dizzard) dizzard.SetActive(true);
    }
    void OnChekcVecHit()
    {
        if (playerCheckHit.isHit) return;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        Vector2 force = new Vector2(-10,10);
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        spriteTrans.localScale = new Vector3(1, 1, 1);
        playerCheckGround.gameObject.SetActive(true);
        currentSpeed = 0;
        moveVec = Vector3.zero;
        isCheckVec = false;
        pLState = PLState.HorHit;
    }
    void OnChekcUpHit()
    {
    
        if (playerCheckUpHit.isHit) return;
        if (playerCheckHit.isHit)
        {
            HandleVecHitCollision();
        }
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerCheckGround.gameObject.SetActive(true);
        currentSpeed = 0;
        moveVec = Vector3.zero;
        pLState = PLState.HorHit;
    }
    void OnCheckHitOverTime()
    {
        damageTime += Time.deltaTime;
        if (damageTime < 1)  return;
     
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("BrakeF", false);
            animator.SetBool("Drop", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Brake", false);
            animator.SetBool("Stick", false);
            animator.SetBool("StickF", false);
            animator.SetBool("Boost", false);
            animator.SetBool("DemageOver", false);
        }
        if (dizzard) dizzard.SetActive(false);
        pLState = PLState.Idel;
        damageTime = 0;
        isHit = false;
        PlayerAutomaticSystem.Instance.OnBeginAutomatic();
    }

    void OnCheckBoostOver()
    {
        if (!boostImageContro.IsReducing)
        {
            OnPlayerControStateChange(PlayerControState.CanelBoost);
        }
    }
    /// <summary> 检查冲击力是否几乎消失 </summary>
    private bool IsForceNearlyGone()
    {
        // 设置一个很小的阈值来判断速度是否接近零
        float velocityThreshold = 0.1f;
        float angularVelocityThreshold = 0.1f;

        // 检查线速度和角速度是否都接近零
        bool isVelocityNearlyZero = Mathf.Abs(rigidbody2D.velocity.x) < velocityThreshold &&
                                   Mathf.Abs(rigidbody2D.velocity.y) < velocityThreshold;

        bool isAngularVelocityNearlyZero = Mathf.Abs(rigidbody2D.angularVelocity) < angularVelocityThreshold;

        return isVelocityNearlyZero && isAngularVelocityNearlyZero;
    }
    public void OnArrowUp()
    {
        if (isHit) return;
        currentSpeed = 0;
        moveVec = Vector2.zero;
        if (animator)
        {
            animator.SetBool("Boost", false);
            animator.SetBool("Drop", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("Run", false);
            animator.SetBool("Jump", true);
        }
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        Vector2 force = new Vector2(0, 15);
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        pLState = PLState.Jump;
        boostImageContro.StartRecovery();
    }
    public void OnArrowRight()
    {
        if (isHit) return;
        currentSpeed = 0;
        moveVec = Vector2.zero;
        rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        if (animator)
        {
            animator.SetBool("Boost", false);
            animator.SetBool("Drop", false);
            animator.SetBool("FastRun", false);
            animator.SetBool("Run", false);
            animator.SetBool("Jump", true);
        }
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        Vector2 force = new Vector2(18, 7);
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        pLState = PLState.Jump;
        boostImageContro.StartRecovery();
    }
    public GameObject strcikObj;
    public void OnSetStickCheck(bool show )
    {
        strcikObj.SetActive(show);
    }
}