using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutomaticPoint : MonoBehaviour
{
    public string index;
    public bool needEnd = true;
    public bool start;
    public GameObject readyPosObj;
    public List<AutomaticVideo> automaticMoveType;
    public List<RedMonsterSetting> redMonsterSetting;
    public List<SawwerSetting> sawwerSetting;
}

[Serializable]
public class AutomaticVideo
{
    public float waitTime;
    public PlayerControState automaticMoveType = PlayerControState.None;
}

[Serializable]
public class RedMonsterSetting
{
    public bool moveRight;
    public Vector3 restPos;
    public RedMonsterController redMonster; 
}

[Serializable]
public class SawwerSetting
{
    public bool moveToPoint2;
    public Vector3 restPos;
    public Sawwer sawwer;
}
