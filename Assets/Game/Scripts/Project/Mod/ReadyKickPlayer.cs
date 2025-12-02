using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyKickPlayer : MonoBehaviour
{
    public float watiTime = 0;
    public Vector3 vector;
    public bool boom = false;
    public bool continuous = false;
    public float continuousTime = 0;

    void OnEnable()
    {
        if (continuous)
        {
            InvokeRepeating("OnHitPlayer", watiTime, continuousTime);
        }
        else
        {
            Invoke("OnHitPlayer", watiTime);
        }
    }
    void OnHitPlayer()
    {
        PlayerModController.Instance.OnKickPlayer(vector, boom);
    }
}
