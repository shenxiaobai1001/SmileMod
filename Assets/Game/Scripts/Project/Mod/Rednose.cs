using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Rednose : MonoBehaviour
{
    [Header("导弹设置")]
    public float speed = 5f; // 飞行速度
    private Transform player; // 玩家引用

    void Start()
    {
 
    }

    void Update()
    {
        if (player == null)
        {
            player = PlayerController.Instance.transform;
            return;
        }
        transform.LookAt(player);

        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        // 检查移动距离是否达到销毁条件
        if (Vector3.Distance(transform.position, player.position) <=0.1f)
        {
            OnHitPlayer();
        }
    }

    // 击中玩家后的处理
    private void OnHitPlayer()
    {
        PlayerModController.Instance.OnKickPlayer(new Vector3(Random.Range(-3, 3), 10),true);
        SimplePool.Despawn(gameObject);
        gameObject.SetActive(false);
    }
}
