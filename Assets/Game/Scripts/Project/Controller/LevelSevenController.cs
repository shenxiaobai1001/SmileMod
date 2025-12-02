using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSevenController : MonoBehaviour
{
    public GameObject Door;
    public List<GameObject> Monsters;


    bool openDoor = false;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(Events.SevenBossDie, OnOpenDoor);
        EventManager.Instance.AddListener(Events.GameRest, GameRest);
    }

   void OnOpenDoor(object msg)
    {
        if(Door) Door.SetActive(false);
     
        if (!openDoor)
        {
            openDoor = true;
            Invoke("OnColseDoor", 20f);
        }
    }

    void GameRest(object msg)
    {
        for (int i = 0; i < Monsters.Count; i++) {
            Monsters[i].gameObject.SetActive(true);
            MonsterBase monsterBase = Monsters[i].GetComponent<MonsterBase>();
            if (monsterBase != null) {
                monsterBase.OnRest();
            }
        }
    }

    void OnColseDoor()
    {
        openDoor = false;
        if (Door) Door.SetActive(true);
    }
}
