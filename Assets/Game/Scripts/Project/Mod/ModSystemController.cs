using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModSystemController : MonoBehaviour
{
    public static ModSystemController Instance { get; private set; }
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

    public bool reverseJump = false;
    float reverseJumpTime = 0;

    public void OnSetRerverseJump(DataInfo dataInfo)
    {
        reverseJumpTime = dataInfo.count * dataInfo.time;
        if (!reverseJump)
        {
            reverseJump = true;
            StartCoroutine(OnCheckRerverseJumpTime());
        }
    }
    IEnumerator OnCheckRerverseJumpTime()
    {
        while (reverseJumpTime > 0)
        {
            reverseJumpTime -= Time.deltaTime;
            yield return null;
        }
        reverseJumpTime = 0;
        reverseJump = false;
    }

    public bool reverseCamera = false;
    float reverseCameraTime = 0;

    public void OnSetRerverseCamera(DataInfo dataInfo)
    {
        reverseCameraTime = dataInfo.count * dataInfo.time;
        if (!reverseCamera)
        {
            reverseCamera = true;
            StartCoroutine(OnCheckReverseCamera());
        }
    }
    IEnumerator OnCheckReverseCamera()
    {
        while (reverseCameraTime > 0)
        {
            reverseCameraTime -= Time.deltaTime;
            yield return null;
        }
        reverseCameraTime = 0;
        reverseCamera = false;
    }

    public bool Protecket = false;
    float ProtecketTime = 0;
    public GameObject shieldIPrefab;
    GameObject shieldIObj;
    public void OnSetPlayerProtecket(DataInfo dataInfo)
    {
        ProtecketTime = dataInfo.count * dataInfo.time;
      //  Sound.PlaySound("Sound/Mod/Freeze");
        if (!Protecket)
        {
            Protecket = true;
            shieldIObj = Instantiate(shieldIPrefab);
            StartCoroutine(OnCheckProtecket());
        }
    }
    IEnumerator OnCheckProtecket()
    {
        while (ProtecketTime > 0)
        {
            ProtecketTime -= Time.deltaTime;
            yield return null;
        }
        if (shieldIObj) Destroy(shieldIObj);
        ProtecketTime = 0;
        Protecket = false;
    }

    public GameObject ShakespeareLeft;
    public GameObject ShakespeareRight;
    public void OnShakespeare()
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
        int value = Random.Range(0,2);
        GameObject createObj = value == 0 ? ShakespeareLeft : ShakespeareRight;
        GameObject obj= SimplePool.Spawn(createObj, transform.position, Quaternion.identity);
        Sound.PlaySound("Sound/Mod/ssby");
        obj.transform.parent = transform;
    }

    public GameObject bigBeta;
    public void OnBigBetaForward()
    {
        Sound.PlaySound("Sound/Mod/dbt");
        GameObject obj = SimplePool.Spawn(bigBeta, transform.position, Quaternion.identity);
        obj.transform.parent = transform;
        PlayerModController.Instance.OnBigBetaForward(true);
    }
    public void OnBigBetaBack()
    {
        Sound.PlaySound("Sound/Mod/dbt");
        GameObject obj =SimplePool.Spawn(bigBeta,transform.position,Quaternion.identity);
        obj.transform.parent = transform;   
        PlayerModController.Instance.OnBigBetaForward(false);
    }
    public GameObject tansfarPre;
    public void OnRandromPlayerPos()
    {
        Instantiate(tansfarPre);
        Invoke("OnRandPlayer",0.4f);
    }
    void OnRandPlayer()
    {
        float leftValue=PlayerController.Instance.transform.position.x-200;
        float  rightValue = PlayerController.Instance.transform.position.x + 200;
        float targetX=Random.Range(leftValue,rightValue);
        PlayerController.Instance.transform.position=new Vector3(targetX,5,0);
    }

}
