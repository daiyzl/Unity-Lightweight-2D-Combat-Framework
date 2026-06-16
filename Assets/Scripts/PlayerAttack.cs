using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public float attackCooldown = 0.4f; // 攻击间隔
    private float lastAttackTime = -999f;

    [Header("剑气视觉")]
    public GameObject slashPrefab;      // 刚才做好的剑气预制体
    public Transform attackPoint;       // 剑气生成的位置（在玩家正前方放一个空物体赋值给它）

    [Header("攻击判定")]
    public float attackRange = 0.8f;    // 判定的圆圈半径
    public LayerMask enemy;        // 敌人的图层

    private PlayerStats playerStats;
    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    void Update()
    {
        // 按下 J 键，且 CD 转好了
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;

        // 1. 生成视觉剑气
        GameObject slash = Instantiate(slashPrefab, attackPoint.position, Quaternion.identity);

        // 如果玩家面朝左边，把剑气也翻转过去
        if (transform.localScale.x < 0)
        {
            slash.transform.localScale = new Vector3(-slash.transform.localScale.x, slash.transform.localScale.y, 1);
        }

        // 2. 物理判定：获取圆圈内的所有敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemy);

        bool hitSomeone = false;

        // 3. 结算伤害
        foreach (Collider2D enemy in hitEnemies)
        {
                // 获取敌人身上的 FSM 大管家组件
                FSM enemyFSM = enemy.GetComponent<FSM>();

                // 只要它身上有 FSM 脚本，就让大管家执行挨打函数！
                if (enemyFSM != null)
                {
                    enemyFSM.TakeDamage(playerStats.AttackPower);
                    hitSomeone = true;
                }

            Debug.Log("砍中了: " + enemy.name);
            hitSomeone = true;
        }

        // 4. 🌟 灵魂注入：如果真的砍中了肉，触发果汁效果！
        if (hitSomeone)
        {
            // 砍中瞬间：世界时间停止 0.05 秒，屏幕剧烈震动 0.1 秒！
            JuiceManager.Instance.HitStop(0.05f);
            JuiceManager.Instance.CameraShake(0.1f, 0.2f);
        }
    }

    // 辅助功能：在 Unity 编辑器里画出这个判定圆圈，方便你调大小
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
