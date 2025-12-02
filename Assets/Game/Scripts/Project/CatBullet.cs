using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatBullet : MonoBehaviour
{
    public Animator animator;
    Transform targetObj;

    [Header("移动设置")]
    public float speed = 20;

    private bool hasHit = false;

    private void Start()
    {
        Invoke("OnReadyDestory",1);
    }
    void OnReadyDestory()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
    }

    // 外部方法：设置目标
    public void SetTarget(Transform target)
    {
        targetObj = target;
        hasHit = false;
    }

    // 向目标移动
    void MoveTowardsTarget()
    {
        if (targetObj == null || hasHit) return;

        // 计算移动方向
        Vector2 direction = (targetObj.position - transform.position).normalized;

        // 移动物体
        transform.position = Vector2.MoveTowards(transform.position, targetObj.position, speed * Time.deltaTime);
    }

    // 碰撞检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // 检查是否碰撞到目标
        if (collision.transform == targetObj)
        {
            hasHit = true;

            // 触发动画
            if (animator != null)
            {
                animator.SetTrigger("Boom"); // 假设动画触发器名为"Explode"
            }

            // 可选：碰撞后一段时间销毁物体
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    // 动画播放后销毁
    IEnumerator DestroyAfterAnimation()
    {
        // 等待动画播放完成（假设动画长度约1秒）
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}