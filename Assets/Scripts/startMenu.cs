using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static allControl;//引入allControl命名空间，以便使用GameManager单例中的变量和方法
using UnityEngine.UI;

public class startMenu : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//加载名为"level1"的场景，开始游戏
    }
    [SerializeField] private Text bestText;

    void Start()
    {
        bestText.text = "Best: " + allControl.GameManager.Instance.bestScore;//在开始时设置最佳分数文本，显示GameManager单例中的最高分数值
    }
}
