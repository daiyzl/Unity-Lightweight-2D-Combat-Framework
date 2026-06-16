using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static PlayerStats;
using static PlayerLife;

public class PlayerController : MonoBehaviour
{
    [Header("土狼时间设置")]
    public float coyoteTime = 0.15f;//设置允许脱离平台的跳跃时间
    private float coyoteTimeCount;//脱离平台的跳跃时间记时

    private Rigidbody2D rb;//定义一个Rigidbody2D变量，用于控制角色的物理行为
    private BoxCollider2D coll;//定义一个BoxCollider2D变量，用于检测角色是否在地面上
    private SpriteRenderer sprite;//定义一个SpriteRenderer变量，用于控制角色的外观
    private Animator anim;//定义一个Animator变量，用于控制角色的动画

    [SerializeField]private LayerMask jumpableGround;//定义一个LayerMask变量，用于指定哪些层是可以跳跃的地面
    private float dirX = 0f;
    private int jumpIndex;
    private enum MovementState { idle, running, jumping, falling }//枚举类型，定义角色的状态
    private PlayerLife playerLife;//声明一个PlayerLife类型的变量
    private PlayerStats playerStats;//声明一个Playstats类型的变量
    private float moveSpeed;
    private float jumpForce;
    private int jumpCount;//定义一个整数变量jumpCount，用于记录角色在空中跳跃次数限制

    [SerializeField] private AudioSource jumpSoundEffect;//定义一个AudioSource变量，用于播放跳跃音效
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        sprite=GetComponent<SpriteRenderer>();
        anim=GetComponent<Animator>();
        coll=GetComponent<BoxCollider2D>();
        jumpSoundEffect=GetComponent<AudioSource>();
        playerLife= GetComponent<PlayerLife>();//获取自己的PlayerLife组件
        playerStats= GetComponent<PlayerStats>();
        moveSpeed = playerStats.Speed;
        jumpForce = playerStats.JumpForce;
        jumpCount = playerStats.JumpCount;
    }
        void Update()
    {
        if (playerLife == null || playerLife.IsDashing || playerLife.IsKnockedBack)
        {
            Debug.Log("移动输入已被禁止");
            return;
        }
        if(IsGrounded())
        {
            jumpIndex = jumpCount;//当角色在地面上时，重置连跳机会
            //Debug.Log("jumpIndex " + jumpIndex);
            coyoteTimeCount = coyoteTime;
        }
        else
        {
            coyoteTimeCount-=Time.deltaTime;
        }
        dirX=Input.GetAxisRaw("Horizontal");
        rb.velocity=new Vector2(dirX*moveSpeed,rb.velocity.y);
        bool FirstJump=IsGrounded()||coyoteTimeCount>0;
        bool otherJump =jumpIndex > 0;
        if (Input.GetButtonDown("Jump")&&( FirstJump|| otherJump))
        {
            //Debug.Log("【跳跃逻辑执行中】");
            if(jumpSoundEffect != null)
            jumpSoundEffect.Play();//播放跳跃音效
            else
            {
                Debug.LogWarning("jumpSoundEffect 未赋值！");
            }
            jumpIndex--;//每次跳跃后，减少一次连跳机会
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //Debug.Log("Jumping");
            //Debug.Log("剩余跳跃次数: " + jumpIndex);
            //Debug.Log(otherJump);
        }
        UpdateAnimationState();
    }
    private void UpdateAnimationState()//根据角色的状态更新动画
    {
        MovementState state;
        if (dirX > 0f)
        {
            state = MovementState.running;
            //sprite.flipX = false;
            transform.localScale = new Vector3(3, 3, 3);
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            //sprite.flipX = true;
            transform.localScale = new Vector3(-3, 3, 3);
        }
        else
        {
            state = MovementState.idle;
        }
        if(rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }
        anim.SetInteger("state", (int)state);
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center,
            coll.bounds.size,
            0f, 
            Vector2.down,
            .1f,
            jumpableGround);
    }
}
