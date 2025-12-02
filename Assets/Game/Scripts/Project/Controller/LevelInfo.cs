using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LevelInfo : MonoBehaviour
{
    public string levelName;
    public float levelLength;
    public Transform levelTransform; // 直接引用场景中已有的关卡
    [HideInInspector] public int levelIndex; // 关卡的固定索引(0-6)
    public int LeftIndex;
    public float LeftPos;
    public int RightIndex;
    public float RightPos;  

}
