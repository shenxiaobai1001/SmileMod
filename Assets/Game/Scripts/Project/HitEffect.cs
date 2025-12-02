using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public GameObject image1;
    public GameObject image2;
    // Start is called before the first frame update
    void OnEnable()
    {
        image1.SetActive(true);
        image2.SetActive(false);
        Invoke("OnChangeImage",0.1f);
        Invoke("OnRedyDestroy", 0.2f);
    }
    void OnChangeImage()
    {
        image1.SetActive(false);
        image2.SetActive(true);
    }
    private void OnRedyDestroy()
    {
        SimplePool.Despawn(gameObject);
        gameObject.SetActive(false);
    }
}
