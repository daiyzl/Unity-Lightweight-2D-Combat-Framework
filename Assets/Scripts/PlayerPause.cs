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
        // 订阅暂停事件，并立刻同步当前的显示状态
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPauseStateChanged += UpdatePause;//订阅GameManager的OnPauseStateChanged事件，当暂停状态发生变化时调用UpdatePauseTipText方法更新提示文本
            UpdatePause(GameManager.Instance.isPause);//在开始时调用UpdatePauseTipText方法，设置初始的提示文本状态
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
    }
}
