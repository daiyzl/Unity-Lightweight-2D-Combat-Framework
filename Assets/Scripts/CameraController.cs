using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField] private Transform target; // 玩家
    [SerializeField] private float smoothSpeed = 5f; // 平滑移动的速度，数值越小越平滑
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, -10f); // 偏移量：摄像机要稍微抬高一点，且Z轴必须是负数

    void LateUpdate() // 用 LateUpdate 解决物理抖动
    {
        if (target == null) return;

        // 计算摄像机应该去的理想位置
        Vector3 desiredPosition = target.position + offset;

        // 使用 Vector3.Lerp 进行平滑过渡计算
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 应用位置
        transform.position = smoothedPosition;
    }
}
