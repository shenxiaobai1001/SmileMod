using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMonsterMoveController : MonoBehaviour
{
    [Header("巡逻设置")]
    public float moveSpeed = 2f;               // 基础移动速度
    public float patrolRange = 5f;             // 巡逻范围
    public Transform leftBound;                // 左边界
    public Transform rightBound;              // 右边界
    public Transform frontCheck;              // 前方检测点
    public Transform backCheck;               // 后方检测点
    public LayerMask wallLayer;               // 墙壁层级
    public string wallTag = "Wall";            // 墙壁标签

    [Header("速度变化设置")]
    public float speedBoostMultiplier = 2f;    // 速度提升倍数
    public float normalSpeedDuration = 10f;   // 正常速度持续时间
    public float boostedSpeedDuration = 5f;    // 加速状态持续时间

    [Header("调试")]
    public bool drawDebugRays = true;         // 绘制调试射线
    public Color rayColor = Color.red;         // 射线颜色

    private float currentSpeed;                // 当前移动速度
    private bool movingRight = true;           // 当前移动方向
    private float speedTimer;                  // 速度状态计时器
    private bool isSpeedBoosted = false;       // 是否处于加速状态

    private void Start()
    {
        currentSpeed = moveSpeed;
        speedTimer = normalSpeedDuration;

        // 如果没有手动设置边界，自动创建
        if (leftBound == null || rightBound == null)
        {
            SetupDefaultBounds();
        }
    }

    private void Update()
    {
        PatrolMovement();
        CheckWalls();
        UpdateSpeedState();
    }

    // 巡逻移动
    private void PatrolMovement()
    {
        // 根据方向移动
        float direction = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * direction * currentSpeed * Time.deltaTime);

        // 检查是否超出边界
        if (movingRight && transform.position.x > rightBound.position.x)
        {
            TurnAround();
        }
        else if (!movingRight && transform.position.x < leftBound.position.x)
        {
            TurnAround();
        }
    }

    // 检查墙壁碰撞
    private void CheckWalls()
    {
        // 前方射线检测
        RaycastHit2D frontHit = Physics2D.Raycast(
            frontCheck.position,
            movingRight ? Vector2.right : Vector2.left,
            0.5f,
            wallLayer);

        // 后方射线检测（防止卡在角落）
        RaycastHit2D backHit = Physics2D.Raycast(
            backCheck.position,
            movingRight ? Vector2.left : Vector2.right,
            0.5f,
            wallLayer);

        // 绘制调试射线
        if (drawDebugRays)
        {
            Debug.DrawRay(frontCheck.position,
                (movingRight ? Vector2.right : Vector2.left) * 0.5f,
                rayColor);

            Debug.DrawRay(backCheck.position,
                (movingRight ? Vector2.left : Vector2.right) * 0.5f,
                rayColor);
        }

        // 如果检测到墙壁则掉头
        if ((frontHit.collider != null && frontHit.collider.CompareTag(wallTag)) ||
            (backHit.collider != null && backHit.collider.CompareTag(wallTag)))
        {
            TurnAround();
        }
    }

    // 掉头
    private void TurnAround()
    {
        movingRight = !movingRight;
        // 可以在这里添加掉头动画或效果
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
                Debug.Log("速度恢复正常");
            }
            else
            {
                // 开始加速状态
                currentSpeed = moveSpeed * speedBoostMultiplier;
                speedTimer = boostedSpeedDuration;
                isSpeedBoosted = true;
                Debug.Log("速度提升!");
            }
        }
    }

    // 设置默认边界
    private void SetupDefaultBounds()
    {
        GameObject left = new GameObject("LeftBound");
        left.transform.position = transform.position - Vector3.right * patrolRange;
        left.transform.SetParent(transform.parent);
        leftBound = left.transform;

        GameObject right = new GameObject("RightBound");
        right.transform.position = transform.position + Vector3.right * patrolRange;
        right.transform.SetParent(transform.parent);
        rightBound = right.transform;
    }

    // 在Scene视图中可视化边界
    private void OnDrawGizmos()
    {
        if (drawDebugRays)
        {
            // 绘制巡逻边界
            Gizmos.color = Color.green;
            if (leftBound != null) Gizmos.DrawWireSphere(leftBound.position, 0.2f);
            if (rightBound != null) Gizmos.DrawWireSphere(rightBound.position, 0.2f);

            // 绘制巡逻路径
            if (leftBound != null && rightBound != null)
            {
                Gizmos.DrawLine(leftBound.position, rightBound.position);
            }
        }
    }
}
