using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    public int index = 0;
    //Used for testing purpose.
    public void SpawnEnemy() {
        
        EnemyManager.GetInstance().SpawnEnemy(index, new Vector3(0, 0, 0));
    }
}
