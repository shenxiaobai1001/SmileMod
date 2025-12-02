using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostBattery : MonoBehaviour
{
    public Transform sprite;
    public Animator animatorEffect;
    public float restTime = 5;
    float swayAngle = 15f;         // 摇摆角度
    float swayDuration = 1f;      // 单次摇摆时间
    bool isRest = false;

    // Start is called before the first frame update
    void Start()
    {
        OnRest();
    }

    void OnRest()
    {
        animatorEffect.gameObject.SetActive(false);
        // 创建旋转摇摆
        sprite.DORotate(new Vector3(0, 0, swayAngle), swayDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        isRest = false;
        sprite.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!isRest)
        {
            animatorEffect.gameObject.SetActive(true); 
            animatorEffect.SetTrigger("Boost");
            sprite.gameObject.SetActive(false);
            isRest = true;
        }
        Invoke("OnRest", restTime);

    }


}
