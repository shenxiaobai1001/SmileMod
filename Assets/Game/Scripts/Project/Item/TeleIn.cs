using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleIn : MonoBehaviour
{
    public Animator animator;
    public Tele tele;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.CompareTag("Player"))
        {
            Sound.PlaySound("Sound/TeleInSfx");
            animator.SetTrigger("Telein");
            PlayerController.Instance.isHit = true;
            PlayerController.Instance.spriteTrans.gameObject.SetActive(false);
            PlayerController.Instance.OnRest();
            tele.OnTeleOut();
        }
    }
}
