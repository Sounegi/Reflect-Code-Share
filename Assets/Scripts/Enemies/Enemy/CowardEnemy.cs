using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowardEnemy : BaseEnemyBehavior
{
    [Header("Run Components")]
    [SerializeField] private float escapeRange;
    [HideInInspector] [SerializeField] protected bool playerTooClose;

    protected override void Update()
    {
        if (sleep)
        {
            StartCoroutine(Dizzy());
        }
        else
        {
            playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, isPlayer);
            playerTooClose = Physics2D.OverlapCircle(transform.position, escapeRange, isPlayer);


            if (playerInAttackRange && !playerTooClose) AttackPlayer();
            else if (playerTooClose)
            {
                Escaping();
                AttackPlayer();
            }
            else Chasing();
        }

    }

    protected void Escaping()
    {
        //maybe random obstacle to hide every time or select the nearest
        //Transform bindBox = obstacles[Random.Range(0, obstacles.Length)];
        //find nearest obstacle to hide

        //calculate hiding spot and set destination
        Vector3 awayFromPlayer = transform.position + (((transform.position - player.position).normalized) * 2f);//new Vector3(2f, 0, 0);
        agent.SetDestination(awayFromPlayer);
    }

    protected override void OnDrawGizmos()
    {
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, escapeRange);
        }
    }

}
