using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostEnemy : MonsterBase
{
    public SpriteRenderer face;
    public SpriteRenderer faceFire;
    public GameObject FlyObj;
    public GameObject dieObj;

    public float dashSpeed = 10f;
    public float dashDistance = 3f;
    public float waitTime = 2f;

    public bool isDashing = false;

    void Start()
    {
        initialPosition = transform.position;
        curHP = 3;
    }

    void Update()
    {
        if (isChasing && playerTarget != null && !isDashing)
        {
            // 更新面向方向
            UpdateFacingDirection();
        }
    }

    void UpdateFacingDirection()
    {
        if (playerTarget != null)
        {
            // 根据玩家在怪物的左右调整朝向
            if (playerTarget.position.x > transform.position.x)
            {
                // 玩家在右边，朝右
                if (face != null) face.flipX = false;
                if (faceFire != null) faceFire.flipX = false;
            }
            else
            {
                // 玩家在左边，朝左
                if (face != null) face.flipX = true;
                if (faceFire != null) faceFire.flipX = true;
            }
        }
    }

    public void OnStartDash(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isChasing)
        {
            playerTarget = collision.transform;
            isChasing = true;
            StartCoroutine(DashCycle());
        }
    }


    IEnumerator DashCycle()
    {
        while (isChasing && playerTarget != null)
        {
            // 冲刺前生成FlyObj
            if (FlyObj != null)
            {
                Instantiate(FlyObj, transform.position, Quaternion.identity);
            }

            // 开始冲刺
            yield return StartCoroutine(PerformDash());

            // 等待一段时间
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;

        // 计算冲刺方向（朝向玩家）
        Vector3 dashDirection = (playerTarget.position - transform.position).normalized;
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;

        // 使用DoTween进行冲刺
        float dashDuration = Vector3.Distance(transform.position, dashTarget) / dashSpeed;
        transform.DOMove(dashTarget, dashDuration).SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    // 停止追击（可选，比如玩家死亡时调用）
    public void StopChasing()
    {
        isChasing = false;
        StopAllCoroutines();
    }

    // 敌人死亡方法
    public void Die()
    {
        if (dieObj != null)
        {
            Instantiate(dieObj, transform.position, Quaternion.identity);
        }

        StopChasing();
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Arrow"))
        {
            Sound.PlaySound("Sound/MonsterHit3Sfx");
            curHP--;
            if (curHP == 0)
            {
                StopChasing();
                Instantiate(dieObj, this.transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
        }
    }
    void OnDestroy()
    {
        // 清理DoTween
        if (DOTween.IsTweening(transform))
            transform.DOKill();
    }
}