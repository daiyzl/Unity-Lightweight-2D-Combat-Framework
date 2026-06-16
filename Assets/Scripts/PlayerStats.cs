using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerLife;

public class PlayerStats : MonoBehaviour//控制玩家数值
{
    private static PlayerStats _instance;//申明应该静态对象,PlayerStats的实例
    public static PlayerStats Instance
    {
        get
        {
            return _instance;
        }
        set
        {

        }
    }
    [Header("角色属性")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attackPower = 1;
    [SerializeField] private int speed = 5;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int jumpcount = 1;//能连跳的次数
    //public int CurrentHealth
    //{
    //    get
    //    {
    //        return currentHealth;
    //    }
    //}
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int AttackPower => attackPower;
    public int Speed => speed;
    public float JumpForce => jumpForce;

    public int JumpCount => jumpcount;

    public event Action<int, int> HealthChanged;//定义应该事件，当血量变化时传递信息
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // 玩家的血量和攻击力跨关卡继承
             //DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化血量
        currentHealth = maxHealth;
        //Debug.Log("PlayerStats脚本 "+PlayerStats.Instance);
    }

    // 受到伤害的接口
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 限制血量不为负数

        // 广播血量变化事件（用于UI）
        HealthChanged?.Invoke(currentHealth, maxHealth);

        // 如果血量归零，通知 PlayerLife 执行死亡
        if (currentHealth <= 0)
        {
            GetComponent<PlayerLife>().Die();
        }
    }

    // 恢复生命值
    public void Heal(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    // 提升攻击力
    public void IncreaseAttack(int amount)
    {
        attackPower += amount;
    }
}

