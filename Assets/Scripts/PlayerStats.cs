using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;
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

    // --- xLua 热更新组件 ---
    private LuaEnv luaEnv;
    private string luaFilePath;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化 Lua 虚拟机和文件路径
        luaEnv = new LuaEnv();
        luaFilePath = Path.Combine(Application.dataPath, "XLuaFiles", "PlayerConfig.lua.txt");

        // 游戏启动时加载一次数据
        LoadLuaConfig();

        // 初始化血量
        currentHealth = maxHealth;
    }
    private void Update()
    {
        //按下 F5 重新加载玩家配置！
        if (Input.GetKeyDown(KeyCode.F5))
        {
            LoadLuaConfig();

            // 热更新了最大血量后，发个广播，血条跟着变
            HealthChanged?.Invoke(currentHealth, maxHealth);

            Debug.Log("<color=green>[GM] 玩家数据 F5 热重载成功！</color>");
        }
    }

    // 专门负责读取 Lua 数据的核心逻辑
    private void LoadLuaConfig()
    {
        if (File.Exists(luaFilePath))
        {
            string luaCode = File.ReadAllText(luaFilePath);
            luaEnv.DoString(luaCode);

            // 从 Lua 环境中提取变量，覆盖 C# 的变量
            maxHealth = luaEnv.Global.Get<int>("playerMaxHealth");
            attackPower = luaEnv.Global.Get<int>("playerAttack");
            speed = luaEnv.Global.Get<int>("playerSpeed");
            jumpForce = luaEnv.Global.Get<float>("playerJumpForce");
            jumpcount = luaEnv.Global.Get<int>("playerJumpCount");

            // 如果热更新把最大血量改小了，当前血量不能超过最大值
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        else
        {
            Debug.LogError($"找不到玩家配置文件，请检查路径: {luaFilePath}");
        }
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
    // 清理内存
    private void OnDestroy()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
        }
    }
}

