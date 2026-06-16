using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Pool;
using static allControl;//引入allControl命名空间，以便使用GameManager单例中的变量和方法
using static PlayerStats;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private bool isdeath=false;//定义一个布尔变量isdeath，用于记录角色是否已经死亡
    [SerializeField] private AudioSource deathSoundEffect;//定义一个AudioSource变量，用于播放死亡音效
    private int cherriesIndex;//定义一个整数变量，用于记录当前的樱桃数量

    [Header("冲刺参数设置")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.1f;
    [SerializeField] private float ghostTrailInterval = 0.05f; // 残影生成的时间间隔

    [Header("受到攻击参数")]
    public Vector2 KnockbackForce = new Vector2(8f, 4f); // X是后退力度，Y是向上弹的力度
    public float knockbackDuration = 0.2f; // 僵直持续时间

    // 标记玩家是否正在受击僵直中
    public bool IsKnockedBack { get; private set; }=false;
    private SpriteRenderer playerSR;
    public bool IsDashing {  get; private set; }=false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSR = GetComponent<SpriteRenderer>();
        cherriesIndex = GameManager.Instance.Score;//从GameManager单例中获取当前的樱桃数量
    }
    void Update()
    {
        //增加死亡时候不能暂停
        if (Input.GetKeyUp(KeyCode.Escape)&&!isdeath)//检测玩家是否按下了Esc键
        {
            GameManager.Instance.TogglePause();//按下Esc键时调用TogglePause方法，切换游戏的暂停状态
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)&&!IsDashing&&!isdeath)
        {
            StartCoroutine(DashCoroutine());/*开始一个协程，public Coroutine StartCoroutine(IEnumerator routine)，IEnumerator协程函数的返回值*/
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("trap"))
        {
            deathSoundEffect.Play();
            Die();
        }
        else if (collision.gameObject.CompareTag("enemy"))
        {
            // 防御机制：如果已经在击退/无敌状态中，就不重复受伤（防止被多段判定秒杀）
            if (IsKnockedBack) return;

            enemyhitbox ehb=collision.GetComponent<enemyhitbox>();
            int damageToTake = ehb != null ? ehb.currentDamage : 1;
            PlayerStats.Instance.TakeDamage(damageToTake);

            Debug.Log("受到伤害");
            // 计算方向：敌人在左边(dir=1向右击退)，敌人在右边(dir=-1向左击退)
            int dir = collision.transform.position.x < transform.position.x ? 1 : -1;

            // 开启受击僵直协程
            StartCoroutine(KnockbackRoutine(dir));
        }
    }

    // 处理受击僵直的核心协程
    private IEnumerator KnockbackRoutine(int dir)
    {
        IsKnockedBack = true; // 1. 上锁！
        Debug.Log("开始僵直协程");
        // 2. 赋予击退速度
        rb.velocity = new Vector2(dir * KnockbackForce.x, KnockbackForce.y);

        // 播放受击动画，目前没有
        // anim.SetTrigger("hurt");

        // 3. 等待僵直时间结束
        yield return new WaitForSeconds(knockbackDuration);

        // 4. 僵直结束，解锁！
        IsKnockedBack = false;
    }
    private IEnumerator DashCoroutine()
    {
        IsDashing = true;
        Debug.Log("开始冲刺");
        // 抓取玩家当前的真实朝向
        // 如果 flipX 为 true 表示面朝左，那就往左冲 (-1)，否则往右 (1)
        float dashDir = playerSR.flipX ? -1f : 1f;

        // 冲刺期间关闭重力，防止玩家掉下悬崖边缘产生下坠曲线
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // 赋予冲刺绝对速度 (注意保留原本的 Y 轴速度设为0，实现纯直线冲刺)
        rb.velocity = new Vector2(dashDir * dashSpeed, 0f);

        //  特效逻辑：在冲刺期间，高频从对象池拿出残影
        float dashTimer = 0f;
        while (dashTimer < dashTime)
        {
            // --- 核心动作：从对象池拿出残影并配置 ---
            GameObject ghost = ObjectPoolManager.Instance.GetPooledObject("Ghost_Trail", transform.position, Quaternion.identity); // 使用对象池单例GetPooledObject方法
            if (ghost != null)
            {
                // 让残影完全复制玩家当前的Sprite和朝向
                SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
                if (ghostSR != null && playerSR != null)
                {
                    ghostSR.sprite = playerSR.sprite;
                    ghostSR.flipX = playerSR.flipX; // 复制翻转状态
                }
            }

            yield return new WaitForSeconds(ghostTrailInterval); // 协程等待，不卡主线程
            dashTimer += ghostTrailInterval;
        }
        //冲刺结束，核心扫尾工作（解除锁定）
        rb.gravityScale = originalGravity; // 恢复重力
        rb.velocity = Vector2.zero;        // 刹车

        // 加上短暂的冲刺冷却，防止玩家狂按Shift导致手感崩盘
        yield return new WaitForSeconds(0.2f);
        IsDashing = false; // 解锁
        Debug.Log("冲刺完成");
    }
    public void Die()
    {
        if (isdeath) return;//防止重复死亡
        isdeath=true;//锁定状态，将isdeath变量设置为true，表示角色已经死亡
        allControl.GameManager.Instance.CheckBestScore();//调用GameManager单例中的CheckBestScore方法，检查当前分数是否为最高分数，并进行相应的处理
        anim.SetTrigger("death");
        //rb.bodyType=RigidbodyType2D.Static;//将角色的刚体类型设置为Static，使其无法再受到物理影响
        rb.simulated=false;//禁用刚体的物理模拟，使角色无法再受到物理影响
    }
    private void RestartLevel()
    {
        // 【关键修复】：强制恢复时间和状态
        Time.timeScale = 1f;
        if (GameManager.Instance.isPause)
        {
            GameManager.Instance.TogglePause(); // 确保内部状态也切回非暂停
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);//重新加载当前场景，重置游戏状态
        GameManager.Instance.Score = cherriesIndex;//将樱桃数量重置为之前记录的值，以便在重新开始游戏时保持一致
    }
}
