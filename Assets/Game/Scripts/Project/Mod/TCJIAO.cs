using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCJiao : MonoBehaviour
{
    public Transform boomPos;
    public GameObject boom;

    Vector3 startPos;
    float allTime = 1.5f;
    float time = 0;
    void Awake()
    {
        startPos = new Vector3(15, 0);
    }

    public void OnStarMove()
    {
        time = 0;
        transform.localPosition = startPos;
        transform.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() => { OnBeginCreateBoom(); });
    }

    void OnBeginCreateBoom()
    {
        EventManager.Instance.SendMessage(Events.OnTCMove, true);
        InvokeRepeating("OnCreateBoom", 0, 0.15f);
    }

    void OnCreateBoom()
    {
        Sound.PlaySound("Sound/Mod/Boom");
        GameObject bb = SimplePool.Spawn(boom, boomPos.transform.position, Quaternion.identity);
        bb.transform.parent = ItemManager.Instance.transform;
        bb.SetActive(true);
        time += 0.15f;
        if (time > allTime)
        {
            if (IsInvoking("OnBeginCreateBoom"))
            {
                CancelInvoke("OnBeginCreateBoom");
            }
            if (IsInvoking("OnCreateBoom"))
            {
                CancelInvoke("OnCreateBoom");
            }
            EventManager.Instance.SendMessage(Events.OnTCMove, false);
            SimplePool.Despawn(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        if (IsInvoking("OnBeginCreateBoom"))
        {
            CancelInvoke("OnBeginCreateBoom");
        }
        if (IsInvoking("OnCreateBoom"))
        {
            CancelInvoke("OnCreateBoom");
        }
    }
}
