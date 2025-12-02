using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBossCheck : MonoBehaviour
{
    public EndHeadBoss endHead;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            endHead.isLive = true;
            endHead.player = PlayerController.Instance.transform;
            endHead.CheckForPlayer();
        }
    }
}
