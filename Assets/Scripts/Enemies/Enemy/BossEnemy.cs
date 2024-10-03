using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossEnemy : BaseEnemyBehavior
{
    [Header("Boss Variable")]
    public GameObject secondBulletPrefab;
    public GameObject minionPrefab;
    public Vector3 minionSpawnPoint;
    protected bool warpAttackOnCooldown;
    protected bool warpAttackDone;
    protected bool bullethellAttackOnCooldown;
    protected bool bullethellAttackDone;
    private Vector3 shootingPoint;
    [HideInInspector] [SerializeField] private EnemyHP bossHealthBar;
    [HideInInspector] [SerializeField] private TMP_Text bossName;
    static int AnimatorWarp = Animator.StringToHash("Warp");
    static int AnimatorCast = Animator.StringToHash("Cast");
    static int AnimatorDead = Animator.StringToHash("Dead");
    static int AnimatorRealDead = Animator.StringToHash("Really Dead");
    private Collider2D col;

    public UnityEvent BossDefeated;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
        warpAttackOnCooldown = false;
        warpAttackDone = false;
        bullethellAttackOnCooldown = false;
        bullethellAttackDone = false;

        bossHealthBar = GetComponentInChildren<EnemyHP>();
        bossHealthBar.gameObject.SetActive(false);
        bossName = GetComponentInChildren<TMP_Text>();
        bossName.gameObject.SetActive(false);
    }
    protected override void Update()
    {
        if(currentHealth <= 0) return;

        shootingPoint = transform.position + new Vector3(myFront.x*2, myFront.y*2, 0);
        playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, isPlayer);
        if (sleep)
        {
            if (playerInAttackRange)
            {
                StartCoroutine(WakeUp());
            }
        }
        else
        {
            if (currentHealth >= (maxHealth * 0.7f))
            {
                if (playerInAttackRange) AttackPlayer();
                else Chasing();
            }
            else if (((maxHealth * 0.7f) > currentHealth) && (currentHealth  >= (maxHealth * 0.3f)))
            {
                if (warpAttackOnCooldown && warpAttackDone)
                {
                    if (playerInAttackRange) AttackPlayer();
                    else Chasing();
                }
                else
                {
                    WarpAttack();
                }
            }
            
            else
            {
                if (bullethellAttackOnCooldown && bullethellAttackDone)
                {
                    if (warpAttackOnCooldown && warpAttackDone)
                    {
                        if (playerInAttackRange) AttackPlayer();
                        else Chasing();
                    }
                    else
                    {
                        WarpAttack();
                    }
                }
                else
                {
                    ShootBulletHell();
                }
            }
        }
    }

    protected override IEnumerator ShootRoutine()
    {
        Debug.Log("shoot Norm");
        animController.SetBool(AnimatorWalk, false);
        yield return new WaitForSeconds(0.7f); //attack animation
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        if (bullet != null) 
        { 
            bullet.GetComponent<BaseBulletBehavior>().ShootAt(player);
            EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
        }
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }


    protected IEnumerator Cooldown(float cooldown, int skill)
    {
        yield return new WaitForSeconds(cooldown);
        if(skill == 0)
        {
            warpAttackOnCooldown = false;
        }else if(skill == 1)
        {
            bullethellAttackOnCooldown = false;
        }
    }
    protected IEnumerator WakeUp()
    {
        animController.SetBool(AnimatorCast, true);
        yield return new WaitForSeconds(3f);
        animController.SetBool(AnimatorCast, false);
        bossHealthBar.gameObject.SetActive(true);
        bossName.gameObject.SetActive(true);
        sleep = false;
    }

    private void WarpAttack()
    {
        if(warpAttackOnCooldown == false)
        {
            Debug.Log("Warp");
            col.enabled = false;
            animController.SetBool(AnimatorWalk, false);
            warpAttackDone = false;
            StartCoroutine(Warping());
            warpAttackOnCooldown = true;
        }
        
    }

    protected IEnumerator Warping()
    {
        animController.SetTrigger(AnimatorWarp);
        yield return new WaitForSeconds(1.3f); //wait warp animation
        transform.position = player.position + new Vector3(0, 0, 0);
        col.enabled = true;
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1.2f); ///warp attack animation
        for(int i = 0; i<3; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint, Quaternion.identity);
            yield return new WaitForSeconds(0.7f);
            if (bullet != null)
            {
                bullet.GetComponent<BaseBulletBehavior>().ShootAt(player);
                EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
                
            }
            yield return new WaitForSeconds(0.3f);
        }
        warpAttackDone = true;
        StartCoroutine(Cooldown(10f, 0));
    }

    private void ShootBulletHell()
    {
        if (bullethellAttackOnCooldown == false)
        {
            Debug.Log("Hell");
            bullethellAttackDone = false;
            agent.SetDestination(transform.position);
            StartCoroutine(SummonBulletHell());
            bullethellAttackOnCooldown = true;
        }
    }

    protected IEnumerator SummonBulletHell()
    {
        animController.SetBool(AnimatorCast, true);
        GameObject[] bulletSet = new GameObject[12];
        yield return new WaitForSeconds(2f); //wait casting animation
        for (int j = 0; j<3; j++)
        {
            for (int i = 0; i < 4; i++)
            {//instantiate bullet hell one by one
                bulletSet[i] = Instantiate(secondBulletPrefab, shootingPoint + new Vector3(j*0.2f,2.5f-i,0), Quaternion.identity);
                yield return new WaitForSeconds(0.7f);
            }
            Instantiate(minionPrefab,minionSpawnPoint,Quaternion.identity);
            for (int i = 0; i < 4; i++)
            {
                //shoot all at once
                if (bulletSet[i] != null)
                {
                    bulletSet[i].GetComponent<BaseBulletBehavior>().ShootAt(player);
                    EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
                }
            }
        }
        animController.SetBool(AnimatorCast, false);
        bullethellAttackDone = true;
        StartCoroutine(Cooldown(15f, 1));
    }

    protected override void FixedUpdate()
    {
        myFront = (player.position - transform.position).normalized;
        sprite.flipX = myFront.x <= 0f;
    }

    public override void AdjustHealth(int deltaHealth)
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
            StartCoroutine(DeadAnim());
        }
        else
        {
            EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
        }
        bossHealthBar.UpdateHealth(maxHealth, currentHealth);

        if(currentHealth <= 0)
        {
            LevelManager.GetInstance().WinScene();
        }
    }

    private IEnumerator DeadAnim()
    {
        animController.SetTrigger(AnimatorDead);
        yield return new WaitForSeconds(4f);
        animController.SetBool(AnimatorRealDead, true);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
