using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostEF : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        OnFadeSprite();
    }
    void OnFadeSprite()
    {
        spriteRenderer.DOFade(0,0.5f).OnComplete(() => {
            spriteRenderer.color = Color.white;
            SimplePool.Despawn(gameObject);
            gameObject.SetActive(false);
        });
    }
}
