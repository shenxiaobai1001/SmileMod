using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTurretLaser : MonoBehaviour
{
    public Transform spriteBox;
    public GameObject turret;
    public CircleCollider2D circleCollider;
    public float currentSpeed;
    public float interval;
    public float moveEnd;
    [Header("攻击设置")]
    public float attackRange = 5f; // 攻击范围
    public float attackInterval = 2f; // 攻击间隔
    public float detectionInterval = 0.2f; // 检测间隔（性能优化）

    [Header("调试显示")]
    public bool showDebugRange = true; // 是否显示攻击范围

    private bool isPlayerInRange = false;
    private Coroutine attackCoroutine;

    void Start()
    {
        circleCollider.radius=attackRange;
        // 开始检测玩家

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerEnterRange();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerExitRange();
        }
    }

    private void OnPlayerEnterRange()
    {
        isPlayerInRange = true;

        // 开始攻击协程
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private void OnPlayerExitRange()
    {
        isPlayerInRange = false;

        // 停止攻击协程
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    IEnumerator AttackRoutine()
    {
        while (isPlayerInRange)
        {
            // 等待攻击间隔
            yield return new WaitForSeconds(attackInterval);

            // 再次确认玩家仍在范围内
            if (isPlayerInRange)
            {
                transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    transform.localScale = Vector3.one;
                });
                LaunchTurret();
            }
        }
    }

    private void LaunchTurret()
    {
        // 计算朝向玩家的方向
        Vector3 attackDirection = (PlayerController.Instance.transform.position - transform.position).normalized;

        // 创建炮塔
        GameObject turretObj = Instantiate(turret, transform.position, Quaternion.identity);
        var turretComponent = turretObj.GetComponent<Turret>();

        if (turretComponent != null)
        {
            // 将玩家位置或方向传递给turret
            turretComponent.OnSetData(attackDirection, currentSpeed, moveEnd);
        }
    }

    // 调试显示攻击范围
    private void OnDrawGizmosSelected()
    {
        if (showDebugRange)
        {
            Gizmos.color = isPlayerInRange ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

}