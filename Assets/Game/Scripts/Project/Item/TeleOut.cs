using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleOut : MonoBehaviour
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
            animator.SetTrigger("Teleout");
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("Teleout");
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("TeleOutEnd");
        }
    }
}
