using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMissile : MonoBehaviour
{
    [Header("组件引用")]
    public GameObject button1;          // 按钮待机状态
    public GameObject button2;          // 按钮按下状态
    public GameObject rocket;           // 火箭对象
    public Animator shotEffect;         // 发射特效
    public Missile missile;             // 火箭控制脚本
    public GameObject missilePos1;      // 有火箭的起落架
    public GameObject missilePos2;      // 空的起落架

    [Header("火箭设置")]
    public string bossTag = "Boss";     // Boss标签
    public float resetCooldown = 10f;   // 重置冷却时间

    private bool isTriggered = false;    // 是否已触发
    private Vector3 rocketStartPosition; // 火箭起始位置
    private Quaternion rocketStartRotation; // 火箭起始旋转

    void Start()
    {
        EventManager.Instance.AddListener(Events.GameRest, OnRest);
        // 保存火箭初始状态
        if (rocket != null)
        {
            rocketStartPosition = rocket.transform.position;
            rocketStartRotation = rocket.transform.rotation;
        }

        // 初始化状态
        ResetSystem();
    }

    void OnRest(object msg)
    {
        ResetSystem();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            Sound.PlaySound("Sound/ShotMissileSfx");
            TriggerRocketLaunch();
        }
    }

    /// <summary>
    /// 触发火箭发射
    /// </summary>
    private void TriggerRocketLaunch()
    {
        isTriggered = true;

        // 1. 改变按钮状态
        UpdateButtonState(true);

        // 2. 播放发射特效
        if (shotEffect != null)
        {
            shotEffect.SetTrigger("Shot");
        }

        // 3. 改变起落架状态
        UpdateLaunchPadState(true);

        // 4. 启动火箭
        if (missile != null)
        {
            // 找到Boss目标
            GameObject boss = FiveHeadBoss.Instance.gameObject;
            if (boss != null)
            {
               if(rocket) rocket.SetActive(true); // 隐藏火箭
                missile.StartChasingBoss(boss.transform);
            }
            else
            {
                Debug.LogWarning("未找到Boss对象，火箭无法发射");
            }
        }
    }

    /// <summary>
    /// 更新按钮状态
    /// </summary>
    private void UpdateButtonState(bool isPressed)
    {
        if (button1 != null) button1.SetActive(!isPressed);
        if (button2 != null) button2.SetActive(isPressed);
    }

    /// <summary>
    /// 更新起落架状态
    /// </summary>
    private void UpdateLaunchPadState(bool isLaunched)
    {
        if (missilePos1 != null) missilePos1.SetActive(!isLaunched);
        if (missilePos2 != null) missilePos2.SetActive(isLaunched);
    }

    /// <summary>
    /// 重置整个系统到初始状态
    /// </summary>
    public void ResetSystem()
    {
        isTriggered = false;

        // 重置按钮状态
        UpdateButtonState(false);

        // 重置起落架状态
        UpdateLaunchPadState(false);

        // 重置火箭
        if (missile != null)
        {
            missile.ResetRocket();
        }

        // 重置火箭位置和显示状态
        if (rocket != null)
        {
            rocket.transform.position = rocketStartPosition;
            rocket.transform.rotation = rocketStartRotation;
            rocket.SetActive(false); // 隐藏火箭
        }

        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.GameRest, OnRest);
    }
}