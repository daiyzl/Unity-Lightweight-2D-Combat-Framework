using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float lifeTime = 0.1f; // 剑气存在的时间极短，0.1秒就消失，凸显速度快

    void Start()
    {
        // 剑气一出生，就开启自毁倒计时
        Destroy(gameObject, lifeTime);
    }
}
