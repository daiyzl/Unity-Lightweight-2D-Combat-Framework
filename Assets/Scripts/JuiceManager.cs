using System.Collections;
using UnityEngine;

public class JuiceManager : MonoBehaviour
{
    // 单例模式，方便全网调用
    public static JuiceManager Instance { get; private set; }

    private Transform cameraTransform;
    private Vector3 cameraOriginalPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 获取主摄像机的 Transform
        cameraTransform = Camera.main.transform;
    }

    // 顿帧 
    public void HitStop(float duration)
    {
        StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        // 瞬间时停
        Time.timeScale = 0f;

        // 因为 timeScale 变成 0 了，传统的 WaitForSeconds 会永远卡死
        // 必须使用 WaitForSecondsRealtime（受现实世界时间影响的等待）
        yield return new WaitForSecondsRealtime(duration);

        // 时间恢复流动
        Time.timeScale = 1f;
    }

    //屏幕震动
    public void CameraShake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        cameraOriginalPos = cameraTransform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 在原位置附近生成一个随机偏移量
            float x = cameraOriginalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = cameraOriginalPos.y + Random.Range(-1f, 1f) * magnitude;

            cameraTransform.position = new Vector3(x, y, cameraOriginalPos.z);

            // 使用 Realtime，保证在顿帧的时候，屏幕依然在剧烈抖动
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cameraTransform.position = cameraOriginalPos;
    }
}
