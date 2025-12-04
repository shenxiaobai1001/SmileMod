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

    public float waitCreateTurret = 0;
    bool isWait = false;

    private Coroutine createTurretCoroutine; // 保存协程引用

    void Start()
    {
        OnStartCreateTurret();
    }

    void OnStartCreateTurret()
    {
        waitCreateTurret = 0;
        isWait = false;

        // 先停止之前的协程（如果存在）
        if (createTurretCoroutine != null)
        {
            StopCoroutine(createTurretCoroutine);
        }

        // 启动新的协程并保存引用
        createTurretCoroutine = StartCoroutine(OnCreateTurret());
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
            chitobj = SimplePool.Spawn(turret, transform.position, Quaternion.identity);
            chitobj.SetActive(true);
            var turretC = chitobj.GetComponent<Turret>();
            turretC.OnSetData(movingVec, currentSpeed, moveEnd);
        }
    }

    public void OnSetWaitTime(float time)
    {
        waitCreateTurret = time;
        if (!isWait)
        {
            // 使用保存的协程引用来停止
            if (createTurretCoroutine != null)
            {
                StopCoroutine(createTurretCoroutine);
                createTurretCoroutine = null;
            }

            isWait = true;
            Invoke("OnStartCreateTurret", waitCreateTurret);
        }
    }
}