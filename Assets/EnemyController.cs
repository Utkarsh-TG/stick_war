using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float jumpNodeHeightRequirement = 0.8f;
    public float pathUpdateSeconds = 0.5f;
    public float nextWaypointDistance = 1f;

    [Header("Physics")] [SerializeField] private LayerMask groundLayer;
    public Transform jumpCheck;
    public float jumpForce = 15f;
    public float jumpCheckRadius = 0.1f;
    
    [Header(("Enemy"))] 
    public List<string> enemyCategories = new List<string> {"A", "B", "C"};
    [SerializeField] private float enemySpeed;
    [SerializeField] private float enemyHealth;
    [SerializeField] private float enemyFireRate;
    [SerializeField] private int enemyMinDamage;
    [SerializeField] private int enemyMaxDamage;
    public UnityEngine.UI.Slider enemyHealthSystem;

    [Dropdown("enemyCategories")] public string thisEnemyCategory;

    [Header(("Custom Behavior"))] 
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    [Header("ParticleEffects")] 
    public GameObject onLandEffect;
    public GameObject onBurstEffect;
    
    private Path path;
    private int currentWaypoint = 0;
    bool isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    private PlayerController playerController;
    private EnemyManager enemyManager;
    private Vector2 direction;
    private bool objectEntered;
    private Animator anim;
    private bool isShooting;
    [SerializeField] private bool enemyCanShoot;
    
    //------TEST ONLY---//
    public GameObject enemyObject;
    public Transform spawnPoint;
    private int g_damage;
    private DamageTextSpawner damageTextSpawner;
    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        enemyManager = FindObjectOfType<EnemyManager>();
        anim = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        damageTextSpawner = FindObjectOfType<DamageTextSpawner>();
        
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    
        ApplyEnemyProperties();

        enemyCanShoot = true;

        enemyHealthSystem.maxValue = enemyHealth;
    }

    private void ApplyEnemyProperties()
    {
        if (thisEnemyCategory == "C")
        {
            enemyHealth = enemyManager.enemyProperties[0].health;
            enemySpeed = enemyManager.enemyProperties[0].speed;
            enemyFireRate = enemyManager.enemyProperties[0].fireRate;
            enemyMinDamage = enemyManager.enemyProperties[0].minDamage;
            enemyMaxDamage = enemyManager.enemyProperties[0].maxDamage;
        }

        if (thisEnemyCategory == "B")
        {
            enemyHealth = enemyManager.enemyProperties[1].health;
            enemySpeed = enemyManager.enemyProperties[1].speed;
            enemyFireRate = enemyManager.enemyProperties[1].fireRate;
            enemyMinDamage = enemyManager.enemyProperties[1].minDamage;
            enemyMaxDamage = enemyManager.enemyProperties[1].maxDamage;
        }

        if (thisEnemyCategory == "A")
        {
            enemyHealth = enemyManager.enemyProperties[2].health;
            enemySpeed = enemyManager.enemyProperties[2].speed;
            enemyFireRate = enemyManager.enemyProperties[2].fireRate;
            enemyMinDamage = enemyManager.enemyProperties[2].minDamage;
            enemyMaxDamage = enemyManager.enemyProperties[2].maxDamage;
        }
    }

    private void Update()
    {
        enemyHealthSystem.value = enemyHealth;
        
        if (enemyHealth <= 0)
        {
            Instantiate(onBurstEffect, transform.position, transform.rotation);
            // test purposes
            Instantiate(enemyObject, spawnPoint.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector3 startOffset = transform.position -
                              new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckRadius);
        isGrounded = Physics2D.OverlapCircle(jumpCheck.position, jumpCheckRadius, groundLayer);

        // Direction Calculation
        direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;

        // Jump
        if (jumpEnabled && isGrounded)
        {
            if (direction.y >= jumpNodeHeightRequirement)
            {
                Debug.Log("Need to jump.");
                rb.velocity = Vector2.up * jumpForce;
            }
        }

        rb.velocity = new Vector2(direction.x * enemySpeed * Time.deltaTime, rb.velocity.y);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (directionLookEnabled)
        {
            if (direction.x > 0.5f)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (direction.x < 0.5f)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        Debug.Log(enemyCanShoot);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D colliderInfo)
    {
        
        if (colliderInfo.gameObject.CompareTag("Player") && enemyCanShoot)
        {
            EnemyShoot();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.SetBool("isShooting", false);   
        }
    }

    private void EnemyShoot()
    {
        anim.SetBool("isShooting", true);
        g_damage = Random.Range(enemyMaxDamage, enemyMinDamage);
        playerController.PlayerTakeDamage(g_damage);
        damageTextSpawner.GetEnemyDamage(g_damage);
        StartCoroutine("ShootingEnemy");
    }
    
    public void EnemyTakeDamage(int t_damage)
    {
        Debug.Log("EnemyScript: Took damage!");
        enemyHealth -= t_damage;
    }

    IEnumerator ShootingEnemy()
    {
        enemyCanShoot = false;
        yield return new WaitForSeconds(enemyFireRate);
        enemyCanShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Instantiate(onLandEffect, jumpCheck.position, transform.rotation);
        }
    }
}
