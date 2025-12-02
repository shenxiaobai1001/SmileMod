using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    public int HP = 2;

    public int curHP = 2;

    public Vector3 initialPosition;

    public Transform playerTarget;
    public bool isChasing = false;

    public void OnRest()
    {
        curHP = HP;
        transform.position = initialPosition;
        playerTarget = null;
        isChasing = false;
    }

}
