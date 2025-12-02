using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tele : MonoBehaviour
{
    public TeleIn teleIn;
    public TeleOut teleOut;
    public Animator animator;

    public void OnTeleOut()
    {
        Sound.PlaySound("Sound/TeleOutSfx");
        animator.SetTrigger("Teleout");
        Invoke("OnOut",0.5f);
    }

    void OnOut()
    {
        PlayerController.Instance.gameObject.transform.position = teleOut.transform.position;
        PlayerController.Instance.spriteTrans.gameObject.SetActive(true);
        PlayerController.Instance.isHit = false;
    }
}
