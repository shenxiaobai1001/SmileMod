using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapFace : MonoBehaviour
{
    public Transform root;
    public GameObject boomPos;
    public GameObject boom;

    Vector3 startPo = new Vector3(-90,0,-115);
    Vector3 endPos = new Vector3(-90, 0, -216);
    public void OnBeginHit()
    {
        root.localEulerAngles = startPo;
        float relativeAngle = -101f; // -216 - (-115) = -101
        root.DOLocalRotate(new Vector3(0, 0, relativeAngle), 0.5f, RotateMode.LocalAxisAdd);
        Invoke("OnHitPlayer", 0.2f);
        Invoke("OnClose", 0.6f);
    } 

    void OnHitPlayer()
    {
        Sound.PlaySound("Sound/Mod/slapFace");
        PlayerModController.Instance.OnKickPlayer(new Vector3(-10, 15));
        GameObject obj = SimplePool.Spawn(boom, boomPos.transform.position, Quaternion.identity);
        obj.transform.SetParent(boomPos.transform);
        obj.SetActive(true);
    }

    void OnClose()
    {
        SimplePool.Despawn(this.gameObject);
        gameObject.SetActive(false);
    }
}
