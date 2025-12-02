using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleStar : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Rigidbody rigidbody;
    public float destroyHeight = 20f; // 物体删除的高度阈值
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;

    bool kickPlayer = true;
    Transform playerTarget;
    Vector3 initialPosition;
    bool isMove = false;
    void Start()
    {
        playerTarget = PlayerController.Instance.transform;

    }

    void Update()
    {
        if (!isMove) return;
        if (kickPlayer)
        {
            ChasePlayer();
        }
        else
        {
            // 检查物体高度
            if (transform.position.y < destroyHeight)
            {
                isMove = false;
                SimplePool.Despawn(gameObject);
            }
        }
    }

    // 外部方法：设置初始位置
    public void StartMove(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        InitializeRandomRotation();
        isMove = true;
        kickPlayer = true;
        particleSystem.Play();
    }

    void ChasePlayer()
    {
        // 关闭物理引擎
        if (!rigidbody.isKinematic)
        {
            rigidbody.isKinematic = true;
        }
        transform.LookAt(playerTarget);
        // 向玩家移动
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, playerTarget.position) < 0.1f)
        {
            kickPlayer = false;
            bool protect = ModSystemController.Instance.Protecket;
            if (!protect)
                PlayerModController.Instance.OnLeftHitPlayer();
            OnReachPlayer();
        }
    }
    // 初始化随机旋转参数
    void InitializeRandomRotation()
    {
        transform.LookAt(playerTarget);
    }
    void OnReachPlayer()
    {
        // 开启物理引擎
        rigidbody.isKinematic = false;

        // 随机给一个向左上或右上的力
        Vector3 randomForce = Vector3.zero;
        if (Random.value > 0.5f)
        {
            // 左上方向
            randomForce = new Vector3(-0.5f, 0.5f, 0f);
        }
        else
        {
            // 右上方向
            randomForce = new Vector3(0.5f, 0.5f, 0f);
        }
        rigidbody.AddForce(randomForce, ForceMode.Impulse);
    }
}
