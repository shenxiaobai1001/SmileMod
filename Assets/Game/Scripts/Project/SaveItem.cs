using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveItem : MonoBehaviour
{
    public Animator animator;

    public int saveIndex;

    public GameObject effect;
    public Transform effectPos;

    bool check = true;

    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player")&& check)
        {
            Sound.PlaySound("Sound/SavePointSfx");
            check = false;
            Instantiate(effect, effectPos.position,Quaternion.identity);
            animator.SetTrigger("Save");
            EventManager.Instance.SendMessage(Events.SaveSchedule, saveIndex);
        }
    }
}
