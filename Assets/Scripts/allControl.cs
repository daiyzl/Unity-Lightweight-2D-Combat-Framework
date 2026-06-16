using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class allControl : MonoBehaviour
{
    //单例模式
    public class GameManager
    {
        private static GameManager instance;//定义一个静态变量instance，用于存储GameManager的实例
        private GameManager()//定义一个私有构造函数，防止外部实例化GameManager
        {
            //_bestScore = PlayerPrefs.GetInt("bestScore", 0);//在私有构造函数中，从PlayerPrefs中获取保存的最高分数，如果没有保存则默认为0
        }
        private int _score = 0;//定义一个静态整数变量score，用于记录游戏的分数
        public bool isPause = false;//定义一个静态布尔变量isPause，用于记录游戏是否处于暂停状态
        public event Action<int> OnScoreChanged;//定义一个事件OnScoreChanged，当分数发生变化时触发，传递新的分数值
        public event Action<bool> OnPauseStateChanged;//定义一个事件OnPauseStateChanged，当暂停状态发生变化时触发，传递新的暂停状态值
        private int _bestScore = 0;//定义一个静态整数变量bestScore，用于记录游戏的最高分数
        public static GameManager Instance//定义一个公共静态属性Instance，用于获取GameManager的实例
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();//如果instance为null，创建一个新的GameManager实例并赋值给instance变量
                    instance._bestScore = PlayerPrefs.GetInt("bestScore", 0);//在私有构造函数中，从PlayerPrefs中获取保存的最高分数，如果没有保存则默认为0
                }
                    return instance;//返回instance变量，即GameManager的实例
            }
        }
        public int bestScore
        {
            get => _bestScore;//获取最高分数时返回bestScore变量的值
        }
        public int Score//定义一个公共属性Score，用于获取和设置分数
        {
            get => _score;//获取分数时返回score变量的值
            set
            {
                _score = value;//设置分数时将传入的值赋给score变量
                OnScoreChanged?.Invoke(_score);//触发OnScoreChanged事件，传递新的分数值
            }
        }
        public bool IsPause//定义一个公共属性IsPause，用于获取和设置暂停状态
        {
            get=> isPause;//获取暂停状态时返回isPause变量的值
            set
            {
                isPause = value;//设置暂停状态时将传入的值赋给isPause变量
                OnPauseStateChanged?.Invoke(isPause);//触发OnPauseStateChanged事件，传递新的暂停状态值
            }
        }
        public void TogglePause()//定义一个方法TogglePause，用于切换游戏的暂停状态
        {
            IsPause = !IsPause;//切换IsPause属性的值
            if (IsPause)
            {
                Time.timeScale = 0f;//如果游戏处于暂停状态，将时间缩放设置为0，停止游戏的更新和物理计算
                Debug.Log("游戏已暂停");
            }
            else
            {
                Time.timeScale = 1f;//如果游戏不处于暂停状态，将时间缩放设置为1，恢复游戏的正常更新和物理计算
                Debug.Log("游戏已恢复");
            }
            OnPauseStateChanged?.Invoke(IsPause);//触发OnPauseStateChanged事件，传递新的暂停状态值
        }
        public void ResetGame()//定义一个方法ResetGame，用于重置游戏状态
        {
            Score = 0;//将分数重置为0
            IsPause = false;//将暂停状态重置为false
            Time.timeScale = 1f;//将时间缩放设置为1，确保游戏处于正常更新状态
            Debug.Log("游戏已重置");
        }
        public void CheckBestScore()//定义一个方法CheckBestScore，用于检查当前分数是否超过最高分数，并更新最高分数
        {
            if (Score > bestScore)//如果当前分数大于最高分数
            {
                _bestScore = Score;//将当前分数赋值给bestScore变量，更新最高分数
                PlayerPrefs.SetInt("bestScore", _bestScore);//将新的最高分数保存到PlayerPrefs中，以便在下次游戏启动时加载
                Debug.Log($"新的最高分数：{_bestScore}");//输出新的最高分数到控制台，便于调试
                PlayerPrefs.Save();//保存PlayerPrefs中的数据，确保最高分数被持久化存储
            }
        }
    }
}
