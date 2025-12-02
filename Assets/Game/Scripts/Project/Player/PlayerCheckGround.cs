using UnityEngine;

public class PlayerCheckGround : MonoBehaviour
{
    [Header("射线设置")]
    public float rayLength1 = 5f;           // 射线长度
    public float rayLength2 = 5f;           // 射线长度
    public float rayLength3 = 5f;           // 射线长度
    public float rayLength4 = 5f;           // 射线长度
    public float rayLength5 = 5f;           // 射线长度
    public float rayLength6 = 5f;           // 射线长度
    public float rayLength7= 5f;           // 射线长度
    public float rayLength = 5f;           // 射线长度
    public float angleOffset = 30f;       // 斜射线角度偏移（默认30°）
    public Color rayColor = Color.red;     // 调试射线颜色
    public LayerMask detectionLayer;       // 检测层级（可选）
    public Vector2 RayAngle = Vector2.down;


    [Header("检测结果")]
    public bool hitCenter;                 // 垂直射线是否碰到物体
    public bool hitLeft;                   // 左侧斜射线是否碰到物体
    public bool hitRight;                  // 右侧斜射线是否碰到物体
    public bool hitLeftfar;               // 左侧更斜的射线是否碰到物体
    public bool hitRightfar;              // 右侧更斜的射线是否碰到物体
    public bool hitLeftFar;               // 左侧更斜的射线是否碰到物体
    public bool hitRightFar;              // 右侧更斜的射线是否碰到物体
    public RaycastHit2D centerHit;         // 垂直射线碰撞信息
    public RaycastHit2D leftHit;          // 左侧斜射线碰撞信息
    public RaycastHit2D rightHit;         // 右侧斜射线碰撞信息
    public RaycastHit2D leftfarHit;          // 左侧斜射线碰撞信息
    public RaycastHit2D rightfarHit;         // 右侧斜射线碰撞信息
    public RaycastHit2D leftFarHit;       // 左侧更斜的射线碰撞信息
    public RaycastHit2D rightFarHit;     // 右侧更斜的射线碰撞信息

    public bool isGround = false;
    public bool isHit = false;

    void Update()
    {
        Vector2 origin = transform.position;

        // 1. 垂直向下射线
        centerHit = Physics2D.Raycast(origin, RayAngle, rayLength1, detectionLayer);
        hitCenter = centerHit.collider != null;

        // 2. 左侧斜射线（-angleOffset°）
        Vector2 leftDir = Quaternion.Euler(0, 0, -angleOffset) * RayAngle;
        leftHit = Physics2D.Raycast(origin, leftDir, rayLength2, detectionLayer);
        hitLeft = leftHit.collider != null;

        // 3. 右侧斜射线（+angleOffset°）
        Vector2 rightDir = Quaternion.Euler(0, 0, angleOffset) * RayAngle;
        rightHit = Physics2D.Raycast(origin, rightDir, rayLength3, detectionLayer);
        hitRight = rightHit.collider != null;       
        // 2. 左侧斜射线（-angleOffset°）
        Vector2 leftfarDir = Quaternion.Euler(0, 0, -angleOffset*2) * RayAngle;
        leftHit = Physics2D.Raycast(origin, leftfarDir, rayLength4, detectionLayer);
        hitLeftfar = leftfarHit.collider != null;

        // 3. 右侧斜射线（+angleOffset°）
        Vector2 rightafrDir = Quaternion.Euler(0, 0, angleOffset*2) * RayAngle;
        rightHit = Physics2D.Raycast(origin, rightafrDir, rayLength5, detectionLayer);
        hitRightfar = rightfarHit.collider != null;

        // 4. 左侧更斜的射线（-angleOffset * 2°）
        Vector2 leftFarDir = Quaternion.Euler(0, 0, -angleOffset * 3) * RayAngle;
        leftFarHit = Physics2D.Raycast(origin, leftFarDir, rayLength6, detectionLayer);
        hitLeftFar = leftFarHit.collider != null;

        // 5. 右侧更斜的射线（+angleOffset * 2°）
        Vector2 rightFarDir = Quaternion.Euler(0, 0, angleOffset * 3) * RayAngle;
        rightFarHit = Physics2D.Raycast(origin, rightFarDir, rayLength7, detectionLayer);
        hitRightFar = rightFarHit.collider != null;

        isHit = hitCenter || hitLeft || hitRight || hitLeftFar || hitRightFar || hitLeftfar || hitRightfar;

        // 任意一条射线碰到地面都算接地
        isGround = hitCenter || hitLeft || hitRight || hitLeftFar || hitRightFar || hitLeftfar || hitRightfar;
    }

    // 在 Scene 视图中绘制调试射线
    void OnDrawGizmos()
    {
        Gizmos.color = rayColor;
        Vector2 origin = transform.position;

        // 1. 绘制垂直射线
        Vector2 centerEnd = origin + RayAngle * rayLength1;
        Gizmos.DrawLine(origin, centerEnd);
        if (hitCenter) Gizmos.DrawSphere(centerHit.point, 0.1f);

        // 2. 绘制左侧斜射线（-angleOffset°）
        Vector2 leftDir = Quaternion.Euler(0, 0, -angleOffset) * RayAngle;
        Vector2 leftEnd = origin + leftDir * rayLength2;
        Gizmos.DrawLine(origin, leftEnd);
        if (hitLeft) Gizmos.DrawSphere(leftHit.point, 0.1f);

        // 3. 绘制右侧斜射线（+angleOffset°）
        Vector2 rightDir = Quaternion.Euler(0, 0, angleOffset) * RayAngle;
        Vector2 rightEnd = origin + rightDir * rayLength3;
        Gizmos.DrawLine(origin, rightEnd);
        if (hitRight) Gizmos.DrawSphere(rightHit.point, 0.1f);

        // 2. 绘制左侧斜射线（-angleOffset°）
        Vector2 leftfarDir = Quaternion.Euler(0, 0, -angleOffset*2) * RayAngle;
        Vector2 leftfarEnd = origin + leftfarDir * rayLength4;
        Gizmos.DrawLine(origin, leftfarEnd);
        if (hitLeftfar) Gizmos.DrawSphere(leftfarHit.point, 0.1f);

        // 3. 绘制右侧斜射线（+angleOffset°）
        Vector2 rightfarDir = Quaternion.Euler(0, 0, angleOffset*2) * RayAngle;
        Vector2 rightfarEnd = origin + rightfarDir * rayLength5;
        Gizmos.DrawLine(origin, rightfarEnd);
        if (hitRightfar) Gizmos.DrawSphere(rightfarHit.point, 0.1f);

        // 4. 绘制左侧更斜的射线（-angleOffset * 2°）
        Vector2 leftFarDir = Quaternion.Euler(0, 0, -angleOffset * 3) * RayAngle;
        Vector2 leftFarEnd = origin + leftFarDir * rayLength6;
        Gizmos.DrawLine(origin, leftFarEnd);
        if (hitLeftFar) Gizmos.DrawSphere(leftFarHit.point, 0.1f);

        // 5. 绘制右侧更斜的射线（+angleOffset * 2°）
        Vector2 rightFarDir = Quaternion.Euler(0, 0, angleOffset * 3) * RayAngle;
        Vector2 rightFarEnd = origin + rightFarDir * rayLength7;
        Gizmos.DrawLine(origin, rightFarEnd);
        if (hitRightFar) Gizmos.DrawSphere(rightFarHit.point, 0.1f);
    }
}