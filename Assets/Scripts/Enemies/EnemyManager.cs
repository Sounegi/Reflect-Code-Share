using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    [SerializeField] private List<GameObject> EnemyPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> SpawnLocations = new List<GameObject>();

    public UnityEvent<Vector3> EnemySpawned;
    public UnityEvent<Vector3, string> EnemyHurt;
    public UnityEvent<Vector3, string> EnemyDie;
    public UnityEvent<Vector3, string> EnemyShoot;
    public UnityEvent AllEnemyDefeated;

    public int enemyAlive;
    public int enemyCount;
    public int enemyLimit;
    public int enemyTotal;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }  
    }

    public static EnemyManager GetInstance() 
    {
        return instance;
    }

    public void StartSpawning()
    {
        enemyAlive = 0;
        enemyCount = 0;

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        SpawnLocations.AddRange(taggedObjects);

        StartCoroutine(Spawner());
    }
    

    public IEnumerator Spawner()
    {
        yield return new WaitForSeconds(1.5f);

        int locationIndex = 0;

        while(true)
        {
            if(enemyCount != enemyTotal && enemyAlive < enemyLimit)
            {
                yield return new WaitForSeconds(0.5f);
                int enemyIndex = Random.Range(0, EnemyPrefabs.Count);
                SpawnEnemy(enemyIndex, SpawnLocations[locationIndex].transform.position);

                locationIndex += 1;
                if(locationIndex >= SpawnLocations.Count)
                {
                    locationIndex = 0;
                }
            }
            else if(enemyCount != enemyTotal && enemyAlive == enemyLimit)
            {
                yield return null;
            }
            else if(enemyCount == enemyTotal && enemyAlive > 0)
            {
                yield return null;
            }
            else
            {
                break;
            }
            
        }
        AllEnemyDefeated?.Invoke();
    }

    public GameObject SpawnEnemy(int index, Vector3 position)
    {
        GameObject enemy = EnemyPrefabs[index];
        Instantiate(enemy, position, Quaternion.identity);
        enemyAlive++;
        enemyCount++;
        EnemySpawned?.Invoke(position);
        return enemy;
    }

    public void SetEnemySelections(List<GameObject> newEnemies, int limit, int total)
    {
        EnemyPrefabs = newEnemies;
        enemyLimit = limit;
        enemyTotal = total;
    }

    /*
    public void StartEnemyWave(int enemyNum) {
        enemyWaveStarted = true;
        for (int i = 0; i < enemyNum; i++) {
            float x = Random.Range(lowerCorner.position.x, upperCorner.position.x);
            float y = Random.Range(lowerCorner.position.y, upperCorner.position.y);
            SpawnEnemy(Random.Range(0, EnemyPrefabs.Count), new Vector3(x, y, 0));
        }
    }

    IEnumerator ListenEnemyExtinct(System.Action callback) {
        yield return new WaitUntil(() => enemyAlive == 0);
        Debug.Log("Enemy Extincted");
        callback.Invoke();
        yield return null;
    }
    */

    public void HandleEnemyDeath(Vector3 deadEnemyPosition, string enemyName)
    {
        enemyAlive--;
        // EnemyDie?.Invoke(deadEnemyPosition, enemyName);
        // EnemyDie?.Invoke();
    }

    public void OpenExit()
    {
        Exit.GetInstance().EnableExit(LevelManager.GetInstance().SelectRandomScene());
        
    }
}
