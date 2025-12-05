using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class PlayerAutomaticSystem : MonoBehaviour
{
    public static PlayerAutomaticSystem Instance { get; private set; }
    public PlayerController playerController;
    private Queue<AutomaticPoint> automaticPoints = new Queue<AutomaticPoint>();
    private Dictionary<string, AutomaticPoint> dicAutomaticPoints = new Dictionary<string, AutomaticPoint>();
    private Coroutine automaticCoroutine;
    public PlayerCheckGround playerCheckGround;
    public PlayerCheckGround playerCheckRight;
    public PlayerCheckGround playerCheckLeft;
    private PlayerControState currentMoveType = PlayerControState.LRun;

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
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameController.Instance.isAutomatic = !GameController.Instance.isAutomatic;
            if (GameController.Instance.isAutomatic)
                OnBeginAutomatic();
            else {
                OnStopAutomatic();
            }
        }
    }

    public void OnBeginAutomatic()
    {
        OnClearAllAutomatic();

        if (automaticCoroutine != null)
            StopCoroutine(automaticCoroutine);

        automaticCoroutine = StartCoroutine(AutomaticMove());
    }

    public void OnStopAutomatic()
    {
        GameController.Instance.isAutomatic = false;

        if (automaticCoroutine != null)
        {
            StopCoroutine(automaticCoroutine);
            automaticCoroutine = null;
        }
    }

    bool MaticSucc = false;
    bool hasAutoMaticPoint = false;
    string needEndIndex = "";

    IEnumerator AutomaticMove()
    {
        yield return new WaitForSeconds(1f);
        while (GameController.Instance.isAutomatic)
        {
            PFunc.Log("OnPlayerControStateChange", automaticPoints.Count, hasAutoMaticPoint);

            // 处理队列中的指令点
            if (automaticPoints.Count > 0 && hasAutoMaticPoint)
            {
                AutomaticPoint point = automaticPoints.Dequeue();

                // 检查玩家位置是否超过了readyPosObj>去到适合执行命令的点
                if (point.readyPosObj != null && IsPlayerBeyondReadyPosition(point)&& point.needCheckReady)
                {
                    PFunc.Log("玩家位置超过准备点，执行折返逻辑");
                    // 先控制玩家回到准备位置
                    yield return MoveToReadyPosition(point);
                    currentMoveType = PlayerControState.CanelLRun;
                    yield return ExecuteMoveInstruction(currentMoveType);
                }
                //重置怪物和齿轮位置方便通过
                if (point.redMonsterSetting.Count > 0)
                {
                    for (int i = 0; i < point.redMonsterSetting.Count; i++) {
                        var redSet = point.redMonsterSetting[i];
                        redSet.redMonster.SetLocalPositionAndDirection(redSet.restPos, redSet.moveRight);
                    }
                }
                if (point.sawwerSetting.Count > 0)
                {
                    for (int i = 0; i < point.sawwerSetting.Count; i++)
                    {
                        var sawwerr = point.sawwerSetting[i];
                        sawwerr.sawwer.SetSawLocalPosition(sawwerr.restPos, sawwerr.moveToPoint2);
                    }
                }

                if (point.fixTurretSetting.Count > 0)
                {
                    for (int i = 0; i < point.fixTurretSetting.Count; i++)
                    {
                        var ffixTurret = point.fixTurretSetting[i];
                        ffixTurret.fixTurret.OnSetWaitTime(ffixTurret.waitTime);
                    }
                }

                bool order = true;
                //开始执行命令节点
                if (point.automaticMoveType.Count>0&& order)
                {
                    for (int i = 0; i < point.automaticMoveType.Count; i++)
                    {
                        var MoveType = point.automaticMoveType[i].automaticMoveType;
                        PFunc.Log("automaticMoveType", point.index, MoveType, currentMoveType);
                        if (MoveType == PlayerControState.Jump || MoveType != currentMoveType)
                        {
                            currentMoveType = MoveType;
                            yield return ExecuteMoveInstruction(currentMoveType);

                            if (point.automaticMoveType[i].waitTime != 0)
                                yield return new WaitForSeconds(point.automaticMoveType[i].waitTime);
                        }
                        if (point.targetPos != Vector3.zero)
                        {
                            float y = point.targetPos.y;
                            PFunc.Log(transform.position.y, y);
                            if (transform.position.y >= y)
                            {
                                order = false;
                                break;
                            }
                        }
                    }
                }
                if (point.automaticMoveType2.Count > 0 && !order)
                {
                    float y = point.targetPos.y;
                    yield return new WaitUntil(() => transform.position.y <= y);
                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < point.automaticMoveType2.Count; i++)
                    {
                        var MoveType = point.automaticMoveType2[i].automaticMoveType;
                        if (MoveType != currentMoveType)
                        {
                            currentMoveType = MoveType;
                            yield return ExecuteMoveInstruction(currentMoveType);
                            if (point.automaticMoveType2[i].waitTime != 0)
                                yield return new WaitForSeconds(point.automaticMoveType2[i].waitTime);
                        }
                    }
                }
                PFunc.Log("Point结束", point.index, currentMoveType);
                MaticSucc = false;
                needEndIndex = point.index;
                dicAutomaticPoints.Remove(point.index);
                if (point.needEnd)
                    yield return new WaitUntil(() => MaticSucc == true);
            }
            else
            {
                // 没有特殊指令时默认跑动
                currentMoveType = PlayerControState.RRun;
                yield return ExecuteMoveInstruction(currentMoveType);
                hasAutoMaticPoint = false;
                yield return new WaitForSeconds(0.05f);
                yield return new WaitUntil(() => hasAutoMaticPoint == true);
            }
            yield return null;
        }
    }

    // 检查玩家是否超过了readyPosObj的位置（X轴）
    private bool IsPlayerBeyondReadyPosition(AutomaticPoint point)
    {
        if (point.readyPosObj == null) return false;

        float playerX = playerController.transform.position.x;
        float readyPosX = point.readyPosObj.transform.position.x;

        // 根据玩家的当前方向判断是否超过
        bool isBeyond = false;

        // 假设玩家默认向右跑动，如果玩家X > 准备点X，则认为超过了
        if (playerX > readyPosX)
        {
            isBeyond = true;
        }

        //PFunc.Log($"位置检查: 玩家X={playerX}, 准备点X={readyPosX}, 是否超过={isBeyond}");
        return isBeyond;
    }

    IEnumerator MoveToReadyPosition(AutomaticPoint point)
    {
        if (point.readyPosObj == null) yield break;

        float targetX = point.readyPosObj.transform.position.x;

        //PFunc.Log("开始移动回准备位置", "玩家X=" + playerController.transform.position.x, "目标X=" + targetX);

        // 持续向左移动，直到玩家到达目标位置左边
        float distanceThreshold = 0.1f; // 到达的阈值

        while (GameController.Instance.isAutomatic &&
               playerController.transform.position.x > targetX + distanceThreshold)
        {
            // 持续向左移动
            playerController.OnPlayerControStateChange(PlayerControState.LRun);

            //PFunc.Log("移动中", "玩家X=" + playerController.transform.position.x, "目标X=" + targetX);
            yield return null;
        }

        // 确保玩家稍微超过一点目标位置（到达左边）
        if (playerController.transform.position.x > targetX)
        {
            // 继续向左移动一小段距离，确保完全超过
            float overshootDistance = 0.05f;
            float overshootTargetX = targetX - overshootDistance;

            while (GameController.Instance.isAutomatic &&
                   playerController.transform.position.x > overshootTargetX)
            {
                playerController.OnPlayerControStateChange(PlayerControState.LRun);
                yield return null;
            }
        }

        // 停止移动
        playerController.OnPlayerControStateChange(PlayerControState.None);

        //PFunc.Log("已到达准备位置左边", "最终位置=" + playerController.transform.position.x, "目标位置=" + targetX);
    }

    IEnumerator ExecuteMoveInstruction(PlayerControState point)
    {
        switch (point)
        {
            case PlayerControState.None:
                playerController.OnPlayerControStateChange(PlayerControState.None);
                break;
            case PlayerControState.LRun:
                playerController.OnPlayerControStateChange(PlayerControState.LRun);
                break;
            case PlayerControState.LRuning:
                playerController.OnPlayerControStateChange(PlayerControState.LRuning);
                break;
            case PlayerControState.CanelLRun:
                playerController.OnPlayerControStateChange(PlayerControState.CanelLRun);
                break;
            case PlayerControState.RRun:
                playerController.OnPlayerControStateChange(PlayerControState.RRun);
                break;
            case PlayerControState.RRuning:
                playerController.OnPlayerControStateChange(PlayerControState.RRuning);
                break;
            case PlayerControState.CanelRRun:
                playerController.OnPlayerControStateChange(PlayerControState.CanelRRun);
                break;
            case PlayerControState.ToFast:
                playerController.OnPlayerControStateChange(PlayerControState.ToFast);
                break;
            case PlayerControState.Fast:
                playerController.OnPlayerControStateChange(PlayerControState.Fast);
                break;
            case PlayerControState.CancelFast:
                playerController.OnPlayerControStateChange(PlayerControState.CancelFast);
                break;
            case PlayerControState.Jump:

                yield return new WaitUntil(() => playerCheckGround.isGround
                                                        || playerCheckRight.isGround 
                                                        || playerCheckLeft.isGround);
                yield return new WaitUntil(() => playerController.pLState == PLState.Idel 
                                                        || playerController.pLState == PLState.RRun
                                                          || playerController.pLState == PLState.LRun
                                                         || playerController.pLState == PLState.FastRRun
                                                          || playerController.pLState == PLState.Stick
                                                          || playerController.pLState == PLState.LStick 
                                                          || playerController.pLState == PLState.RStick);
                playerController.OnPlayerControStateChange(PlayerControState.Jump);
                break;
            case PlayerControState.Jumping:
                playerController.OnPlayerControStateChange(PlayerControState.Jumping);
                break;
            case PlayerControState.CJump:
                playerController.OnPlayerControStateChange(PlayerControState.CJump);
                break;
            case PlayerControState.LStickJump:
                playerController.OnPlayerControStateChange(PlayerControState.LStickJump);
                break;
            case PlayerControState.RStickJump:
                playerController.OnPlayerControStateChange(PlayerControState.RStickJump);
                break;
            case PlayerControState.Boost:
                playerController.OnPlayerControStateChange(PlayerControState.Boost);
                break;
            case PlayerControState.Boosting:
                playerController.OnPlayerControStateChange(PlayerControState.Boosting);
                break;
            case PlayerControState.CanelBoost:
                playerController.OnPlayerControStateChange(PlayerControState.CanelBoost);
                break;
            default:
                break;
        }
        yield return new WaitForEndOfFrame();
    }

    public void OnClearAllAutomatic()
    {
        if (automaticPoints != null && automaticPoints.Count > 0)
        {
            automaticPoints.Clear();
        }
        if (dicAutomaticPoints != null && dicAutomaticPoints.Count > 0)
        {
            dicAutomaticPoints.Clear();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AutomaticPoint point = collision.GetComponent<AutomaticPoint>();
        if (point != null)
        {
            if (point.start && !dicAutomaticPoints.ContainsKey(point.index))
            {
                automaticPoints.Enqueue(point);
                dicAutomaticPoints.Add(point.index, point);
                hasAutoMaticPoint = true;
            }
            else
            {
                if (needEndIndex == point.index && !point.start)
                {
                    MaticSucc = true;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AutomaticPoint point = collision.GetComponent<AutomaticPoint>();
        if (point != null && !hasAutoMaticPoint)
        {
            if (point.start)
            {
                if (point.start && !dicAutomaticPoints.ContainsKey(point.index))
                {
                    automaticPoints.Enqueue(point);
                    dicAutomaticPoints.Add(point.index, point);
                    hasAutoMaticPoint = true;
                }
            }
        }
    }
}