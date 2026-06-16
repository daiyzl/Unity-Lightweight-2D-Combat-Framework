using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public Transform pointA;  // 巡逻点A
    public Transform pointB;  // 巡逻点B
    public float moveSpeed = 2f;
    public float detectRange = 3f; // 检测玩家范围
    public Transform player;
    private Animator anim; // 动画组件
    private enum frogState { idle, run } // 状态枚举
    private Transform targetPoint; // 当前要去的点
    private bool isChasing;       // 是否在追人

    void Start()
    {
        targetPoint = pointA;
        isChasing = false;
        anim= GetComponent<Animator>();
    }
}
