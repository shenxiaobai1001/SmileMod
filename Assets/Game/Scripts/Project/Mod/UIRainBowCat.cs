using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRainBowCat : MonoBehaviour
{
    public Image catImag;
    public float destroyDistance = 10f; // 移动多远后销毁
    Tween tween;
    void Start()
    {
        catImag.gameObject.SetActive(false);
        EventManager.Instance.AddListener(Events.RainBowCat, RainBowCat);
    }

    void RainBowCat(object msg)
    { 
        catImag.gameObject.SetActive(true);
        catImag.color = Color.white;
        tween = catImag.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        Invoke("OnCloseDoFade",3);
    }
     
    void OnCloseDoFade()
    {
        if (tween!=null) {
            tween.Pause();
            tween.Kill();
        }

        catImag.color = Color.white;
        catImag.gameObject.SetActive(false);
    }
}
