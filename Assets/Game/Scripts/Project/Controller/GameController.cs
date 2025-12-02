using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public int gameLevel = 0;

    int saveIndex=1;
    public List<GameObject> savePos = new List<GameObject>();
    public bool isAutomatic = false;
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

    private void Start()
    {
        EventManager.Instance.AddListener(Events.SaveSchedule, OnSetSavePos);
        EventManager.Instance.AddListener(Events.PlayerRestToSavePos, PlayerRestToSavePos);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerRestToSavePos(null);
        }
    }

    public void OnSetGameLevel(int level)
    {
        gameLevel = level;
  
        switch (gameLevel)
        {
            case 1:
                Sound.PlayMusic("Bgm/bgm_0101world");
                break;
            case 2:
                Sound.PlayMusic("Bgm/bgm_ElectroBully");
                break;
            case 3:
                Sound.PlayMusic("Bgm/bgm_0101world");
                break;
            case 4:
                Sound.PlayMusic("Bgm/bgm_ElectroScream");
                break;
            case 5:
                Sound.PlayMusic("Bgm/bgm_FollowMe");
                break;
            case 6:
                Sound.PlayMusic("Bgm/bgm_Akatuski");
                break;
            case 7:
                Sound.PlayMusic("Bgm/bgm_hybrid");
                break;
        }
        PFunc.Log("修改关卡", gameLevel);
        PlayerController.Instance.OnSetStickCheck(gameLevel != 1&& gameLevel != 2);
        EventManager.Instance.SendMessage(Events.MapChanged);
    }

    void OnSetSavePos(object msg)
    {
        if (msg == null) return;
        saveIndex = (int)msg;
    }
    void PlayerRestToSavePos(object msg)
    {
        PFunc.Log("重置玩家位置");
        Vector3 vector2 = savePos[saveIndex-1].gameObject.transform.position;
        PlayerController.Instance.transform.position = vector2;
        PlayerController.Instance.OnRest();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.SaveSchedule, OnSetSavePos);
        EventManager.Instance.RemoveListener(Events.PlayerRestToSavePos, PlayerRestToSavePos);
    }
}
