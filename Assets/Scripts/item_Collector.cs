using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static allControl;//引入allControl命名空间，以便使用GameManager单例中的变量和方法
using static ObjectPoolManager;//ObjectPoolManager命名空间，以便使用ObjectPoolManager单例中的变量和方法

public class item_Collector : MonoBehaviour
{
    
    int cherries;//定义一个整数变量cherries，用于记录收集到的樱桃数量，并从GameManager单例中获取初始值
    [SerializeField] private Text cheeriesText;
    [SerializeField] private AudioSource collectSoundEffect;//定义一个AudioSource变量，用于播放收集音效
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("cherry"))
        {
            collectSoundEffect.Play();//播放收集音效
            ObjectPoolManager.Instance.GetPooledObject("FlashEffect",collision.transform.position,Quaternion.identity);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            GameManager.Instance.Score++;//增加GameManager单例中的分数值，触发OnScoreChanged事件，更新樱桃数量文本
            //Debug.Log($"【item_Collector】当前分数：{GameManager.Instance.score}");//输出当前分数值到控制台，便于调试
        }
    }
    private void Awake()
    {
        cherries = GameManager.Instance.Score;//定义一个整数变量cherries，用于记录收集到的樱桃数量，并从GameManager单例中获取初始值
    }
    private void Start()
    {
        //订阅GameManager的OnScoreChanged事件，当分数发生变化时调用UpdateCherriesText方法更新樱桃数量文本
        GameManager.Instance.OnScoreChanged += UpdateCherriesText;
        //订阅GameManager的OnPauseStateChanged事件，当暂停状态发生变化时调用Ispause方法，根据isPause的值显示或隐藏樱桃数量文本
        GameManager.Instance.OnPauseStateChanged += Ispause;
        UpdateCherriesText(GameManager.Instance.Score);//在开始时调用UpdateCherriesText方法，设置初始的樱桃数量文本
    }
    void UpdateCherriesText(int newScore)
    {
        cheeriesText.text = "cherries: " + newScore;//根据传入的新分数值，更新樱桃数量文本
    }
    void Ispause(bool isPause)
    {
        cheeriesText.gameObject.SetActive(!isPause);//根据isPause的值，显示或隐藏樱桃数量文本
    }
    void OnDestroy()
    {
        //取消订阅GameManager的OnPauseStateChanged事件，防止内存泄漏
        GameManager.Instance.OnPauseStateChanged -= Ispause;
        //取消订阅GameManager的OnScoreChanged事件，防止内存泄漏    
        GameManager.Instance.OnScoreChanged -= UpdateCherriesText;
    }
}
