using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailFollower : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target; // 跟随的目标
    public float followDistance = 5f; // 开始跟随的距离阈值
    public float normalSpeed = 3f; // 正常跟随速度
    public float slowSpeed = 1f; // 缓慢移动速度
    public float stoppingDistance = 0.5f; // 停止距离

    [Header("调试")]
    public bool showGizmos = true; // 是否显示调试图形

    private bool isFollowing = false; // 是否正在跟随
    private bool targetIsMoving = false; // 目标是否在移动
    private Vector3 lastTargetPosition; // 上一帧目标位置
    private float targetMoveThreshold = 0.1f; // 判断目标移动的最小距离阈值

    void Start()
    {
        // 初始化目标位置
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    void Update()
    {
        if (target == null) return;

        // 检查目标是否在移动
        CheckTargetMovement();

        // 计算与目标的距离
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 判断是否应该开始跟随
        if (!isFollowing && distanceToTarget > followDistance)
        {
            isFollowing = true;
        }

        // 跟随逻辑
        if (isFollowing)
        {
            // 如果距离已经很近，停止跟随
            if (distanceToTarget <= stoppingDistance)
            {
                isFollowing = false;
                return;
            }

            // 选择移动速度：如果目标在移动则用正常速度，否则用慢速
            float currentSpeed = targetIsMoving ? normalSpeed : slowSpeed;

            // 计算移动方向
            Vector3 direction = (target.position - transform.position).normalized;

            // 移动物体
            transform.position = Vector3.Lerp(transform.position, target.position, currentSpeed * Time.deltaTime);

            // 可选：使物体朝向移动方向（如果是2D游戏）
            // if (direction != Vector3.zero)
            // {
            //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //     transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            // }
        }
    }

    // 检查目标是否在移动
    void CheckTargetMovement()
    {
        float moveDistance = Vector3.Distance(target.position, lastTargetPosition);
        targetIsMoving = moveDistance > targetMoveThreshold;
        lastTargetPosition = target.position;
    }

    // 在Scene视图中显示调试信息
    void OnDrawGizmos()
    {
        if (!showGizmos || target == null) return;

        // 绘制到目标的线
        Gizmos.color = isFollowing ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, target.position);

        // 绘制跟随距离范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        // 绘制停止距离范围
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // 在目标位置绘制图标
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(target.position, Vector3.one * 0.5f);
    }

    // 公共方法：强制开始跟随
    public void StartFollowing()
    {
        isFollowing = true;
    }

    // 公共方法：强制停止跟随
    public void StopFollowing()
    {
        isFollowing = false;
    }

    // 公共方法：设置新的目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }
}
