using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrunck : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image image;
    Tween tween1;
    Tween tween2;
    bool isChange = false;
    void Start()
    {
        canvasGroup.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
        EventManager.Instance.AddListener(Events.OnTrunckMove, OnTrcunckMove);
    }

    void OnTrcunckMove(object msg)
    {
        canvasGroup.gameObject.SetActive(true);
        image.gameObject.SetActive(true);

        if (!isChange)
        {
            canvasGroup.alpha = 1;
            image.color = Color.white;
            isChange = true;
            tween1= canvasGroup.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            tween2= image.DOFade(0, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        Invoke("OnRest",4);
    }

    void OnRest()
    {
        isChange = false;
        tween1.Pause(); 
        tween2.Pause();
        tween1.Kill();
        tween2.Kill();
        canvasGroup.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
    }
}
