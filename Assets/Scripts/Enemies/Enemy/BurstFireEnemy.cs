using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BurstFireEnemy : BaseEnemyBehavior { 

    [Header("Rapid Attack Variables")]
    [SerializeField] protected float rapidDelay; //delay between each attack
    [SerializeField] protected int bulletRound; //how many bullet to shoot in a set

    protected override void Awake()
    {
        base.Awake();
        runTo = transform.position;

    }

    protected override void Update()
    {
        if (sleep)
        {
            StartCoroutine(Dizzy());
        }
        else
        {
            playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, isPlayer);
            // playerInDetectRange = Physics2D.OverlapCircle(transform.position, detectRange, isPlayer);

            //if(playerInAttackRange && isAttacking) ReLocation();
            if (playerInAttackRange) AttackPlayer();
            //else if (playerInAttackRange && isAttacking) ReLocation();
            // else if (playerInDetectRange && !playerInAttackRange) Chasing();
            else Chasing();
        }

    }
    protected override void AttackPlayer()
    {
        //agent.isStopped = true;
        //Debug.Log("Attack!");

        if (!isAttacking)
        {
            agent.SetDestination(transform.position);
            StartCoroutine(ShootRoutine());
            isAttacking = true;
        }
        else
        {
            agent.SetDestination(runTo);
        }
    }
    protected override IEnumerator ShootRoutine()
    {
        //Debug.Log("Shoot!");
        for (int i = 0; i < bulletRound; i++)
        {
            animController.SetTrigger(AnimatorAttack);
            //Instantiate(shootEffect, transform.position, Quaternion.identity);
            GameObject bullet = Instantiate(bulletPrefab, agent.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
            if (bullet != null)
            {
                bullet.GetComponent<BaseBulletBehavior>().ShootAt(player);
                EnemyManager.GetInstance().EnemyShoot.Invoke(gameObject.transform.position, enemyName);
            }
            yield return new WaitForSeconds(rapidDelay);
        }
        agent.isStopped = false;
        //Vector3 runFrom = transform.position + (transform.position - player.position + new Vector3(0, Random.Range(-4, 4), 0));
        //agent.SetDestination(runTo);
        yield return new WaitForSeconds(attackDelay);
        runTo = RandomLocation();
        isAttacking = false;

    }
    private Vector3 RandomLocation()
    {
        Vector3 rand = transform.position +  new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
        //new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
        return rand;
    }

    Vector3 runTo;
    //didn't use these, will use it in other enemy
    //////////////
    protected void ReLocation()
    {
        agent.isStopped = false;
        //Vector3 runFrom = transform.position + (transform.position - player.position + new Vector3(0, Random.Range(-4, 4), 0));
        if (transform.position == runTo || runTo == null)
        {
            runTo = RandomLocation();
        }
        agent.SetDestination(runTo);

        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(attackDelay);
        runTo = RandomLocation();
        
    }
    /////////////



}
