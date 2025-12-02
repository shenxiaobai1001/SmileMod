using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTurret : MonoBehaviour
{
    public Transform spriteBox;
    public GameObject turret;

    public Vector3 movingVec;
    public float currentSpeed;
    public float interval;
    public float moveEnd;
    void Start()
    {
        StartCoroutine(OnCreateTurret());
    }
    GameObject chitobj;
    IEnumerator OnCreateTurret()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
            }); 
            chitobj = SimplePool.Spawn(turret,transform.position,Quaternion.identity);
            chitobj.SetActive(true);
            var turretC= chitobj.GetComponent<Turret>();
            turretC.OnSetData(movingVec, currentSpeed, moveEnd);
        }
    }
}
