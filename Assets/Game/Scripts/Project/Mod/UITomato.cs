using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITomato : MonoBehaviour
{
    public GameObject hitObj;
    public Transform gaiya;

    void Start()
    {
        gaiya.gameObject.SetActive(false);
        hitObj.gameObject.SetActive(false);
        EventManager.Instance.AddListener(Events.GaiyaTomato, OnOpenGaiYa);
        EventManager.Instance.AddListener(Events.GaiyaTomatoEnd, GaiyaTomatoEnd);
    }

   void OnOpenGaiYa(object msg)
    {
        hitObj.SetActive(true);
        gaiya.gameObject.SetActive(true);
        gaiya.localScale = new Vector3(0.1f,0.1f,1);
        gaiya.DOScale(new Vector3(0.5f, 0.5f, 1), 2);
        Invoke("OnCloseGaiya",3);
    }
    void OnCloseGaiya()
    {
        gaiya.gameObject.SetActive(false);
    }
    void GaiyaTomatoEnd(object msg)
    {
        hitObj.gameObject.SetActive(false);
    }
}
