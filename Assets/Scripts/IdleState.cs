using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
//Idle,Patrol,Chase,React,Attack
public class IdleState : IState//静止
{
    StateType enemyState;
    private FSM manager;
    private Paramenter paramenter;
    private float timer;
    //private Animator anim;
    public IdleState(FSM manager)
    {
        this.manager = manager;
        this.paramenter = manager.paramenter;
        enemyState=StateType.Idle;
        //anim=paramenter.animator;
    }
    public void OnEnter()
    {
        Debug.Log("静止状态");
        int val = (int)enemyState;
        //Debug.Log("设置 enemyState = " + val);
        paramenter.animator.SetInteger("enemyState", (int)enemyState);
    }
    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if(timer>=paramenter.idleTime)
        {
            manager.TransitionState(StateType.Patrol);
        }
    }
    public void OnExit()
    {
        timer = 0;
    }
}
//--------------------------------------------------------------------------------------------------------------------
public class PatrolState:IState//巡逻
{
    StateType enemyState;
    private FSM manager;
    private Paramenter paramenter;
    private int PatrolPosition;
    public PatrolState(FSM manager)
    {
        this.manager = manager;
        this.paramenter = manager.paramenter;
        enemyState = StateType.Patrol;
    }
    public void OnEnter()
    {
        int val = (int)enemyState;
        //Debug.Log("设置 enemyState = " + val);
        paramenter.animator.SetInteger("enemyState", (int)enemyState);
    }
    public void OnUpdate()
    {
        manager.FlipTo(paramenter.PatrolPoints[PatrolPosition]);//转向逻辑

        // 🌟 终极防线：视野与领地双重判定
        if (paramenter.target != null)
        {
            // 获取领地的绝对左右边界
            float leftBound = Mathf.Min(paramenter.ChasePoints[0].position.x, paramenter.ChasePoints[1].position.x);
            float rightBound = Mathf.Max(paramenter.ChasePoints[0].position.x, paramenter.ChasePoints[1].position.x);

            float targetPX = paramenter.target.position.x;

            // 只有当玩家明确在追击点范围内，且我自己位于追击点范围内，我才开始追！
            if (targetPX >= leftBound
                && targetPX <= rightBound
                && manager.transform.position.x <= rightBound
                && manager.transform.position.x >= leftBound)
            {
                manager.TransitionState(StateType.Chase);
                return;
            }
        }
        float targetX = paramenter.PatrolPoints[PatrolPosition].position.x;// 提取目标点的 X 坐标
        /*manager.transform.position = Vector2.MoveTowards(manager.transform.position,
        paramenter.PatrolPoints[PatrolPosition].position,
        paramenter.moveSpeed * Time.deltaTime);
        会让敌人的y和检测点的y强行对齐，导致敌人部分陷入地面，修改为x于检测点对齐
        */
        //计算 X 轴的移动量
        float newX = Mathf.MoveTowards(manager.transform.position.x, targetX, paramenter.moveSpeed * Time.deltaTime);

        //X 轴用算出来的移动值，Y 轴保留怪物自己当前的 Y
        manager.transform.position = new Vector2(newX, manager.transform.position.y);
        //Debug.Log("开始巡逻");
        //Debug.Log("巡逻点数为:"+PatrolPosition);
        //Debug.Log("离巡逻点距离:" + Vector2.Distance(manager.transform.position, paramenter.PatrolPoints[PatrolPosition].position));
        if (Vector2.Distance(manager.transform.position, paramenter.PatrolPoints[PatrolPosition].position) < .3f)
            /*
             高度有差距，x重合仍然大于.1f
             改为靠x判断或者修改为.3f
             */
        {
            Debug.Log("切换为静止状态");
            manager.TransitionState(StateType.Idle);//到了巡逻点，切换为静止
        }
    }
    public void OnExit()
    {
        PatrolPosition++;
        PatrolPosition = PatrolPosition % paramenter.PatrolPoints.Length;
    }
}
//-------------------------------------------------------------------------------------------------------------------------------------------
public class ChaseState:IState//追击
{
    StateType enemyState;
    private FSM manager;
    private Paramenter paramenter;
    public ChaseState(FSM manager)
    {
        this.manager = manager;
        this.paramenter = manager.paramenter;
        enemyState = StateType.Chase;
    }
    public void OnEnter()
    {
        int val = (int)enemyState;
        //Debug.Log("设置 enemyState = " + val);
        paramenter.animator.SetInteger("enemyState", (int)enemyState);
    }
    public void OnUpdate()
    {
        manager.FlipTo(paramenter.target);

        // 获取领地的绝对左右边界
        float leftBound = Mathf.Min(paramenter.ChasePoints[0].position.x, paramenter.ChasePoints[1].position.x);
        float rightBound = Mathf.Max(paramenter.ChasePoints[0].position.x, paramenter.ChasePoints[1].position.x);

        if (paramenter.target == null
            || manager.transform.position.x < paramenter.ChasePoints[0].position.x
            || manager.transform.position.x > paramenter.ChasePoints[1].position.x
            || paramenter.target.position.x < leftBound
            || paramenter.target.position.x > rightBound)
        {
            manager.TransitionState(StateType.Patrol);
            return;
        }
            float targetX = paramenter.target.position.x;
            float newX = Mathf.MoveTowards(manager.transform.position.x, targetX, paramenter.chaseSpeed * Time.deltaTime);
            //paramenter.moveSpeed * Time.deltaTime，限制每一秒移动的最大距离

            /*
                manager.transform.position=Vector2.MoveTowards(manager.transform.position,
                paramenter.target.position,
                paramenter.chaseSpeed * Time.deltaTime);
            会导致角色跳起来怪物也吸附上来，修改为只有x轴的追击
            */
            manager.transform.position = new Vector2(newX, manager.transform.position.y);
            //Debug.Log("正在追击");
            //Debug.Log("和玩家距离" + Vector2.Distance(manager.transform.position, paramenter.target.position));

        // 距离够近了，且 CD 已经转好了，才允许进入攻击状态！
        if (Vector2.Distance(manager.transform.position, paramenter.target.position) < 1.0f)
        {
            // 检查：现在的时间 >= 上次攻击的时间 + 冷却时间
            if (Time.time >= paramenter.lastAttackTime + paramenter.attackCooldown)
            {
                manager.TransitionState(StateType.Attack);
                return;
            }
            else
            {
                // 如果 CD 没转好，怪物就只会跟着玩家跑（或者你可以在这里写一个后撤步逻辑）
                Debug.Log("技能 CD 中，只追击，不攻击！");
            }
        }
    }
    public void OnExit()
    {
       
    }
}
//-------------------------------------------------------------------------------------------------------------------------------------------
public class AttackState : IState
{
    private StateType enemyState;
    private Paramenter paramenter;
    private FSM manager;
    private Rigidbody2D rb; // 注意：2D 游戏必须用 Rigidbody2D！
    private SpriteRenderer sr;
    private enemyhitbox myHitBox;

    public bool IsAttacking { get; private set; } = false;
    public AttackState(FSM fsm)
    {
        this.manager = fsm;
        paramenter = fsm.paramenter;
        enemyState = StateType.Attack;

        // 获取组件
        rb = fsm.GetComponent<Rigidbody2D>();
        sr = fsm.GetComponent<SpriteRenderer>();
        myHitBox=fsm.GetComponent<enemyhitbox>();
        //myHitBox=manager.GetComponent<enemyhitbox>();
    }

    public void OnEnter()
    {
        //Debug.Log("进入攻击状态");
        int val = (int)enemyState;
        paramenter.animator.SetInteger("enemyState", val);
        //Debug.Log("设置 enemyState = " + val);

        // 状态一进入，立刻开始整个攻击流！
        manager.StartCoroutine(AttackSequence());
    }

    public void OnUpdate()
    {
        //// 只要正在攻击流程中，屏蔽普通的 Update 逻辑
        //if (IsAttacking) return;
        ////攻击流程结束直接进入静止状态
        //manager.TransitionState(StateType.Idle);
    }

    public void OnExit()
    {
        // 如果因为特殊情况被强行打断（比如怪物死了），停止所有协程
        manager.StopAllCoroutines();
        IsAttacking = false;
        sr.color = Color.white; // 恢复颜色
        // 攻击结束时，记录下当前的游戏时间
        paramenter.lastAttackTime = Time.time;
        //无论协程进行到哪，离开攻击状态必须强制刹车！
        if (rb != null) rb.velocity = Vector2.zero;
    }

    //一个协程管控整个动作流
    private IEnumerator AttackSequence()
    {
        IsAttacking = true;

        // 在攻击刚开始时，记住玩家当前的位置！
        Vector2 lastKnownPos = paramenter.target.position;
        //锁定玩家位置
        Transform lockedTarget = paramenter.target;

        // 在刚进入攻击时，记住怪物自己脚踩的地板高度！
        float originalFloorY = manager.transform.position.y;

        //预警
        rb.velocity = Vector2.zero; // 停下脚步，盯着玩家
        manager.FlipTo(paramenter.target); // 瞄准方向

        Color originalColor = sr.color;
        sr.color = Color.red; // 身体变红，警告玩家

        // 倒计时 0.5 秒，给玩家反应时间
        yield return new WaitForSecondsRealtime(paramenter.telegraphTime);

        // 预警结束，恢复颜色，准备出招！
        sr.color = originalColor;

        // 出招 =================
        // Random.value 会随机返回 0.0 到 1.0 之间的小数
        if (Random.value > 0.5f)
        {
            // 冲撞 
            //Debug.Log("冲撞");

            // 冲撞前，如果玩家还在，更新一次记忆；如果不在了，就用结束协程
            if (lockedTarget != null)
            {
                lastKnownPos = lockedTarget.position;
            }
            else
            {
                yield break;
            }
            // 根据记忆中的位置判断左右
            float dir = lastKnownPos.x > manager.transform.position.x ? 1f : -1f;

            // 给予瞬间的极大水平速度
            Vector2 dashDir = new Vector2(dir, 0);
            rb.velocity = dashDir * paramenter.dashForce;

            if (myHitBox != null) myHitBox.currentDamage = 2;//修改伤害数值
            // 冲刺持续 0.4 秒
            float dashTimer = 0.4f;
            while (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;

                // 发射雷达：从中心点，向冲刺方向，发射长度为 1.0f 的射线
                // 1.0f 是射线的长度，可以根据怪物微调
                RaycastHit2D hit = Physics2D.Raycast(manager.transform.position, dashDir, 1.0f, paramenter.groundLayer);

                if (hit.collider != null)
                {
                    Debug.Log("冲刺撞墙了！");
                    break; // 发现墙壁，直接跳出 while 循环，结束冲刺！
                }

                yield return null;
            }
            if (myHitBox != null) myHitBox.currentDamage = 1;//结束伤害数值修改
            rb.velocity = Vector2.zero; // 确保结束时完全停住
        }
        else
        {
            //下坠 (
           // Debug.Log("发动技能：下坠");

            //原地跃起
            rb.velocity = new Vector2(0, paramenter.jumpForceY); // jumpForceY 可以设为 12f

            //飞到最高点，时间随便
            yield return new WaitForSeconds(0.4f);

            //在空中强行停住 0.2 秒，给玩家闪避提示
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f; // 暂时关闭重力，让它像直升机一样悬停
            yield return new WaitForSeconds(0.2f);//会受伤改为 WaitForSecondsRealtime

            //判断玩家是否还活着
            if (lockedTarget != null)
            {
                lastKnownPos = lockedTarget.position;
            }

            else
            {
                rb.gravityScale = 1f;//恢复重力
                yield break;//关闭协程
            }
            // 向着记忆中的位置下降
            Vector2 diveDirection = (lastKnownPos - (Vector2)manager.transform.position).normalized;
            if (myHitBox != null) myHitBox.currentDamage = 2;//修改伤害值

            //赋予极大的俯冲速度
            float diveSpeed = 25f;
            rb.velocity = diveDirection * diveSpeed;

            /*/等待它砸到地上
            yield return new WaitForSeconds(0.3f);
            会出现怪物穿过地形的问题
            */

            // 只要怪物的高度还在自己原来高度之上，就让它继续飞
            float safetyTimer = 1.0f; // 防卡死保险
            Debug.Log("最低的高度为:" + originalFloorY);
            while (manager.transform.position.y > originalFloorY && safetyTimer > 0f)
            {
                Debug.Log("现在的高度是:" + manager.transform.position.y);
                safetyTimer -= Time.deltaTime;
                yield return null; // yield return null 意思是“等待下一帧”再继续循环
            }
            Debug.Log("技能结束");
            if (myHitBox != null) myHitBox.currentDamage = 1;//结束恢复
            //怪物的高度已经砸到了玩家之前站的地面高度
            rb.velocity = Vector2.zero;
            
            manager.transform.position = new Vector2(manager.transform.position.x, originalFloorY);
            // 加个屏幕震动，或者播放一个特效目前没有

            // 恢复重力
            rb.gravityScale = 1f; // 恢复正常重力！

        }

        //攻击后摇
        rb.velocity = Vector2.zero; // 攻击完后需要喘口气
        yield return new WaitForSeconds(0.5f); // 休息 0.5 秒

        IsAttacking = false;

        // 攻击全部结束，根据距离决定是继续追击，还是原地发呆
        manager.TransitionState(StateType.Chase);
    }
}
