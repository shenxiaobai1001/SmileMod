using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public enum ArrowType
    {
        UpArrow,
        DownArrow,
        LeftArrow,
        RightArrow,
    }

    public ArrowType arrowType;
    public GameObject uparrow;
    public GameObject downarrow;
    public GameObject leftarrow;
    public GameObject rightarrow;
    void Start()
    {
        if(uparrow) uparrow.SetActive(arrowType== ArrowType.UpArrow);
        if (downarrow) downarrow.SetActive(arrowType == ArrowType.DownArrow);
        if (leftarrow) leftarrow.SetActive(arrowType == ArrowType.LeftArrow);
        if (rightarrow) rightarrow.SetActive(arrowType == ArrowType.RightArrow);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Player"))
        {
            Sound.PlaySound("Sound/distorKick");
            transform.DOScale(1.6f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
            }); ;
        }

    }

}
