using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static allControl;//引入allControl命名空间，以便使用GameManager单例中的变量和方法
using static PlayerStats;

public class PlayerPause : MonoBehaviour
{
    [SerializeField] private Text pauseTipText;
    [Header("血量UI设置")]
    public Transform heartParent;//血量挂载的父节点位置
    public GameObject heartPrefab;//单个预制体，里面一个fullHeart图片，一个emptyHeart图片
    public Sprite fullHeart;
    public Sprite emptyHeart;
     void Start()
    {
        //Debug.Log(PlayerStats.Instance);
        // 1. 订阅暂停事件，并立刻同步当前的显示状态
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPauseStateChanged += UpdatePause;//订阅GameManager的OnPauseStateChanged事件，当暂停状态发生变化时调用UpdatePauseTipText方法更新提示文本
            UpdatePause(GameManager.Instance.isPause);//在开始时调用UpdatePauseTipText方法，设置初始的提示文本状态
        }
        // 2. 订阅血量事件
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.HealthChanged += UpdateHealth;

            // 【关键修复】：游戏刚开始时，手动调用一次生成满血爱心
            UpdateHealth(PlayerStats.Instance.CurrentHealth, PlayerStats.Instance.MaxHealth);
        }
        else
        {
            Debug.LogError("PlayerPause: 找不到 PlayerStats 单例，请检查执行顺序或场景配置！");
        }
    }
    void UpdateHealth(int currentHealth, int maxHealth)
    {
        // 安全判断
        if (heartParent == null || heartPrefab == null)
        {
            Debug.LogError("heartParent 或 heartPrefab 未赋值！");
            return;
        }

        // 销毁旧爱心
        foreach (Transform child in heartParent)
            Destroy(child.gameObject);

        // 生成新爱心
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, heartParent);
            Image heartImg = heartObj.GetComponentInChildren<Image>(); // 👈 改成这个！

            if (heartImg != null)
                heartImg.sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }
    void UpdatePause(bool Ispause)
    {
        if (pauseTipText != null)
            pauseTipText.gameObject.SetActive(Ispause);//根据Ispause的值，显示或隐藏暂停提示文本
    }
    void OnDestroy()
    {
        // 空判断，避免重启场景时干扰单例
        if (GameManager.Instance != null)
            GameManager.Instance.OnPauseStateChanged -= UpdatePause;//取消订阅GameManager的OnPauseStateChanged事件，防止内存泄漏

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.HealthChanged -= UpdateHealth;//取消订阅PlayerStats的HealthChanged事件，防止内存泄漏
    }
}
