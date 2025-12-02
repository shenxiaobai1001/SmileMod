using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billiards : MonoBehaviour
{
    public GameObject Model;
    public GameObject ganZi;
    Sequence sequence;
    Vector3 ganZiPos = new Vector3(-10.36f, 1.06f);
    Vector3 enPos=new Vector3(-5.67f,1.73f);
    Vector3 readyPos = new Vector3(-7.81f, 1.4f);

    public GameObject boom;
    public GameObject boomPos;
    public void StartShow()
    {
        Model.SetActive(true);
        // 创建序列动画
        sequence = DOTween.Sequence();
        ganZi.transform.localPosition = readyPos;
       ganZiPos = ganZi.transform.localPosition;
        // 添加动画到序列中
        sequence.Append(ganZi.transform.DOLocalMove(enPos, 0.75f));  // 第一步：移动到enPos
        sequence.Append(ganZi.transform.DOLocalMove(ganZiPos, 0.15f)); // 第二步：返回原位
        sequence.onComplete += () => {
            PlayerModController.Instance.OnKickPlayer(new Vector3(-15, 5));
            GameObject obj = SimplePool.Spawn(boom, boomPos.transform.position,Quaternion.identity);
            obj.transform.SetParent(boomPos.transform);
            obj.SetActive(true);
            Model.SetActive(false);
            Invoke("OnClose", 1f);
        };
    }
    void OnClose()
    {
        SimplePool.Despawn(this.gameObject);
        gameObject.SetActive(false);
    }


}
