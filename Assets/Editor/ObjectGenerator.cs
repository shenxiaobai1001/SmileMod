using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ObjectGenerator : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject prefabToGenerate;  // 要生成的预制体
    public int objectCount = 5;          // 生成数量
    public Vector3 generationDirection = Vector3.right; // 生成方向
    public float spacing = 1.0f;         // 物体间距
    public bool alignRotation = true;    // 是否对齐生成方向

    [Header("调试")]
    public bool drawGizmos = true;       // 是否绘制辅助线
    public Color gizmoColor = Color.green;

    private GameObject[] generatedObjects; // 存储生成的物体

    // 在Inspector中添加按钮
    [ContextMenu("生成物体")]
    public void GenerateObjects()
    {
        // 先清除已生成的物体
        ClearGeneratedObjects();

        if (prefabToGenerate == null)
        {
            Debug.LogWarning("未指定要生成的预制体!");
            return;
        }

        generatedObjects = new GameObject[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            // 计算位置
            Vector3 position = transform.position + generationDirection.normalized * (spacing * i);

            // 生成物体
            GameObject obj;
            if (Application.isPlaying)
            {
                obj = Instantiate(prefabToGenerate, position, Quaternion.identity);
            }
            else
            {
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToGenerate);
                obj.transform.position = position;
            }

            // 设置旋转
            if (alignRotation && generationDirection != Vector3.zero)
            {
                obj.transform.rotation = Quaternion.LookRotation(generationDirection);
            }

            obj.transform.parent = this.transform;
            obj.name = $"{prefabToGenerate.name}_{i}";
            generatedObjects[i] = obj;
        }

        Debug.Log($"已生成 {objectCount} 个物体");
    }

    [ContextMenu("清除生成的物体")]
    public void ClearGeneratedObjects()
    {
        if (generatedObjects != null)
        {
            foreach (GameObject obj in generatedObjects)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(obj);
                    }
                    else
                    {
                        DestroyImmediate(obj);
                    }
                }
            }
        }

        // 清除所有子物体（防止漏网之鱼）
        while (transform.childCount > 0)
        {
            GameObject child = transform.GetChild(0).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        generatedObjects = null;
        Debug.Log("已清除所有生成的物体");
    }

    // 绘制辅助线
    private void OnDrawGizmos()
    {
        if (!drawGizmos || !enabled) return;

        Gizmos.color = gizmoColor;

        // 绘制生成方向
        Gizmos.DrawLine(transform.position, transform.position + generationDirection.normalized * spacing * objectCount);

        // 绘制每个生成点的位置
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 pos = transform.position + generationDirection.normalized * (spacing * i);
            Gizmos.DrawWireCube(pos, Vector3.one * 0.3f);
        }

        // 绘制方向箭头
        if (generationDirection != Vector3.zero)
        {
            Vector3 endPoint = transform.position + generationDirection.normalized * spacing * objectCount;
            DrawArrow(transform.position, endPoint, 0.5f);
        }
    }

    // 绘制箭头辅助方法
    private void DrawArrow(Vector3 start, Vector3 end, float arrowHeadLength = 0.25f)
    {
        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * Vector3.forward;

        Gizmos.DrawRay(end, right * arrowHeadLength);
        Gizmos.DrawRay(end, left * arrowHeadLength);
    }
}