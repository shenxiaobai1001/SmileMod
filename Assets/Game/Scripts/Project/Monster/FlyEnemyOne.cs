using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyOne : MonsterBase
{
    public SpriteRenderer Sprite;
    public GameObject enemyDie;

    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;


    Tween tween;

    void Start()
    {
        initialPosition = transform.position;
        tween = transform.DOScale(new Vector3(1,0.8f,1), 1f).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        if (isChasing && playerTarget != null)
        {
            // 追击玩家
            ChasePlayer();

            // 根据玩家位置调整朝向
            UpdateSpriteDirection();
        }
    }

    void ChasePlayer()
    {
        // 向玩家移动
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
    }

    void UpdateSpriteDirection()
    {
        if (playerTarget != null && Sprite != null)
        {
            // 根据玩家在怪物的左右调整朝向
            if (playerTarget.position.x > transform.position.x)
            {
                // 玩家在右边，朝右
                Sprite.flipX = false;
            }
            else
            {
                // 玩家在左边，朝左
                Sprite.flipX = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTarget = collision.transform;
            isChasing = true;
        }
        if (collision.CompareTag("Arrow"))
        {
            Sound.PlaySound("Sound/MonsterDie0Sfx");
            Sprite.DOFade(0, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                Sprite.color = Color.white;
            });
            curHP--;
            if (curHP == 0)
            {
                Instantiate(enemyDie, this.transform.position, Quaternion.identity);
                if(tween!=null) tween.Kill();
                transform.localScale = Vector3.one;
                gameObject.SetActive(false);
            }
        }
    }

    // 可选：如果希望怪物在玩家离开一定距离后返回原位，可以取消注释以下方法
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            ReturnToInitialPosition();
        }
    }

    // 返回初始位置的方法（可选）
    void ReturnToInitialPosition()
    {
        transform.DOMove(initialPosition, 1f).OnComplete(() =>
        {
            // 到达初始位置后的回调
        });
    }
    
}