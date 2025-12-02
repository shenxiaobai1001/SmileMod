using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookEnemy : MonsterBase
{
    public GameObject backGround;
    public Transform eyePos;
    public SpriteRenderer eyeSprite;
    public CircleCollider2D circleCollider;
    public GameObject enemyDie;
    public GameObject center;
    public float eyeRadius = 0.5f; // 眼睛距离中心的固定距离

    private Vector3 initialEyeLocalPos; // 眼睛初始本地位置

    int health = 2;
    bool check = true;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        // 查找玩家对象
        playerTarget = PlayerController.Instance.transform;

        // 记录眼睛初始本地位置
        if (eyePos != null)
        {
            initialEyeLocalPos = eyePos.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget != null && eyePos != null)
        {
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        // 计算从敌人中心指向玩家的方向向量
        Vector3 directionToPlayer = PlayerController.Instance.transform.position - transform.position;

        // 如果是2D游戏，可能需要忽略Z轴
        directionToPlayer.z = 0;

        // 归一化方向向量并乘以固定距离
        Vector3 eyeOffset = directionToPlayer.normalized * eyeRadius;

        // 设置眼睛的位置（相对于敌人中心）
        eyePos.position = transform.position + eyeOffset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!check) return;
        if (!collision) return;

        if (collision.CompareTag("Arrow"))
        {
            Sound.PlaySound("Sound/MonsterHit2Sfx");
            eyeSprite.DOFade(0, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                eyeSprite.color = Color.white;
            });
            curHP--;
            if (curHP == 0) {
                Instantiate(enemyDie, this.transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
        }
    }
}