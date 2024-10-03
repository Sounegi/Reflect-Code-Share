using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BaseEnemyBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject bulletPrefab;
    public string enemyName;
    public bool sleep;

    [Header("Animation Properties")]
    protected Vector3 myFront;
    [HideInInspector][SerializeField] protected Animator animController;
    [HideInInspector][SerializeField] protected SpriteRenderer sprite;
    [HideInInspector][SerializeField] public static int AnimatorWalk = Animator.StringToHash("Walk");
    [HideInInspector][SerializeField] public static int AnimatorAttack = Animator.StringToHash("Attack");

    [Header("Player Variables")]
    [SerializeField] protected Transform player;
    [SerializeField] protected LayerMask isPlayer;

    [Header("Attack Variables")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float detectRange;
    [HideInInspector] [SerializeField] protected bool playerInAttackRange;
    [HideInInspector] [SerializeField] protected bool playerInDetectRange;
    [HideInInspector] [SerializeField] protected bool isAttacking = false;

    [Header("Health Components")]
    [SerializeField] Sprite full;
    [SerializeField] Sprite empty;
    [HideInInspector][SerializeField] List<GameObject> hearts = new List<GameObject>();
    [SerializeField] protected int maxHealth;
    [HideInInspector] [SerializeField] protected int currentHealth;
    [HideInInspector][SerializeField] Coroutine[] heartCoroutines;
    [HideInInspector][SerializeField] private EnemyHP myHealthBar;

    [Header("Effects")]
    [SerializeField] protected GameObject hurtEffect;
    [SerializeField] protected GameObject dieEffect;
    [SerializeField] protected GameObject shootEffect;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        animController = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        //***this 2 line make navmesh work in 2d (must have in everything use navmesh)
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //****
        sleep = true;
        currentHealth = maxHealth;
        

    }

    protected virtual void OnEnable()
    {
        //myHealthBar = GetComponentInChildren<EnemyHP>();
        //myHealthBar.gameObject.SetActive(false);
        /*
        heartCoroutines = new Coroutine[maxHealth];

        foreach (Transform heart in transform)
        {
            hearts.Add(heart.gameObject);
            heart.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        */
        
    }

    //Basic enemy only walk to player and attack
    protected virtual void Update()
    {
        if (sleep)
        {
            StartCoroutine(Dizzy());
            //animController.SetBool(AnimatorWalk, false);
        }
        else
        {
            playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, isPlayer);
            // playerInDetectRange = Physics2D.OverlapCircle(transform.position, detectRange, isPlayer);
            

            if (playerInAttackRange) AttackPlayer();
            // else if (playerInDetectRange && !playerInAttackRange) Chasing();
            else Chasing();
        }
        
    }

    protected virtual void FixedUpdate()
    {
        myFront = (player.position - transform.position).normalized;
        sprite.flipX = myFront.x > 0f;
    }

    protected virtual void Idle()
    {
        animController.SetBool(AnimatorWalk, false);
        agent.isStopped = true;
        agent.SetDestination(transform.position);
    }

    //just walk until reach attack range
    protected virtual void Chasing()
    {
        //animController.SetBool(AnimatorWalk, true);
        agent.isStopped = false;
        agent.SetDestination(player.position);

    }

    protected IEnumerator Dizzy()
    {
        yield return new WaitForSeconds(5f);
        sleep = false;
    }

    public void Knockback(Transform collision, float knockbackForce)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector2 knockbackDirection = (transform.position - collision.position).normalized;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 currentVelocity = rb.velocity;
                Vector2 knockbackVelocity = knockbackDirection * knockbackForce;
                Vector2 newVelocity = new Vector2(
                    Mathf.Abs(knockbackVelocity.x) > Mathf.Abs(currentVelocity.x) ? 0 : currentVelocity.x,
                    Mathf.Abs(knockbackVelocity.y) > Mathf.Abs(currentVelocity.y) ? 0 : currentVelocity.y
                );

                rb.velocity = newVelocity;

                rb.AddForce(knockbackVelocity, ForceMode2D.Impulse);
            }
        }
    }

    protected virtual void AttackPlayer()
    {
        //agent.isStopped = true;
        
        
        if (!isAttacking)
        {
            //animController.SetTrigger(AnimatorAttack);
            agent.SetDestination(transform.position);
            StartCoroutine(ShootRoutine());
            isAttacking = true;
        }
    }

    protected virtual IEnumerator ShootRoutine()
    {

        //Instantiate(shootEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        if (bullet != null)
        {
            bullet.GetComponent<BaseBulletBehavior>().ShootAt(player);
            EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
        }
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        
    }

    public virtual void AdjustHealth(int deltaHealth)
    {
        currentHealth += deltaHealth;

        //UpdateHearts();
        //myHealthBar.UpdateHealth(maxHealth, currentHealth);
        //StartCoroutine(ShowHealthbar(myHealthBar));

        //Instantiate(currentHealth <= 0 ? dieEffect: hurtEffect, transform.position, Quaternion.identity);
        
        if (currentHealth <= 0)
        {
            EnemyManager.GetInstance().EnemyDie.Invoke(transform.position, enemyName);
            PlayerManager.GetInstance().AddVialPoint(1);
            Destroy(this.gameObject);
        }
        else {
            EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            /*
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            */
        }
    }

    private void OnDestroy()
    {
        EnemyManager.GetInstance().HandleEnemyDeath(transform.position, enemyName);
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < maxHealth; i ++)
        {
            if (heartCoroutines[i] != null)
            {
                StopCoroutine(heartCoroutines[i]);
            }

            heartCoroutines[i] = StartCoroutine(ShowHeart(hearts[i], i));

            if(i < currentHealth)
            {
                hearts[i].GetComponent<SpriteRenderer>().sprite = full;
            }
            else
            {
                hearts[i].GetComponent<SpriteRenderer>().sprite = empty;
            }
        }
    }

    IEnumerator ShowHeart(GameObject heart, int idx)
    {
        heart.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(1f);
        heart.GetComponent<SpriteRenderer>().enabled = false;
        heartCoroutines[idx] = null;
    }

    IEnumerator ShowHealthbar(EnemyHP healthbar)
    {
        healthbar.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        healthbar.gameObject.SetActive(false);
    }

    //check if enemy reach destination
    //not working
    protected bool pathComplete(Vector3 dest)
    {
        if (transform.position.x - dest.x <= 0.1f && transform.position.y - dest.y <= 0.1f)
        {
            return true;
        }

        return false;
    }
}
