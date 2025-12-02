using UnityEngine;

public class AdvancedBossController : MonoBehaviour
{
    [Header("状态设置")]
    public Transform startPosition;
    public Transform riseTargetPosition;
    public float riseSpeed = 3f;
    public float detectionRange = 5f;
    public string playerTag = "Player";

    [Header("追击设置")]
    public float boostChaseSpeed = 8f;
    public float maxChaseDistance = 10f;
    public float minDistanceToPlayer = 1.5f;
    [Range(0.1f, 1f)] public float chaseSmoothness = 0.3f;
    public LayerMask wallLayer;         // 墙壁层级
    [Header("组件引用")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float maxChaseLevel =0;
    public Transform bloodPos;
    public GameObject redMonsterDie;

    float normalChaseSpeed = PlayerData.Instance.moveSpeed;
    private Transform player;
    private enum BossState { Idle, Rising, Chasing }
    private BossState currentState = BossState.Idle;

    public float vectorX = 0;

    void Start()
    {
        EventManager.Instance.AddListener(Events.GameRest, OnRest);
        OnRest(null);
    }
    void OnRest(object msg)
    {
        if (animator)
        {
            animator.Rebind(); 
            animator.Update(0f);
        }
        currentState = BossState.Idle;
        transform.position = startPosition.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                CheckForPlayer();
                break;
            case BossState.Rising:
                RiseToPosition();
                break;
            case BossState.Chasing:
                ChasePlayer();
                break;
        }
    }

    private void CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.up,
            detectionRange, wallLayer);

        if (hit.collider != null && hit.collider.CompareTag(playerTag))
        {
            player = hit.collider.transform;
            currentState = BossState.Rising;
            OnPlayerDetected();
        }
    }

    private void RiseToPosition()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            riseTargetPosition.position,
            riseSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, riseTargetPosition.position) < 0.1f)
        {
            currentState = BossState.Chasing;
            OnReachedTargetPosition();
        }
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

        // 移动逻辑
        if (distance <= minDistanceToPlayer)
        {
            OnPlayerInRange();
            return;
        }

        if (distance > maxChaseDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                currentSpeed * Time.deltaTime);
            OnBoostSpeedActivated();
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

    // ===== 事件接口 =====
    private void OnPlayerDetected()
    {
        Debug.Log("玩家检测！开始上升");
        // 触发动画等

    }

    private void OnReachedTargetPosition()
    {
        Debug.Log("到达目标高度，开始追击");
        // 触发动画等
        animator.SetTrigger("Eat");
        Sound.PlaySound("Sound/BossScreamSfx");
    }

    private void OnBoostSpeedActivated()
    {
        Debug.Log("距离过远，急加速！");
        // 触发加速特效等
    }

    private void OnPlayerInRange()
    {
        Debug.Log("玩家进入攻击范围");
        // 准备攻击等
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            OnPlayerCaught();
        }
        if (other.tag.Equals("Monster"))
        {
            Sound.PlaySound("Sound/BossEat01");
            Instantiate(redMonsterDie, bloodPos.position, Quaternion.identity);
            var redMonster = other.GetComponent<RedMonsterController>();
            redMonster.OnClose();
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