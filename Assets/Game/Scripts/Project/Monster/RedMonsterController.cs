using DG.Tweening;
using UnityEngine;

public class RedMonsterController : MonsterBase
{
    public Transform checkPos;          // 检测起始位置
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject enemyDie;
    public BoxCollider2D boxCollider;

    [Header("检测设置")]
    public float rayLength = 10f;       // 射线长度
    public LayerMask playerLayer;       // Player所在的层
    public string playerTag = "Player"; // Player标签
    public bool drawDebugRay = true;    // 是否绘制调试射线
    public Color rayColor = Color.red;  // 调试射线颜色

    private RaycastHit2D centerHit;     // 垂直射线碰撞信息
    private bool checkPlayer = true;    // 是否继续检测

    [Header("巡逻模式选择")]
    public PatrolMode patrolMode = PatrolMode.WallDetection; // 巡逻模式
    public enum PatrolMode
    {
        WallDetection,   // 墙壁检测模式（原有）
        TargetPoints     // 目标点模式（新增）
    }

    [Header("墙壁检测模式设置")]
    public float moveSpeed = 2f;        // 基础移动速度
    public LayerMask wallLayer;         // 墙壁层级
    public string wallTag = "Wall";     // 墙壁标签
    public float valueY = 0;            // 掉落后的固定本地Y坐标

    [Header("目标点模式设置")]
    public float minLocalX = -5f;       // 最小本地X坐标
    public float maxLocalX = 5f;         // 最大本地X坐标
    public float arrivalThreshold = 0.1f; // 到达点的阈值

    [Header("速度变化设置")]
    public float speedBoostMultiplier = 2f;    // 速度提升倍数
    public bool drawDebugRays = true;         // 绘制调试射线

    public bool isLive = true;
    public bool alreadyPatrol = false;

    private float currentSpeed;                // 当前移动速度
    private bool movingRight = true;           // 当前移动方向（墙壁模式）
    private bool movingToMax = true;           // 当前移动方向（目标点模式）
    private float speedTimer;                  // 速度状态计时器
    private bool isSpeedBoosted = false;       // 是否处于加速状态
    private bool isPatrol = false;             // 是否处于巡逻状态
    private bool isFalling = false;            // 是否正在掉落
    private float fallSpeed = 5f;              // 掉落速度
    private float normalSpeedDuration = 10f;   // 正常速度持续时间
    private float boostedSpeedDuration = 5f;   // 加速状态持续时间

    private Vector3 initialLocalPosition;     // 初始本地位置

    private void Start()
    {
        initialPosition = transform.position;
        initialLocalPosition = transform.localPosition; // 保存初始本地位置
        EventManager.Instance.AddListener(Events.GameRest, OnRest);
        currentSpeed = moveSpeed;
        speedTimer = normalSpeedDuration;

        if (alreadyPatrol)
        {
            checkPlayer = false;
            isFalling = false;
            isPatrol = true;
            if (animator) animator.SetBool("Run", true);
            OnBeginPartal();
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.GameRest, OnRest);
    }

    void OnRest(object msg)
    {
        if (alreadyPatrol) { return; }
        if (spriteRenderer) spriteRenderer.enabled = true;
        if (boxCollider) boxCollider.enabled = true;
        transform.position = initialPosition;
        transform.localPosition = initialLocalPosition; // 重置本地位置
        if (animator)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        checkPlayer = true;
        isFalling = false;
        isPatrol = false;
    }

    public void OnClose()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        checkPlayer = false;
        isFalling = false;
        isPatrol = false;
    }

    private void Update()
    {
        if (!isLive) { return; }

        if (checkPlayer)
        {
            CheckForPlayer();
        }

        // 处理掉落状态
        if (isFalling)
        {
            HandleFalling();
        }

        if (isPatrol)
        {
            PatrolMovement();
            UpdateSpeedState();
        }
    }

    // 检测Player的方法
    private void CheckForPlayer()
    {
        // 从checkPos位置垂直向下发射射线
        centerHit = Physics2D.Raycast(checkPos.position, Vector2.down, rayLength, playerLayer);

        // 绘制调试射线
        if (drawDebugRay)
        {
            Debug.DrawRay(checkPos.position, Vector2.down * rayLength, rayColor);
        }
        // 如果检测到物体
        if (centerHit.collider != null)
        {
            // 检查是否是Player
            if (centerHit.collider.CompareTag(playerTag))
            {
                OnPlayerDetected(centerHit.collider.gameObject);
                checkPlayer = false; // 停止继续检测
            }
        }
    }

    // 处理掉落状态（基于本地坐标）
    private void HandleFalling()
    {
        // 计算新本地Y位置
        float newLocalY = Mathf.MoveTowards(transform.localPosition.y, valueY, fallSpeed * Time.deltaTime);
        transform.localPosition = new Vector3(transform.localPosition.x, newLocalY, transform.localPosition.z);

        // 检查是否到达目标本地Y位置
        if (Mathf.Approximately(transform.localPosition.y, valueY))
        {
            Sound.PlaySound("Sound/SpankingSfx");
            isFalling = false;
            if (animator) animator.SetTrigger("Down");
            Invoke("OnBeginPartal", 1f);
        }
    }

    // 巡逻移动（根据模式选择）
    private void PatrolMovement()
    {
        switch (patrolMode)
        {
            case PatrolMode.WallDetection:
                WallDetectionMovement();
                break;
            case PatrolMode.TargetPoints:
                TargetPointsMovement();
                break;
        }
    }

    // ... 其他代码保持不变 ...

    // 修改墙壁检测模式移动（基于本地坐标）
    private void WallDetectionMovement()
    {
        // 根据方向移动，只改变本地X坐标
        float direction = movingRight ? 1 : -1;
        float newLocalX = transform.localPosition.x + direction * currentSpeed * Time.deltaTime;
        transform.localPosition = new Vector3(newLocalX, valueY, transform.localPosition.z); // 保持本地Y坐标不变

        // 检查墙壁碰撞
        CheckWalls();
    }

    // 修改目标点模式移动（基于本地坐标）
    private void TargetPointsMovement()
    {
        // 确定目标本地X坐标
        float targetLocalX = movingToMax ? maxLocalX : minLocalX;

        // 向目标本地X坐标移动
        float newLocalX = Mathf.MoveTowards(transform.localPosition.x, targetLocalX, currentSpeed * Time.deltaTime);
        transform.localPosition = new Vector3(newLocalX, valueY, transform.localPosition.z); // 保持本地Y坐标不变

        // 更新面向方向
        UpdateFacingDirection(targetLocalX);

        // 检查是否到达目标点
        if (Mathf.Abs(transform.localPosition.x - targetLocalX) <= arrivalThreshold)
        {
            // 切换目标点
            movingToMax = !movingToMax;
        }
    }

    // 新增：设置怪物本地位置和移动方向的方法
    public void SetLocalPositionAndDirection(Vector3 localPosition, bool moveRight)
    {
        if (!isLive || !isPatrol) return;
        speedTimer = 10;
        currentSpeed = moveSpeed;
        speedTimer = normalSpeedDuration;
        isSpeedBoosted = false;
        // 设置本地位置
        transform.localPosition = localPosition;

        // 根据巡逻模式设置移动方向（同上）
        switch (patrolMode)
        {
            case PatrolMode.WallDetection:
                movingRight = moveRight;
                int scaleX = movingRight ? 1 : -1;
                if (spriteRenderer != null)
                {
                    spriteRenderer.transform.localScale = new Vector3(scaleX, 1, 1);
                }
                break;

            case PatrolMode.TargetPoints:
                float currentLocalX = transform.localPosition.x;
                movingToMax = moveRight ? (currentLocalX <= maxLocalX) : (currentLocalX <= minLocalX);

                if (moveRight)
                {
                    movingToMax = true;
                }
                else
                {
                    movingToMax = false;
                }

                UpdateFacingDirection(movingToMax ? maxLocalX : minLocalX);
                break;
        }

        // 确保Y坐标固定在指定位置
        Vector3 localPos = transform.localPosition;
        transform.localPosition = new Vector3(localPos.x, valueY, localPos.z);
    }

    // 更新面向方向
    private void UpdateFacingDirection(float targetLocalX)
    {
        // 计算移动方向
        float direction = targetLocalX - transform.localPosition.x;

        // 更新Sprite朝向
        if (direction > 0)
        {
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 检查墙壁碰撞（基于本地坐标转换到世界坐标进行检测）
    private void CheckWalls()
    {
        // 将本地位置转换为世界位置进行射线检测
        Vector3 worldPosition = transform.TransformPoint(transform.localPosition);

        // 前方射线检测
        RaycastHit2D frontHit = Physics2D.Raycast(
            transform.position,
            transform.right, // 使用本地右方向
            0.5f,
            wallLayer);

        // 后方射线检测（防止卡在角落）
        RaycastHit2D backHit = Physics2D.Raycast(
             transform.position,
            -transform.right, // 使用本地左方向
             0.5f,
            wallLayer);

        // 绘制调试射线
        if (drawDebugRays)
        {
            Debug.DrawRay(transform.position, transform.right * 0.5f, rayColor);
            Debug.DrawRay(transform.position, -transform.right * 0.5f, rayColor);
        }

        if (movingRight)
        {
            // 如果检测到墙壁则掉头
            if ((frontHit.collider != null && frontHit.collider.CompareTag(wallTag)))
            {
                TurnAround();
            }
        }
        else
        {
            // 如果检测到墙壁则掉头
            if ((backHit.collider != null && backHit.collider.CompareTag(wallTag)))
            {
                TurnAround();
            }
        }
    }

    // 掉头（墙壁检测模式）
    private void TurnAround()
    {
        movingRight = !movingRight;
        int xx = movingRight ? 1 : -1;
        spriteRenderer.transform.localScale = new Vector3(xx, 1, 1);
    }

    // 更新速度状态
    private void UpdateSpeedState()
    {
        speedTimer -= Time.deltaTime;

        if (speedTimer <= 0)
        {
            if (isSpeedBoosted)
            {
                // 结束加速状态
                currentSpeed = moveSpeed;
                speedTimer = normalSpeedDuration;
                isSpeedBoosted = false;
            }
            else
            {
                // 开始加速状态
                currentSpeed = moveSpeed * speedBoostMultiplier;
                speedTimer = boostedSpeedDuration;
                isSpeedBoosted = true;
            }
        }
    }

    // Player被检测到时的处理
    private void OnPlayerDetected(GameObject player)
    {
        Sound.PlaySound("Sound/ChainSfx");
        Debug.Log("检测到Player: " + player.name);
        if (animator) animator.SetTrigger("Drop");
        isFalling = true; // 开始掉落状态
    }

    void OnBeginPartal()
    {
        isPatrol = true;
    }

    // 重置检测（可以重新开始检测Player）
    public void ResetDetection()
    {
        checkPlayer = true;
    }

    // 设置目标点范围（本地坐标）
    public void SetTargetPoints(float minLocalX, float maxLocalX)
    {
        this.minLocalX = minLocalX;
        this.maxLocalX = maxLocalX;
        patrolMode = PatrolMode.TargetPoints; // 自动切换到目标点模式
    }

    // 设置巡逻模式
    public void SetPatrolMode(PatrolMode mode)
    {
        patrolMode = mode;
    }

    // 在Scene视图中可视化
    private void OnDrawGizmos()
    {
        if (checkPos != null && drawDebugRay)
        {
            Gizmos.color = rayColor;
            Gizmos.DrawLine(checkPos.position, checkPos.position + Vector3.down * rayLength);
        }

        // 绘制目标点模式的范围（本地坐标）
        if (patrolMode == PatrolMode.TargetPoints)
        {
            // 将本地坐标转换为世界坐标进行绘制
            Vector3 minWorldPos = transform.parent != null ?
                transform.parent.TransformPoint(new Vector3(minLocalX, valueY, 0)) :
                new Vector3(minLocalX, valueY, 0);

            Vector3 maxWorldPos = transform.parent != null ?
                transform.parent.TransformPoint(new Vector3(maxLocalX, valueY, 0)) :
                new Vector3(maxLocalX, valueY, 0);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(minWorldPos, maxWorldPos);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(minWorldPos, 0.3f);
            Gizmos.DrawWireSphere(maxWorldPos, 0.3f);

            // 绘制当前移动方向
            if (isPatrol)
            {
                float targetLocalX = movingToMax ? maxLocalX : minLocalX;
                Vector3 targetWorldPos = transform.parent != null ?
                    transform.parent.TransformPoint(new Vector3(targetLocalX, valueY, 0)) :
                    new Vector3(targetLocalX, valueY, 0);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetWorldPos);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision) return;

        if (collision.CompareTag("Arrow"))
        {
            Sound.PlaySound("Sound/MonsterHit2Sfx");
            spriteRenderer.DOFade(0, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                spriteRenderer.color = Color.white;
            });
            curHP--;
            if (curHP == 0)
            {
                Instantiate(enemyDie, this.transform.position, Quaternion.identity);
                OnClose();
            }
        }
    }
}