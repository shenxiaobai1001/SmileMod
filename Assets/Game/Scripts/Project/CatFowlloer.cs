using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatFowlloer : MonoBehaviour
{
    public static CatFowlloer Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public Transform target;            
    public SpriteRenderer spriteRenderer;
    public Vector3 offset = new Vector3(0f, -4.5f, 3.5f); 
    public float followSpeed = 5f;     
    public bool smoothFollow = true;
    public GameObject CatBullet;

    List<GameObject> atkTargets = new List<GameObject>();

    Vector3 waitPos = new Vector3(2765, 0);

    bool isFollow = false;
    bool isAtk=false;

    private void Start()
    {
        EventManager.Instance.AddListener(Events.GameRest, GameRest);
    }
    void GameRest(object msg)
    {
        if (GameController.Instance.gameLevel==7) {
            isFollow = true;
            transform.position = PlayerController.Instance.transform.position;
            if(target==null) 
                target = PlayerController.Instance.transform;
        }
        else
        {
            transform.position = waitPos;
        }
        isAtk = false;
        atkTargets.Clear();
        StopAllCoroutines();
    }
    IEnumerator OnATK()
    {
        while (atkTargets.Count>0) {
            GameObject atkTarget = atkTargets[0];
            MonsterBase monsterBase = atkTarget.GetComponent<MonsterBase>();
            yield return ATKATK(monsterBase ,monsterBase.curHP);
           if(atkTargets.Count > 0&& atkTargets.Contains(atkTarget)) 
                atkTargets.Remove(atkTarget);
        }
        isAtk = false;
    }

    IEnumerator ATKATK(MonsterBase monsterBase, int Count)
    {
        PFunc.Log("怪物血量", Count);
        for (int i = 0; i < Count; i++)
        {
            yield return new WaitForSeconds(0.25f);
            Sound.PlaySound("Sound/SysBodyShotSfx");
            GameObject bullet = Instantiate(CatBullet, this.transform.position, Quaternion.identity);
            CatBullet catbullet = bullet.GetComponent<CatBullet>();
            catbullet.SetTarget(monsterBase.transform);
        }
    }

    void LateUpdate()
    {
        if (transform.position.x < 2764)
        {
            isFollow = false;
            return;
        }
        if (transform.position.x > 3260)
        {
            isFollow = false;
            return;
        }
        if (!isFollow) return;
        if (target == null)
        {
            target = PlayerController.Instance.transform;
            return;
        }

        Vector3 targetPosition = target.position + offset;
        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }
        if (spriteRenderer)
        {
            spriteRenderer.flipX = PlayerController.Instance.spriteTrans.transform.localScale.x==1;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Player"))
        {
            isFollow = true;
        }
        if (collision.CompareTag("Monster")|| collision.CompareTag("Boss"))
        {
            isFollow = true;
            atkTargets.Add(collision.gameObject);
            if (!isAtk)
            {
                isAtk = true; 
                StartCoroutine(OnATK());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Monster") || collision.CompareTag("Boss"))
        {
           if (atkTargets.Contains(collision.gameObject))
            {
                atkTargets.Remove(collision.gameObject);
            }
            if (atkTargets.Count <= 0) {
                isAtk = false;
                StopAllCoroutines(); 
            }
        }
    }
}
