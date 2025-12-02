using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndHeadBoss : MonsterBase
{
    public string playerTag = "Player";
    [Header("追击设置")]
    public float boostChaseSpeed = 8f;
    public float maxChaseDistance = 10f;
    public float minDistanceToPlayer = 1.5f;
    [Range(0.1f, 1f)] public float chaseSmoothness = 0.3f;
    [Header("冲刺设置")]
    public float dashSpeed = 15f;       // 冲刺速度
    public float dashDistance = 4f;     // 冲刺距离
    public float dashInterval = 5f;    // 冲刺间隔（秒）

    [Header("组件引用")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float maxChaseLevel = 0;
    public Transform bloodPos;
    public GameObject redMonsterDie;
    public GameObject SalivaObj;
    public PolygonCollider2D collider2D;

    float normalChaseSpeed = PlayerData.Instance.moveSpeed;
    public Transform player;
    private enum BossState { Idle, Angry, Chasing, Dashing }
    private BossState currentState = BossState.Idle;
    Vector3 startPos;
    public bool isLive = false;

    public float vectorX = 0;

    // 冲刺相关变量
    private float dashTimer = 0f;
    private Vector3 dashDirection;
    private Vector3 dashTargetPosition;
    private bool isDashing = false;

    void Start()
    {
        initialPosition = transform.position;
        EventManager.Instance.AddListener(Events.GameRest, OnRest);
        startPos = transform.position;
        SalivaObj.SetActive(false);
        OnRest(null);
    }

    void OnRest(object msg)
    {
        if (animator)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        collider2D.enabled = true;
        spriteRenderer.gameObject.SetActive(true);
        SalivaObj.gameObject.SetActive(false);
        currentState = BossState.Idle;
        transform.position = startPos;
        isDashing = false;
        isLast = false;
        dashTimer = 0f;
        CancelInvoke("StartDash");
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                OnCheckPlayer();
                break;
            case BossState.Chasing:
                ChasePlayer();
                UpdateDashTimer();
                break;
            case BossState.Dashing:
                PerformDash();
                break;
        }
    }

    void OnCheckPlayer()
    {
        if (PlayerController.Instance.transform.position.x > 3206&&!isLive)
        {
           isLive = true;
           player = PlayerController.Instance.transform;
           CheckForPlayer();
        }
    }

    void UpdateDashTimer()
    {
        if (!isDashing)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashInterval)
            {
                StartDash();
            }
        }
    }

    void StartDash()
    {
        if (player == null || isDashing) return;

        // 进入冲刺状态
        currentState = BossState.Dashing;
        isDashing = true;
        dashTimer = 0f;

        // 计算冲刺方向（朝向玩家）
        dashDirection = (player.position - transform.position).normalized;
        dashTargetPosition = transform.position + dashDirection * dashDistance;
        Sound.PlaySound("Sound/BossScreamShortSfx");
        if (isLast) 
            animator.SetTrigger("BossLastAtk");
        else 
            animator.SetTrigger("BossAtk");
    }

    void PerformDash()
    {
        // 向目标位置冲刺
        transform.position = Vector3.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.deltaTime);

        // 检查是否到达冲刺目标位置
        float distanceToTarget = Vector3.Distance(transform.position, dashTargetPosition);
        if (distanceToTarget < 0.1f)
        {
            // 冲刺结束，返回追击状态
            EndDash();
        }
    }

    void EndDash()
    {
        if (isLast)
            animator.SetTrigger("BossLastAtkEnd");
        else
            animator.SetTrigger("BossAtkEnd");
        isDashing = false;
        currentState = BossState.Chasing;

        // 重置计时器，准备下一次冲刺
        dashTimer = 0f;
    }

    public void CheckForPlayer()
    {
        Sound.PlaySound("Sound/BossGrowlSfx");
        Invoke("Screamming", 3f);

    }

    void Screamming()
    {
        Sound.PlaySound("Sound/BossScreamSfx");
        animator.SetTrigger("BossAtk");
        Invoke("ScreammingEnd", 3f);
        if (SalivaObj) SalivaObj.SetActive(true);
    }

    void ScreammingEnd()
    {
        animator.SetTrigger("BossAtkEnd");
        if (SalivaObj) SalivaObj.SetActive(false);
        currentState = BossState.Chasing;
        PFunc.Log("开始追击", currentState);
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        if (player.position.x < vectorX)
        {
            return;
        }
        if (transform.position.x > maxChaseLevel)
        {
            OnRest(null);
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        Vector3 direction = toPlayer.normalized;

        // 确定速度
        float currentSpeed = (distance > maxChaseDistance) ?
            boostChaseSpeed : normalChaseSpeed;

        // 目标位置（保持最小距离）
        Vector3 targetPosition = player.position - direction * minDistanceToPlayer;

        if (distance > maxChaseDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                currentSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                chaseSmoothness * Time.deltaTime * currentSpeed);
        }

        // 面向玩家
        spriteRenderer.flipX = direction.x > 0;
    }

    bool isLast = false;

    private void OnTriggerEnter2D(Collider2D other)
    { 
        if (other.CompareTag(playerTag))
        {
            Sound.PlaySound("Sound/BossEat02");
            OnPlayerCaught();
        }
        if (other.CompareTag("Arrow"))
        {
            curHP--;
            if (curHP < 20&&!isLast)
            {
                Sound.PlaySound("Sound/BrokenGlass");
                isLast = true;
                animator.SetTrigger("BossLastIdel");
            }

            if (curHP <= 0 )
            {
                EventManager.Instance.SendMessage(Events.SevenBossDie);
                Sound.PlaySound("Sound/BrokenGlass");
                animator.SetTrigger("BossDie");
                isLive = false;
                collider2D.enabled = false;
                spriteRenderer.gameObject.SetActive(false);
            }
        }
    }

    private void OnPlayerCaught()
    {
        EventManager.Instance.SendMessage(Events.GameRest);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.GameRest, OnRest);
    }
}