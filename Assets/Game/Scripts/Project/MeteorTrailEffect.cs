using UnityEngine;
using System.Collections.Generic;

public class MeteorTrailEffect : MonoBehaviour
{
    [Header("基础设置")]
    public Sprite trailPointSprite;       // 圆点精灵图片
    int trailPointsCount = 5;      // 拖尾点数
    float spacing = 3;          // 点间距
    float scaleReduction = 0.2f;   // 每点缩放减少量
    float alphaReduction = 0.2f;   // 每点透明度减少量

    [Header("移动设置")]
    float moveSpeed = 50;          // 移动速度
    public float rotationSpeed = 180f;    // 旋转速度(度/秒)
    public float trailSmoothness = 10f;    // 拖尾跟随平滑度

    private Transform headPoint;           // 头部圆点
    private List<Transform> trailPoints = new List<Transform>(); // 拖尾点列表
    private List<SpriteRenderer> trailRenderers = new List<SpriteRenderer>();
    private Vector3 targetPosition = Vector3.zero;        // 目标位置
    private Vector3 lastMoveDirection = Vector3.right;    // 记录上一帧移动方向

    bool canMove = false;
    void Start()
    {
        CreateTrailEffect();
    }

    void Update()
    {
        if (canMove)
        {
            MoveToTarget();
            UpdateTrailPoints();
        }

    }

    // 初始化拖尾效果
    private void CreateTrailEffect()
    {
        // 创建头部圆点
        headPoint = CreateTrailPoint(transform.position, 1f, 1f).transform;
        headPoint.name = "HeadPoint";

        // 创建拖尾圆点
        for (int i = 0; i < trailPointsCount; i++)
        {
            float scale = 1f - (i + 1) * scaleReduction;
            float alpha = 1f - (i + 1) * alphaReduction;

            // 初始位置沿移动方向后方排列
            Vector3 spawnPos = transform.position/* - lastMoveDirection * spacing * (i + 1)*/;
            GameObject point = CreateTrailPoint(spawnPos, scale, alpha);

            trailPoints.Add(point.transform);
            trailRenderers.Add(point.GetComponent<SpriteRenderer>());
        }
        Invoke("CanMoveToTarget",0.5f);
    }

    void CanMoveToTarget()
    {
        canMove = true;
    }

    // 创建单个拖尾点
    private GameObject CreateTrailPoint(Vector3 position, float scale, float alpha)
    {
        GameObject point = new GameObject("TrailPoint");
        point.transform.position = position;
        point.transform.parent = transform;

        SpriteRenderer renderer = point.AddComponent<SpriteRenderer>();
        renderer.sprite = trailPointSprite;
        renderer.color = new Color(1, 1, 1, alpha);
        renderer.sortingOrder = -1; // 确保在头部后面

        point.transform.localScale = Vector3.one * scale;

        return point;
    }

    // 移动向目标
    private void MoveToTarget()
    {
        if (targetPosition == Vector3.zero) return;

        // 计算移动方向
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // 更新移动方向（如果正在移动）
        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }

        // 移动头部
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        // 旋转头部制造动态效果（根据移动方向）
        headPoint.Rotate(0, 0, rotationSpeed * Time.deltaTime * (moveDirection.x > 0 ? 1 : -1));

        // 检查是否到达目标
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            OnReachTarget();
        }
    }

    // 更新拖尾点位置和外观
    private void UpdateTrailPoints()
    {
        for (int i = 0; i < trailPoints.Count; i++)
        {
            // 前一个点的位置（头部是第一个参考点）
            Vector3 prevPointPos = i == 0 ? headPoint.position : trailPoints[i - 1].position;

            // 计算理想位置（沿移动方向后方）
            Vector3 desiredPosition = prevPointPos - lastMoveDirection * spacing;

            // 平滑移动到理想位置
            trailPoints[i].position = Vector3.Lerp(
                trailPoints[i].position,
                desiredPosition,
                trailSmoothness * Time.deltaTime);

            // 更新旋转（跟随头部）
            trailPoints[i].rotation = headPoint.rotation;
        }
    }

    // 设置目标位置
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        // 初始化移动方向
        if (target != Vector3.zero)
        {
            lastMoveDirection = (target - transform.position).normalized;
        }
    }

    // 到达目标时的处理
    private void OnReachTarget()
    {
        // 可以在这里添加爆炸效果等
        Destroy(gameObject);
    }

    // 编辑器工具：预览拖尾效果
    [ContextMenu("Preview Trail")]
    private void PreviewTrail()
    {
        // 清除旧的点
        foreach (Transform point in trailPoints)
        {
            if (point != null) DestroyImmediate(point.gameObject);
        }
        trailPoints.Clear();
        trailRenderers.Clear();

        if (headPoint != null) DestroyImmediate(headPoint.gameObject);

        // 重新创建
        CreateTrailEffect();
    }
}