using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    // 消息队列容器
    private Queue<string> _messageQueue = new Queue<string>();

    public void OnDispseMsg(DataInfo dataInfo)
    {
        if (dataInfo == null)
        {
            PFunc.Log("消息空");
            return;
        }

        switch (dataInfo.call)
        {
            case "砸鸭子":
                ItemManager.Instance.OnCreateFlyItem(dataInfo);
                break;
            case "左边砸平底锅":
                ItemManager.Instance.OnCreatePDG(dataInfo);
                break;
            case "右边砸平底锅":
                ItemManager.Instance.OnCreatePDG(dataInfo);
                break;
            case "反向跳":
                ModSystemController.Instance.OnSetRerverseJump(dataInfo);
                break;
            case "视角反转":
                ModSystemController.Instance.OnSetRerverseCamera(dataInfo);
                break;
            case "冰冻":
                ItemManager.Instance.OnSetPlayerFreeze(dataInfo);
                break;
            case "无敌护盾":
                ModSystemController.Instance.OnSetPlayerProtecket(dataInfo);
                break;
            case "左正蹬":
                ItemManager.Instance.OnLeftLegKick(dataInfo);
                break;
            case "右鞭腿":
                ItemManager.Instance.OnRightLegKick(dataInfo);
                break;
            case "麒麟臂":
                MeshCreateController.Instance.OnCreateQLBi();
                break;
            case "天残脚":
                MeshCreateController.Instance.OnCreateTCJiao();
                break;
            case "撞大运":
                MeshCreateController.Instance.OnCreateTrunck();
                break;
            case "莎士比亚":
                ModSystemController.Instance.OnShakespeare();
                break;
            case "大贝塔":
                ModSystemController.Instance.OnBigBetaForward();
                break;
            case "反向大贝塔":
                ModSystemController.Instance.OnBigBetaBack();
                break;
            case "电击":
                ItemManager.Instance.OnLightningHit();
                break;
            case "彩虹猫":
                ItemManager.Instance.OnRainbowCat();
                break;
            case "番茄连招":
                PlayerModController.Instance.OnClickToCreateTomaTo(dataInfo);
                break;
            case "Boom":
                ItemManager.Instance.OnBoomGrandma();
                break;
            case "随机传送":
                ModSystemController.Instance.OnRandromPlayerPos();
                break;
            case "呸":
                ItemManager.Instance.OnCreateBlackHand(dataInfo);
                break;
            case "导弹":
                ItemManager.Instance.OnCreateRocket();
                break;
            case "隐身":
                PlayerModController.Instance.OnInvisibility(dataInfo);
                break;
            case "加速":
                PlayerModController.Instance.OnFastSpeed(dataInfo);
                break;
            case "减速":
                PlayerModController.Instance.OnMainSpeed(dataInfo);
                break;
            case "啄木鸟":
                ItemManager.Instance.OnCreateBird();
                break;
            case "砸落头像":
                ImageDownloader.Instance.OnRoleStar(dataInfo);
                break;
            case "打台球":
                ItemManager.Instance.OnCreateBilliard();
                break;
            case "大巴掌":
                ItemManager.Instance.OnCreateSlapFace();
                break;
            case "一阳指":
                MeshCreateController.Instance.OnCreateOneFinger();
                break;
            case "乌萨奇":
                ItemManager.Instance.OnCreateWuSaQi();
                break;
        }
    }
}

[Serializable]
public class DataInfo
{
    public string user;       // 用户名字段
    public string userAvatar; // 用户头像URL
    public string call;       // 功能
    public int count;         // 数量
    public int time;          // 功能触发时间
    public string enalbe;
}