using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Vector3 startingPosition;

    void Start()
    {
        LevelManager.GetInstance().Reset();
        EnemyManager.GetInstance().StartSpawning();
        PlayerControlScript.GetInstance().transform.position = startingPosition;
        PlayerManager.GetInstance().Reset();
    }
}
