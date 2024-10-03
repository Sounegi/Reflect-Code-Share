using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyEnemy : BaseEnemyBehavior
{
    [Header("Sneak Components")]
    [HideInInspector] [SerializeField] private Transform[] obstacles;
    [SerializeField] private Transform myObstacle;
    [SerializeField] private float escapeRange;
    [HideInInspector] [SerializeField] protected bool playerTooClose;

    protected override void Awake()
    {
        base.Awake();
        GameObject[] obs = GameObject.FindGameObjectsWithTag("Obstacles");
        obstacles = new Transform[obs.Length];
        for(int i = 0; i < obs.Length; i++)
        {
            obstacles[i] = obs[i].transform;
        }
        //select obstacle to hide -> will bind to this obstacle
        myObstacle = obstacles[Random.Range(0, obstacles.Length)];

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
            playerTooClose = Physics2D.OverlapCircle(transform.position, escapeRange, isPlayer);


            if (playerInAttackRange && !playerTooClose) AttackPlayer();
            else if (playerTooClose)
            {
                Hiding();
                AttackPlayer();
            }
            else Chasing();
        }

    }

    protected void Hiding()
    {
        //maybe random obstacle to hide every time or select the nearest
        //Transform bindBox = obstacles[Random.Range(0, obstacles.Length)];
        //find nearest obstacle to hide

        //calculate hiding spot and set destination
        Vector3 behindBox = myObstacle.position + (((myObstacle.position - player.position).normalized) * 2f);//new Vector3(2f, 0, 0);
        agent.SetDestination(behindBox);
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
