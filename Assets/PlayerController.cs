using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Animator animator;
    private List<GameObject> enemies = new List<GameObject>();
    
    [Header("Movement")]
    public Transform groundPos;
    [SerializeField] private LayerMask groundLayer;
    public float speed;
    public float jumpForce;
    public float checkRadius;
    public float jumpTime;
    [SerializeField] private GameObject onLandParticles;
    [SerializeField] private GameObject onDeathEffect;

    [Header("Shooting")] 
    public int playerHealth;
    public int playerMinDamage;
    public int playerMaxDamage;
    public float playerFireRate;

    [Header(("Supplies"))] 
    public float suppliesTime = 10f;
    public float doubleArmourTime = 0;
    
    private bool isJumping;
    private bool playerIsShooting;
    private bool isGrounded;
    private bool playerCanShoot;
    private bool enemyInTrigger;
    private float jumpTimeCounter;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerCanShoot = true;
    }
    
    void Update()
    {
        if (playerHealth <= 0)
        {
            Instantiate(onDeathEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        isGrounded = Physics2D.OverlapCircle(groundPos.position, checkRadius, groundLayer);

        if (isGrounded && Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) && isGrounded)
        {
            animator.SetTrigger("takeOf");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb2d.velocity = Vector2.up * jumpForce;
        }

        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }
        
        if (isJumping && Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb2d.velocity = new Vector2(moveInput*speed*Time.deltaTime, rb2d.velocity.y);

        if (!Input.GetMouseButton(0) && moveInput == 0)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isRunning", false);
        }
        else if(!Input.GetMouseButton(0) && moveInput != 0)
        {
            animator.SetBool("isRunning", true);
        }
        
        if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }
        if(Input.GetMouseButton(0) && moveInput == 0)
        {
            animator.SetBool("shootOnRun", false);
            animator.SetBool("shootOnStand", true);
            playerIsShooting = true;
        }
        else if (Input.GetMouseButton(0) && moveInput != 0)
        {
            animator.SetBool("shootOnStand", false);
            animator.SetBool("shootOnRun", true);
            playerIsShooting = true;
        }
        if (!Input.GetMouseButton(0))
        {
            animator.SetBool("shootOnStand", false);
            animator.SetBool("shootOnRun", false);
            playerIsShooting = false;
        }
        if (playerIsShooting && playerCanShoot && enemies != null)
        {
            Debug.Log("PlayerScript: Is Shooting!");
            ShootEnemy();
        }
    }

    private void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        if (colliderInfo.gameObject.CompareTag("Enemy"))
        {
            if (enemies.Count == 0)
            {
                enemies.Add(colliderInfo.gameObject);
            }
            else
            {
                foreach (var enemy in enemies)
                {
                    if (enemy != colliderInfo.gameObject)
                    {
                        enemies.Add(colliderInfo.gameObject);
                    }
                }   
            }
        }
    }

    private void OnTriggerExit2D(Collider2D colliderInfo)
    {
        if (colliderInfo.gameObject.CompareTag("Enemy"))
        {
            foreach (var enemy in enemies)
            {
                if (enemy == colliderInfo.gameObject)
                {
                    enemies.Remove(enemy);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if (hitInfo.gameObject.CompareTag("Ground"))
        {
            Instantiate(onLandParticles, groundPos.position, transform.rotation);
        }
    }

    private void ShootEnemy()
    {
        foreach (var enemy in enemies)
        {
            int damage = UnityEngine.Random.Range(playerMinDamage, playerMaxDamage);
            EnemyController enemyManager = enemy.GetComponent<EnemyController>();
            enemyManager.EnemyTakeDamage(damage);
        }
        
        StartCoroutine("ShootingEnemy");
    }

    IEnumerator ShootingEnemy()
    {
        playerCanShoot = false;
        yield return new WaitForSeconds(playerFireRate);
        playerCanShoot = true;
    }

    public void PlayerTakeDamage(int t_damage)
    {
        playerHealth -= t_damage;
    }

    public void ActivateDoubleArmour()
    {
        doubleArmourTime += 5;
        if (doubleArmourTime != 0)
        {
            StartCoroutine("DoubleArmourTimer");
        }
    }

    IEnumerator DoubleArmourTimer()
    {
        playerHealth *= 2;
        yield return new WaitForSecondsRealtime(suppliesTime);
        playerHealth /= 2;
    }
}
