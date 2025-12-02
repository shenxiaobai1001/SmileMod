using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vcode : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject meteorPrefab1;
    public GameObject meteorPrefab2;
    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;

    Tween tween;
    // Start is called before the first frame update
    void Start()
    {
        tween = spriteRenderer.DOFade(0, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player")) 
        {
            tween.Kill();
            spriteRenderer.gameObject.SetActive(false);
            animator.SetTrigger("Eat");
            MeteorTrailEffect meteor1 = Instantiate(meteorPrefab1, pos1.position, Quaternion.identity).GetComponent<MeteorTrailEffect>();
            MeteorTrailEffect meteor2 = Instantiate(meteorPrefab2, pos2.position, Quaternion.identity).GetComponent<MeteorTrailEffect>();
            MeteorTrailEffect meteor3 = Instantiate(meteorPrefab1, pos3.position, Quaternion.identity).GetComponent<MeteorTrailEffect>();
            MeteorTrailEffect meteor4 = Instantiate(meteorPrefab2, pos4.position, Quaternion.identity).GetComponent<MeteorTrailEffect>();

            meteor1.SetTarget(UIVCodeGet.Instance.effectPos.position);
            meteor2.SetTarget(UIVCodeGet.Instance.effectPos.position);
            meteor3.SetTarget(UIVCodeGet.Instance.effectPos.position);
            meteor4.SetTarget(UIVCodeGet.Instance.effectPos.position);
            Invoke("OnReadyDestory",0.6f);
        }
    }
    void OnReadyDestory()
    {
        Destroy(this.gameObject);
    }
}
