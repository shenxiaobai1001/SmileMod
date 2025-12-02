using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollowXOnly : MonoBehaviour
{
    public Transform target;  // 绑定玩家对象
    public float smoothSpeed = 5f;  // 平滑跟随速度
    public float yPosition = 1.3f;  // 固定的Y轴高度

    void LateUpdate()
    {
        if (target != null)
        {
            bool flowY = GameController.Instance.gameLevel == 7;

           float ypos=flowY ? target.position.y: yPosition;
            if (ypos< yPosition) ypos = yPosition;
            // 仅跟随X轴，Y轴固定
            Vector3 targetPosition = new Vector3(
                target.position.x+1.5f,  // X轴跟随
                ypos,          // Y轴固定
                transform.position.z  // Z轴不变;
            );

            // 平滑移动
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }

        bool protect = ModSystemController.Instance.Protecket;
        if (ModSystemController.Instance.reverseCamera&&!protect)
        {
            transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}