using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreateController : MonoBehaviour
{
    public static MeshCreateController Instance { get; private set; }

    public GameObject TCJiao;
    public GameObject QLBi;
    public GameObject Trunck;

    public Transform trunckPos;
    public Transform qlPos;
    public Transform tcPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        
    }
    public void OnCreateTCJiao()
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
        GameObject obj = SimplePool.Spawn(TCJiao, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(tcPos, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        Sound.PlaySound("Sound/Mod/TCJiao");
        TCJiao tCJiao = obj.GetComponent<TCJiao>();
        tCJiao.OnStarMove();
    }

    public void OnCreateQLBi()
    {
        bool protect = ModSystemController.Instance.Protecket;
        if (protect) return;
        GameObject obj = SimplePool.Spawn(QLBi, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(qlPos, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        Sound.PlaySound("Sound/Mod/QLBi");
        QLBI qLBi = obj.GetComponent<QLBI>();
        qLBi.OnStarMove();
    }

    public void OnCreateTrunck()
    {
        EventManager.Instance.SendMessage(Events.OnTrunckMove);
        GameObject obj = SimplePool.Spawn(Trunck, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(trunckPos, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        Trunck qLBi = obj.GetComponent<Trunck>();
        qLBi.OnReadyStarMove();
        Sound.PlaySound("Sound/Mod/warning");
    }

    public Transform fingerPos;
    public GameObject yiyangzhi;
    public GameObject oneFinger;
    public void OnCreateOneFinger()
    {
        Sound.PlaySound("Sound/Mod/yiyangz");
        GameObject obj  =Instantiate(yiyangzhi);
        obj.transform.SetParent(Camera.main.transform, false);
        obj.transform.localPosition = new Vector3(0,0,1);
        obj.transform.localRotation = Quaternion.identity;
        Invoke("OnReadyCreateFinger",1);
    }

    void OnReadyCreateFinger()
    {
        GameObject obj = SimplePool.Spawn(oneFinger, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(fingerPos);
        obj.GetComponent<OneFingerZen>().OnReadyStarMove();
        obj.SetActive(true);
    }
}
