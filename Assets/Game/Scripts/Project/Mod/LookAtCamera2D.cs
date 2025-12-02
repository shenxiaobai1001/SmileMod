using UnityEngine;

public class LookAtCamera2D : MonoBehaviour
{
    [Header("摄像头设置")]
    public Camera targetCamera;
    public bool lookAway = false; // 是否反向看向摄像头

    [Header("旋转轴设置")]
    public bool freezeX = true; // 2D通常冻结X轴
    public bool freezeY = true; // 2D通常冻结Y轴
    public bool freezeZ = false; // 2D通常只旋转Z轴

    [Header("更新设置")]
    public UpdateMethod updateMethod = UpdateMethod.LateUpdate;

    public enum UpdateMethod
    {
        Update,
        LateUpdate,
        FixedUpdate
    }

    private Vector3 originalRotation;

    void Start()
    {
        // 保存原始旋转
        originalRotation = transform.eulerAngles;

        // 如果没有指定摄像机，使用主摄像机
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError("没有找到摄像机！");
            this.enabled = false;
        }
    }

    void Update()
    {
        if (updateMethod == UpdateMethod.Update)
        {
            LookAtCamera();
        }
    }

    void LateUpdate()
    {
        if (updateMethod == UpdateMethod.LateUpdate)
        {
            LookAtCamera();
        }
    }

    void FixedUpdate()
    {
        if (updateMethod == UpdateMethod.FixedUpdate)
        {
            LookAtCamera();
        }
    }

    void LookAtCamera()
    {
        if (targetCamera == null) return;

        // 计算朝向摄像头的方向
        Vector3 direction = targetCamera.transform.position - transform.position;

        // 2D情况下主要计算Z轴旋转
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 如果需要反向看，旋转180度
        if (lookAway)
        {
            angle += 180f;
        }

        // 应用旋转，考虑冻结的轴
        Vector3 newRotation = new Vector3(
            freezeX ? originalRotation.x : angle,
            freezeY ? originalRotation.y : angle,
            freezeZ ? originalRotation.z : angle
        );

        transform.rotation = Quaternion.Euler(newRotation);
    }

    // 公共方法：动态设置目标摄像头
    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
    }

    // 公共方法：临时禁用/启用面向摄像头
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }
}