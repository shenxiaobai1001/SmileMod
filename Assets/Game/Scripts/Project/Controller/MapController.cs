using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("πÿø®≈‰÷√")]
    public List<LevelInfo> levels = new List<LevelInfo>();
    private int lastGameLevel = -1;
    private float totalCycleLength;

    private void Start()
    {
        EventManager.Instance.AddListener(Events.MapChanged, OnMapChanged);
    }

    void OnMapChanged(object msg)
    {
        int level = GameController.Instance.gameLevel;
        LevelInfo levelInfo = levels[level-1];
        LevelInfo leftInfo = levels[levelInfo.LeftIndex - 1];
        LevelInfo rightInfo = levels[levelInfo.RightIndex - 1];
        float levelInfoX = levelInfo.levelTransform.position.x;
        float leftX = levelInfoX + levelInfo.LeftPos;
        float rightX = levelInfoX + levelInfo.RightPos;
        leftInfo.levelTransform.position = new Vector3(leftX,0);
        rightInfo.levelTransform.position = new Vector3(rightX, 0);
        for (int i = 0; i < levels.Count; i++) {

            levels[i].gameObject.SetActive(levels[i]== levelInfo|| levels[i] == leftInfo || levels[i] == rightInfo);
        }
    }
}
