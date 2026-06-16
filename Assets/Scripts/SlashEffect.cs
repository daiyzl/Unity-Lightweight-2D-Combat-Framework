using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float lifeTime = 0.1f; // 存在的时间极短，0.1秒就消失

    void Start()
    {
        // 一出生，就开启自毁倒计时
        Destroy(gameObject, lifeTime);
    }
}
