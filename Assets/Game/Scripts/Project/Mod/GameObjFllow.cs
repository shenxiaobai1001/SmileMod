using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjFllow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;              // 要跟随的目标物体
    public Vector3 offset = new Vector3(0f, -4.5f, 3.5f); // 相对偏移量
    public float followSpeed = 5f;       // 跟随速度
    public bool smoothFollow = true;     // 是否平滑跟随

    void LateUpdate()
    {
        if (target == null)
        {
            target = PlayerController.Instance.transform;
            Debug.LogWarning("跟随目标未设置!");
            return;
        }

        // 计算目标位置（考虑目标的旋转）
        Vector3 targetPosition = target.position + offset;

        // 移动跟随物体
        if (smoothFollow)
        {
            // 平滑跟随
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // 直接跟随
            transform.position = targetPosition;
        }
    }
}
