using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static allControl;

public class endMenu : MonoBehaviour
{
    public void ReloadGame()
    {
        allControl.GameManager.Instance.CheckBestScore();//调用GameManager单例中的CheckBestScore方法，检查当前分数是否为最高分数，并进行相应的处理
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);//加载当前场景的前一个场景，重新开始游戏
        allControl.GameManager.Instance.ResetGame();//重置游戏数据
    }
}
