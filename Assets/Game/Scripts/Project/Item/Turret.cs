using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform turret;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D circleCollider;

    public Vector3 movingVec;
    public float currentSpeed;
    public float moveEnd; // 移动结束距离

    float rotationSpeed = -360;    // 旋转速度(度/秒)
    bool isRun = false;
    private Vector3 startPosition;   // 记录起始位置

    private void Start()
    {
        // 记录起始位置
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isRun) return;

        // 旋转齿轮
        RotateSaw();
        Movement();

        // 检查是否超过移动距离
        CheckMoveDistance();
    }

    // 巡逻移动
    private void Movement()
    {
        transform.position += movingVec * currentSpeed * Time.deltaTime;
    }

    // 旋转齿轮
    private void RotateSaw()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
       if(turret) turret.transform.Rotate(0f, 0f, rotationAmount);
    }

    // 检查移动距离
    private void CheckMoveDistance()
    {
        float distanceMoved = Vector3.Distance(startPosition, transform.position);

        if (distanceMoved >= moveEnd)
        {
            isRun = false;
            circleCollider.enabled = false;
            OnMoveEndReached();
        }
    }

    // 到达移动终点
    private void OnMoveEndReached()
    {
        spriteRenderer.DOFade(0, 1).OnComplete(() => {
            spriteRenderer.color = Color.white;
            SimplePool.Despawn(gameObject);
            gameObject.SetActive(false);
        });
    }

    public void OnSetData(Vector3 movingVec, float currentSpeed, float moveEndDistance = 10f)
    {
  
        this.movingVec = movingVec.normalized; // 确保方向向量标准化
        this.currentSpeed = currentSpeed;
        this.moveEnd = moveEndDistance;
        this.startPosition = transform.position; // 重置起始位置
        if(circleCollider) circleCollider.enabled = true;
        isRun = true;
    }

}