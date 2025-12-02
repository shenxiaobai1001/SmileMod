using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("火箭移动设置")]
    public float moveSpeed = 30;                // 移动速度
    public float detectionRange = 50f;          // Boss检测范围
    public float explosionRadius = 2f;          // 爆炸半径
    public float arrivalThreshold = 0.5f;      // 到达阈值
    public int missileType;

    private Transform bossTarget;               // Boss目标
    private bool isChasing = false;             // 是否正在追击
    private bool hasExploded = false;           // 是否已爆炸
    private Vector3 startPosition;              // 起始位置
    private Quaternion startRotation;           // 起始旋转

    void Start()
    {
        // 保存初始状态
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (isChasing && bossTarget != null && !hasExploded)
        {
            ChaseBoss();
        }
    }

    /// <summary>
    /// 开始追击Boss
    /// </summary>
    public void StartChasingBoss(Transform bossTransform)
    {
        bossTarget = bossTransform;
        isChasing = true;
        hasExploded = false;

    }

    /// <summary>
    /// 直接冲向Boss的逻辑（无旋转）
    /// </summary>
    private void ChaseBoss()
    {
        // 计算2D方向和距离
        Vector2 direction = (bossTarget.position - transform.position);
        float distance = direction.magnitude;

        // 直接朝Boss移动（使用Vector2.MoveTowards）
        transform.position = Vector2.MoveTowards(transform.position, bossTarget.position, moveSpeed * Time.deltaTime);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // 减去90度让上方向指向目标
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
    }


    /// <summary> 
    /// 到达Boss时的处理 
    /// </summary>
    private void OnReachBoss()
    {
        if (hasExploded) return;
        PFunc.Log("到达boss");
        hasExploded = true;
        isChasing = false;

        // 对Boss造成伤害
        DamageBoss();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 对Boss造成伤害 
    /// </summary>
    private void DamageBoss()
    {
        if (missileType == 1)
            FiveHeadBoss.Instance.OnHitDemage();
        else
        {
            FiveHeadBoss.Instance.OnHitDemage2();
        }
    }

    /// <summary>
    /// 重置火箭到初始状态
    /// </summary>
    public void ResetRocket()
    {
        isChasing = false;
        hasExploded = false;
        bossTarget = null;

        // 重置位置和旋转
        transform.position = startPosition;
        transform.rotation = startRotation;
        gameObject.SetActive(true);
        CancelInvoke();
    }

    /// <summary>
    /// 在Scene视图中绘制调试信息
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 绘制检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 绘制爆炸范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // 绘制移动方向
        if (isChasing && bossTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, bossTarget.position);
        }
    }

    /// <summary>
    /// 碰撞检测（备用触发方式）
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isChasing && !hasExploded && other.CompareTag("Boss"))
        {
            OnReachBoss();
        }
    }
}