using UnityEngine;
using System.Collections.Generic;

public class Sawwer : MonoBehaviour
{
    public enum SawType { Saw1, Saw2 }

    [Header("齿轮设置")]
    public SawType sawType = SawType.Saw1; // 选择显示的齿轮
    public float moveSpeed = 3f;          // 移动速度
    public float rotationSpeed = 180f;    // 旋转速度(度/秒)
    public Vector2 pointOffset1 = new Vector2(-5f, 0f); // 点位1偏移
    public Vector2 pointOffset2 = new Vector2(5f, 0f);  // 点位2偏移

    [Header("路径显示")]
    public Transform linePointsParent;    // 路径点父物体
    public GameObject linePointPrefab;    // 路径点预制体
    public float linePointSpacing = 1f;   // 路径点间距

    [Header("组件引用")]
    public GameObject saw1;              // 齿轮1
    public GameObject saw2;              // 齿轮2
    public GameObject collider1;        // 碰撞体1
    public GameObject collider2;        // 碰撞体2
    public GameObject Point1;        // 碰撞体1
    public GameObject Point2;        // 碰撞体2

    private Vector3 point1;              // 实际点位1
    private Vector3 point2;              // 实际点位2
    private bool movingToPoint2 = true;  // 移动方向标志
    private List<GameObject> linePoints = new List<GameObject>(); // 存储路径点

    private GameObject activeSaw;
    private GameObject activeCollider;

    private void Start()
    {
        // 初始化点位
        point1 = transform.position + (Vector3)pointOffset1;
        point2 = transform.position + (Vector3)pointOffset2;
        Point1.transform.position = point1;
        Point2.transform.position = point2;
        // 设置初始显示的齿轮
        SetActiveSaw(sawType);

        // 生成路径点
        GeneratePathPoints();
        linePointPrefab.SetActive(false);
        activeSaw = sawType == SawType.Saw1 ? saw1 : saw2;
        activeCollider = sawType == SawType.Saw1 ? collider1 : collider2;
    }

    private void Update()
    {
        // 旋转齿轮
        RotateSaw();

        // 移动齿轮
        MoveSaw();
    }

    // 设置当前显示的齿轮
    private void SetActiveSaw(SawType type)
    {
        saw1.SetActive(type == SawType.Saw1);
        saw2.SetActive(type == SawType.Saw2);
        collider1.SetActive(type == SawType.Saw1);
        collider2.SetActive(type == SawType.Saw2);
    }

    // 旋转齿轮
    private void RotateSaw()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        if (sawType == SawType.Saw1)
        {
            saw1.transform.Rotate(0f, 0f, -rotationAmount);
        }
        else
        {
            saw2.transform.Rotate(0f, 0f, rotationAmount);
        }
    }

    // 移动齿轮
    private void MoveSaw()
    {
        Vector3 targetPoint = movingToPoint2 ? Point2.transform.localPosition : Point1.transform.localPosition;
        Vector3 currentPosition = activeSaw.transform.localPosition;

        // 移动齿轮和碰撞体
        activeSaw.transform.localPosition = Vector3.MoveTowards(
            currentPosition,
            targetPoint,
            moveSpeed * Time.deltaTime);

        activeCollider.transform.position = activeSaw.transform.position;

        // 检查是否到达目标点
        if (Vector3.Distance(currentPosition, targetPoint) < 0.1f)
        {
            movingToPoint2 = !movingToPoint2;
        }
    }

    // 生成路径点
    private void GeneratePathPoints()
    {
        if (linePointPrefab == null || linePointsParent == null) return;

        // 计算路径总长度和点数
        float pathLength = Vector3.Distance(point1, point2);
        int pointCount = Mathf.FloorToInt(pathLength / linePointSpacing);

        // 清除旧的点
        foreach (GameObject point in linePoints)
        {
            Destroy(point);
        }
        linePoints.Clear();

        // 生成新的点
        for (int i = 0; i <= pointCount; i++)
        {
            float t = i / (float)pointCount;
            Vector3 pointPosition = Vector3.Lerp(point1, point2, t);

            GameObject point = Instantiate(
                linePointPrefab,
                pointPosition,
                Quaternion.identity,
                linePointsParent);

            linePoints.Add(point);
        }
    }

    // 新增：设置activeSaw和activeCollider本地位置的方法
    public void SetSawLocalPosition(Vector3 localPosition, bool moveToPoint2)
    {
        if (activeSaw != null)
        {
            activeSaw.transform.localPosition = localPosition;
        }

        if (activeCollider != null)
        {
            activeCollider.transform.localPosition = localPosition;
        }
        movingToPoint2 = moveToPoint2;
    }

    // 新增：重置到起始点的方法
    public void ResetToStartPoint()
    {
        if (activeSaw != null)
        {
            activeSaw.transform.localPosition = Point1.transform.localPosition;
        }

        if (activeCollider != null)
        {
            activeCollider.transform.position = activeSaw.transform.position;
        }

        movingToPoint2 = true;
    }

    // 新增：更新Point1和Point2的位置（保持相对偏移）
    private void UpdatePointPositions()
    {
        if (Point1 != null && Point2 != null)
        {
            // 根据当前activeSaw的位置重新计算Point1和Point2的位置
            Vector3 currentPos = activeSaw != null ? activeSaw.transform.position : transform.position;

            point1 = currentPos + (Vector3)pointOffset1;
            point2 = currentPos + (Vector3)pointOffset2;

            Point1.transform.position = point1;
            Point2.transform.position = point2;

            // 重新生成路径点
            GeneratePathPoints();
        }
    }

    // 在编辑器中可视化点位
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)pointOffset1, 0.3f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)pointOffset2, 0.3f);
        Gizmos.DrawLine(
            transform.position + (Vector3)pointOffset1,
            transform.position + (Vector3)pointOffset2);
    }

    // 编辑器工具：更新路径点
    [ContextMenu("更新路径点")]
    private void UpdatePathPoints()
    {
        point1 = transform.position + (Vector3)pointOffset1;
        point2 = transform.position + (Vector3)pointOffset2;

        if (Application.isPlaying)
        {
            GeneratePathPoints();
        }
    }
}