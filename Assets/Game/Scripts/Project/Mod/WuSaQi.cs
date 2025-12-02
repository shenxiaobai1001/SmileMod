using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WuSaQi : MonoBehaviour
{
    public float moveSpeed = 3f;
    public GameObject spriteRenderer;

    bool kickPlayer = true;
    Transform playerTarget;
    bool isMove = false;
    void Start()
    {
        playerTarget = PlayerController.Instance.transform;
    }

    void Update()
    {
        if (!isMove) return;
        if (kickPlayer)
        {
            ChasePlayer();
        }
    }

    // 外部方法：设置初始位置
    public void StartMove( )
    {
        spriteRenderer.SetActive(false);
        isMove = true;
        kickPlayer = true;
    }

    void ChasePlayer()
    {
        // 向玩家移动
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, playerTarget.position) < 0.1f)
        {
            Sound.PlaySound("Sound/Mod/dao");
            spriteRenderer.SetActive(true);
            kickPlayer = false;
            bool protect = ModSystemController.Instance.Protecket;
            if (!protect)
                PlayerModController.Instance.OnRightHitPlayer();
            Invoke("OnClose",0.5f);
        }
    }

    void OnClose()
    {
        SimplePool.Despawn(gameObject);
        gameObject.SetActive(false);
    }

}
