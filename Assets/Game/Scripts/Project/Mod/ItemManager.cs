using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public Transform createPos1;
    public Transform createPosLeft;
    public Transform createPosRight;

    public GameObject duckObj;
    public GameObject pdgObj;
    public GameObject Ice;
    public GameObject Shield;
    public GameObject RightLeg;
    public GameObject LeftLeg;
    public GameObject Electricity;
    public GameObject RainBowCat;
    public GameObject BoomGrandema;
    public GameObject blackHand;

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

    public void OnCreateFlyItem(DataInfo dataInfo)
    {
        int allDuck = dataInfo.count * dataInfo.time;
        for (int i = 0; i < allDuck; i++) {
            Sound.PlaySound("Sound/Mod/Duck");
            int y = Random.Range(-5, 5);
            Vector3 duckCPos = new Vector3(createPos1.position.x, y);
            GameObject duck = SimplePool.Spawn(duckObj, duckCPos, Quaternion.identity);
            duck.transform.SetParent(this.transform);
            Duck duck1 = duck.GetComponent<Duck>();
            duck1.StartMove();
        }
        allDuck = 0;
    }
    public void OnCreatePDG(DataInfo dataInfo)
    {
        int allDuck = dataInfo.count * dataInfo.time;

        Transform createPos=null;
        switch (dataInfo.call)
        {
            case "×ó±ßÔÒÆ½µ×¹ø":
                createPos = createPosLeft;
                break;
            case "ÓÒ±ßÔÒÆ½µ×¹ø":
                createPos = createPosRight;
                break;
        }

        for (int i = 0; i < allDuck; i++)
        {
            Sound.PlaySound("Sound/Mod/PDG");
            GameObject obj = SimplePool.Spawn(pdgObj, createPos.position, Quaternion.identity);
            obj.transform.SetParent(this.transform);
            Pan pan = obj.GetComponent<Pan>();
            pan.StartMove(createPos== createPosLeft);
        }
        allDuck = 0;
    }

    public bool Freeze = false;
    float FreezeTime = 0;
    GameObject iceObject;
    public void OnSetPlayerFreeze(DataInfo dataInfo)
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
         FreezeTime = dataInfo.count * dataInfo.time;
        Sound.PlaySound("Sound/Mod/Freeze");
        if (!Freeze)
        {
            PlayerController.Instance.isHit = true;
            Freeze = true;
            iceObject=Instantiate(Ice);
            StartCoroutine(OnChecklayerFreeze());
        }
    }
    IEnumerator OnChecklayerFreeze()
    {
        while (FreezeTime > 0)
        {
            FreezeTime -= Time.deltaTime;
            yield return null;
        }
       if(iceObject) Destroy(iceObject);
        FreezeTime = 0;
        Freeze = false;
        PlayerController.Instance.isHit = false;
    }

    public void OnRightLegKick(DataInfo dataInfo)
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
        int allCount = dataInfo.count * dataInfo.time;

        for (int i = 0; i < allCount; i++) {
        
            GameObject obj=  Instantiate(RightLeg);
            obj.transform.SetParent(transform);
            Sound.PlaySound("Sound/Mod/rightLeg");
        }
    }
    public void OnLeftLegKick(DataInfo dataInfo)
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
        int allCount = dataInfo.count * dataInfo.time;

        for (int i = 0; i < allCount; i++)
        {
            GameObject obj = Instantiate(LeftLeg);
            obj.transform.SetParent(transform);
            Sound.PlaySound("Sound/Mod/leftLeg");
        }
    }

    public void OnLightningHit()
    {
        GameObject obj = SimplePool.Spawn(Electricity, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        PlayerModController.Instance.OnKickPlayer(new Vector3(Random.Range(-3,3),10));
    }

    public bool rainBow = false;
    float rainBowCount = 300;

    public void OnRainbowCat()
    {
        if (!rainBow)
        {
            Sound.PlaySound("Sound/Mod/chm");
            rainBow = true;
            EventManager.Instance.SendMessage(Events.RainBowCat);
            StartCoroutine(OnRainbowCatRun());
        }
    }
    IEnumerator OnRainbowCatRun()
    {
        while (rainBowCount > 0) {
            yield return new WaitForSeconds(0.1f);
            int y = Random.Range(-8, 8);
            Vector3 dCPos = new Vector3(createPos1.position.x, y);
            GameObject obj = SimplePool.Spawn(RainBowCat, dCPos, Quaternion.identity);
            obj.transform.SetParent(transform);
            rainBowCount--;
        }
        rainBowCount = 300;
        rainBow = false;
    }

    public void OnBoomGrandma()
    {
        Sound.PlaySound("Sound/Mod/BoomGrandma");
        GameObject obj = SimplePool.Spawn(BoomGrandema, transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
    }

    public GameObject tomatoBoom;
    public void OnCreateTomatoBoom(Vector3 pos)
    {
        GameObject obj = SimplePool.Spawn(tomatoBoom, pos, Quaternion.identity);
        obj.transform.SetParent(transform);
    }

    public void OnCreateBlackHand(DataInfo dataInfo)
    {
        int allCount = dataInfo.time;
        GameObject obj = Instantiate(blackHand);
        obj.transform.SetParent(Camera.main.transform);
        obj.transform.position = Vector3.zero;
        BlackHand fastRunEffect = obj.GetComponent<BlackHand>();
        fastRunEffect.OnSetTime (allCount);
        Sound.PlaySound("Sound/Mod/hs");
    }

    public Transform flowPos3;
    public GameObject normalRocket;
    public GameObject spacilRocket;
    public void OnCreateRocket()
    {
        int value = Random.Range(0, 10);
        GameObject rocket = value == 5 ? spacilRocket : normalRocket;
        int x = Random.Range(-10, 10);
        Vector3 dCPos = new Vector3(x, flowPos3.position.y);
        GameObject obj = SimplePool.Spawn(rocket, dCPos, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }

    public GameObject bird;
    public void OnCreateBird()
    {
        Sound.PlaySound("Sound/Mod/brid");
        GameObject obj = SimplePool.Spawn(bird, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }

    public GameObject billiard;
    public void OnCreateBilliard()
    {
        GameObject obj = SimplePool.Spawn(billiard, PlayerController.Instance.transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.GetComponent<Billiards>().StartShow();
        obj.SetActive(true);
    }

    public GameObject SlapFace;
    public void OnCreateSlapFace()
    {
        GameObject obj = SimplePool.Spawn(SlapFace, PlayerController.Instance.transform.position, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.GetComponent<SlapFace>().OnBeginHit();
        obj.SetActive(true);
    }
    public Transform createPos4;
    public GameObject wusaqi;
    public void OnCreateWuSaQi()
    {
        Sound.PlaySound("Sound/Mod/wusaqi");
        Vector3 dCPos = new Vector3(createPos4.position.x, createPos4.position.y);
        GameObject obj = SimplePool.Spawn(wusaqi, dCPos, Quaternion.identity);
        obj.transform.SetParent(createPos4.transform);
        obj.GetComponent<WuSaQi>().StartMove();
        obj.SetActive(true);
    }
}
