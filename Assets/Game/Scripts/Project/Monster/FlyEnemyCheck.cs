using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyCheck : MonoBehaviour
{
    public FlyEnemyOne dlyEnemyOne;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dlyEnemyOne.playerTarget = collision.transform;
            dlyEnemyOne.isChasing = true;
        }
     
    }
}
