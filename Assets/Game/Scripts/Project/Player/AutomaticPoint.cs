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
    public bool needCheckReady = true;
    public GameObject readyPosObj;
    public Vector3 targetPos;
    public List<AutomaticVideo> automaticMoveType;
    public List<AutomaticVideo> automaticMoveType2;
    public List<RedMonsterSetting> redMonsterSetting;
    public List<SawwerSetting> sawwerSetting;
    public List<FixTurretSetting> fixTurretSetting;
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

[Serializable]
public class FixTurretSetting
{
    public float waitTime;
    public FixTurret fixTurret;
}