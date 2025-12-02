using UnityEngine;
using System.Collections.Generic;

public class CloudLayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;          // 移动速度
    public float resetDistance = 25f;    // 重置距离
    public float spacing = 15f;          // 云雾间距

    [Header("调试")]
    public bool drawDebug = true;
    public Color debugColor = Color.cyan;

    private List<Transform> clouds = new List<Transform>();

    private void Start()
    {
        // 初始化获取所有子物体云雾
        foreach (Transform child in transform)
        {
            clouds.Add(child);
        }
    }

    private void Update()
    {
        MoveClouds();
    }

    private void MoveClouds()
    {
        for (int i = 0; i < clouds.Count; i++)
        {
            Transform cloud = clouds[i];

            // 移动云雾
            cloud.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);

            // 检查是否需要重置位置
            if (cloud.position.x < resetDistance)
            {
                // 找到最右侧的云雾
                Transform rightMost = GetRightMostCloud();

                // 重置到最右侧
                Vector3 newPos = rightMost.position;
                newPos.x += spacing;
                cloud.position = newPos;
            }
        }
    }

    // 获取最右侧的云雾
    private Transform GetRightMostCloud()
    {
        Transform rightMost = clouds[0];
        foreach (Transform cloud in clouds)
        {
            if (cloud.position.x > rightMost.position.x)
            {
                rightMost = cloud;
            }
        }
        return rightMost;
    }

}