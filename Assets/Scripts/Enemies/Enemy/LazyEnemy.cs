using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyEnemy : BaseEnemyBehavior
{
    [Header ("Lazy Behavior")]
    private Vector3 mySpawnPoint;
    private bool tired;
    [SerializeField] private float lazyDelay;

    protected override void Awake()
    {
        base.Awake();
        mySpawnPoint = transform.position;
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
            playerInDetectRange = Physics2D.OverlapCircle(transform.position, detectRange, isPlayer);


            if (playerInAttackRange) AttackPlayer();
            else if (playerInDetectRange && !playerInAttackRange)
            {
                Chasing();
                tired = false;
            }
            else
            {
                if (tired) Home();
                else
                {
                    Chasing();
                    StartCoroutine(Lazy());
                }
            }

        }

    }
    private IEnumerator Lazy()
    {
        yield return new WaitForSeconds(lazyDelay);
        tired = true;
    }
    protected void Home()
    {
        //animController.SetBool(AnimatorWalk, false);
        agent.SetDestination(mySpawnPoint);
        //Debug.Log("Dest set");
    }


    private Vector3 WalkAroundHome()
    {
        Vector3 rand = mySpawnPoint + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        //new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
        return rand;
    }

    protected override void OnDrawGizmos()
    {
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            
        }
    }
}
