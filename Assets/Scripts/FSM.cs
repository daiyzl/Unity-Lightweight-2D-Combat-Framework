using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static JuiceManager;
using Unity.Mathematics;

public enum StateType
{
    Idle,Patrol,Chase,React,Attack
}
[Serializable]
public class Paramenter
{
    public int health;
    public float moveSpeed;//正常移动速度
    public float chaseSpeed;//追击移动速度
    public float idleTime;//静止时间
    public int defence;
    public Transform[] PatrolPoints;//巡逻点
    public Transform[] ChasePoints;//追击点
    public Transform target;//目标
    public Animator animator;
    public LayerMask groundLayer;
    // 攻击参数
    public float telegraphTime = 0.5f; // 玩家反应时间（蓄力前摇）
    public float dashForce = 15f;      // 冲撞力度
    public float jumpForceY = 12f;     // 跳砸的起跳力度
    public float smashForceY = -20f;   // 跳砸的下坠力度
    // 攻击冷却相关
    public float attackCooldown = 2.0f; // 两次攻击之间至少间隔 2 秒
    public float lastAttackTime = -999f; // 记录上次攻击的时刻（初始为负数，保证第一次见面就能直接打）
    public Func<int, int, int> luaDamageCalculator;
}
public class FSM : MonoBehaviour
{
    public Paramenter paramenter;
    private IState currentState;
    private Dictionary<StateType,IState> states=new Dictionary<StateType,IState>();
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashCoroutine; //记录当前正在执行的闪烁协程
    //状态转移函数
    public void TransitionState(StateType type)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
    }
    private void Start()
    {
        paramenter.animator = GetComponent<Animator>();
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));
        TransitionState(StateType.Idle);
        sr=GetComponent<SpriteRenderer>();
        originalColor=sr.color;
    }
    private void Update()
    {
        currentState.OnUpdate();
    }
    //更改朝向
    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-3, 3, 3);
            }
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(3, 3, 3);
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            paramenter.target=collision.transform;
            //Debug.Log("检测到玩家");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            paramenter.target = null;
            //Debug.Log("玩家超出追击范围");
        }
    }
    // 处理怪物受伤的逻辑
    public void TakeDamage(int damageAmount)
    {
        int finalDamage = damageAmount;
        if (paramenter.luaDamageCalculator != null)
        {
            int myDefense = paramenter.defence; 

            //把 2 个参数一起喂给 Lua
            finalDamage = paramenter.luaDamageCalculator(damageAmount, myDefense);

            Debug.Log($"[Lua计算] 玩家基础伤害:{damageAmount}，怪物防御:{myDefense}，Lua返回最终伤害:{finalDamage}");
        }
        else
        {
            // 如果没挂载 Lua，就用普通的 C# 减法
            finalDamage = damageAmount - paramenter.defence;
            if (finalDamage < 1) finalDamage = 1;
        }
        // 1. 扣除档案袋里的血量
        paramenter.health -= finalDamage;
        // 2. 在这里播放受击闪红特效、击退或者顿帧
        JuiceManager.Instance.HitStop(0.05f);
        JuiceManager.Instance.CameraShake(0.05f,0.05f);

        // 本体果汁感：触发受击闪烁！
        if (sr != null)
        {
            // 防连击锁：如果当前正在闪，立刻强行打断它！
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            // 开启新的闪烁，并把它记录在案
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        // 死亡判断
        if (paramenter.health <= 0)
        {
            Die();
        }
        // 3. 死亡判断
        if (paramenter.health <= 0)
        {
            Die();
        }
    }

    // 受伤的协程
    private IEnumerator FlashRoutine()
    {
        // 瞬间变成纯白色
        sr.color = Color.cyan;
        
        //用 Realtime，无视顿帧的时间停止
        yield return new WaitForSecondsRealtime(0.2f);

        // 恢复原本的颜色
        sr.color =originalColor;
        //清空记录
        flashCoroutine = null;
    }

    //处理死亡的逻辑
    private void Die()
    {
        //Debug.Log("怪物死亡！");
        // 停止所有动作，销毁怪物（或者播放死亡动画）
        Destroy(gameObject);
    }
}       
