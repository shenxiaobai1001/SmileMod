using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHand : MonoBehaviour
{
    public GameObject mask;
    int targetTime = 0;
    public void OnSetTime(int time)
    {
        targetTime = time;

    }
    void OnCreateMask()
    {
        mask.SetActive(true);
        Invoke("OnReadyDelete", targetTime);
    }

    void OnReadyDelete()
    {
        Destroy(gameObject);
    }
}
