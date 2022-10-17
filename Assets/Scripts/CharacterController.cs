using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public KeyCode meleeAttack = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public string xAxis = "Horizontal";

    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float groundedLeeway = 0.1f;


    public Transform meleeAttackOrigin;
    public float meleeAttackRadius = 1f;
    public float meleeAttackDelay = 1.1f;
    public float freezeDelay = 0.4f;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;




    private Rigidbody2D rigidbody2D;
    private float moveIntentionX = 0f;

    private bool attemptJump = false;
    private bool meleeAttemptAttack = false;
    private float meleeAttackReady = 0;


    private bool isAttacking = false;
    private bool isFrozen = false;

    private Animator _animator;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        GetInput();

        HandleJump();
        HandleMeleeAttack();

        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("attack01") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            isFrozen = false;
            isAttacking = false;
        }

        if(meleeAttemptAttack)
        {
            if(!isAttacking)
            {
                if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("attack01"))
                {
                    isAttacking = true;
                    isFrozen = true;
                    _animator.CrossFade("attack01", 0);
                }
            }
        }

        if(attemptJump && CheckGrounded() || rigidbody2D.velocity.y > 1f)
        {
            if(!isAttacking)
            {
                if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("jump"))
                {
                    _animator.CrossFade("jump", 0);
                }
            }
        }

        if(!isAttacking)
        {
            if(Mathf.Abs(moveIntentionX) > 0 && CheckGrounded())
            {   
                if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("run"))
                {
                    _animator.CrossFade("run", 0);
                }
            }else if(CheckGrounded()){
                if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    _animator.CrossFade("idle", 0);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, - Vector2.up * groundedLeeway, Color.green);

        if(meleeAttackOrigin != null)
        {
            Gizmos.DrawWireSphere(meleeAttackOrigin.position, meleeAttackRadius);
        }
    }

    private void GetInput()
    {
        moveIntentionX = Input.GetAxis(xAxis);
        meleeAttemptAttack = Input.GetKeyDown(meleeAttack);
        attemptJump = Input.GetKeyDown(jumpKey);
    }

    private void HandleMovement()
    {
        if(moveIntentionX > 0 && !isAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }else if(moveIntentionX < 0 && !isAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }

        if(!isFrozen || !CheckGrounded())
        {
            rigidbody2D.velocity = new Vector2(moveIntentionX * moveSpeed, rigidbody2D.velocity.y);
        }else if(CheckGrounded())
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }

        if(isAttacking)
        {
            if(transform.rotation.y >= 0)
            {
                rigidbody2D.velocity = new Vector2(1f * 0.5f, rigidbody2D.velocity.y);
            }else{
                rigidbody2D.velocity = new Vector2(-1f * 0.5f, rigidbody2D.velocity.y);
            }
        }
    }

    private void HandleJump()
    {
        if(attemptJump && CheckGrounded())
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
        }
    }

    private void HandleMeleeAttack()
    {
        if(meleeAttemptAttack && meleeAttackReady <= 0)
        {
            Collider2D[] overlapedCols = Physics2D.OverlapCircleAll(meleeAttackOrigin.position, meleeAttackRadius, enemyLayer);
            foreach(Collider2D col in overlapedCols)
            {
                EnemyController enemy = col.GetComponent<EnemyController>();
                if(enemy != null)
                {
                    enemy.TakeDamage();
                }
            }

            meleeAttackReady = meleeAttackDelay;
        }else{
            meleeAttackReady -= Time.deltaTime;   
        }
    }

    private bool CheckGrounded()
    {
        bool grounded = Physics2D.Raycast (transform.position, -Vector2.up, groundedLeeway, groundLayer);
        return grounded;
    }
}
