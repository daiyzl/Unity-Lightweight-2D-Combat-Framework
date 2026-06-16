using UnityEngine;

public class PoolEffect : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        // 每次被激活时重新播放粒子
        if (ps != null) ps.Play();

        // 第四步：自动回收
        // 建议时间略长于粒子播放时长
        //Debug.Log("粒子已经播放");
        Invoke("DisableSelf", 2.0f);
    }

    private void DisableSelf()
    {
        // 设置为隐藏，对象池就能再次找到它
        //Debug.Log("粒子被隐藏");
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke(); // 预防内存泄漏
    }
}
