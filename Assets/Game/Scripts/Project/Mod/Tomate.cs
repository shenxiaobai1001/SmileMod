using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tomate : MonoBehaviour
{
    public List<GameObject> sprites;
    public float destroyDistance = 10f; // 移动多远后销毁
    bool move = false;
    Vector3 startPosition;
    Vector2 moveVec;

    public void OnStartMove( )
    {    // 记录开始位置
        startPosition = transform.position;
        int index=Random.Range(0,sprites.Count);
        for (int i = 0; i < sprites.Count; i++)
        {
            int value = i;
            sprites[i].SetActive(value==index);
        }

        move = true;
    }

    private void Update()
    {
        if (!move) return;
         transform.Translate(Vector2.right * 10 * Time.deltaTime);
        // 检查移动距离是否达到销毁条件
        if (Vector3.Distance(startPosition, transform.position) >= destroyDistance)
        {
            move = false;
            SimplePool.Despawn(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("RainbowCat"))
        {
            Vector2 collisionPoint = collision.ClosestPoint(transform.position);

            ItemManager.Instance.OnCreateTomatoBoom(new Vector3(collisionPoint.x + 1, collisionPoint.y));
            move = false;
            SimplePool.Despawn(gameObject);
            gameObject.SetActive(false);
        }
    }
}
