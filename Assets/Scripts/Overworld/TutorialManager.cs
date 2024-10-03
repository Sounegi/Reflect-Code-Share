using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance;

    private int currentState;

    private Coroutine spawnCoroutine = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static TutorialManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        currentState = 0;
        LevelManager.GetInstance().Reset();
        StartCoroutine(MissionLog.GetInstance().UpdateLog("Press WASD to move"));
    }

    public void UpdateLog(string message)
    {
        StartCoroutine(MissionLog.GetInstance().UpdateLog(message));
    }

    public void PlayerMoved(InputAction.CallbackContext context)
    {
        if(currentState != 0)
        {
            return;
        }

        if (context.performed && !MissionLog.GetInstance().isUpdating)
        {
            currentState = 1;
            StartCoroutine(MissionLog.GetInstance().UpdateLog("Press Space to Dash"));
        }
    }

    public void PlayerDashed(InputAction.CallbackContext context)
    {
        if(currentState != 1)
        {
            return;
        }

        if (context.performed && !MissionLog.GetInstance().isUpdating)
        {
            currentState = 2;
            StartCoroutine(MissionLog.GetInstance().UpdateLog("Left Click to Reflect"));
        }
    }

    public void PlayerReflected(InputAction.CallbackContext context)
    {
        if(currentState != 2)
        {
            return;
        }

        if (context.performed && !MissionLog.GetInstance().isUpdating)
        {
            currentState = 3;
            StartCoroutine(MissionLog.GetInstance().UpdateLog("Reflect bullets back at enemies"));
            if(spawnCoroutine == null)
            {
                EnemyManager.GetInstance().StartSpawning();
            }
        }
    }

    public void PlayerKilledEnemies()
    {
        if(currentState != 3)
        {
            return;
        }

        currentState = 4;
        PlayerManager.GetInstance().AddVialPoint(10);
        PlayerManager.GetInstance().AdjustHealth(-10);
        StartCoroutine(MissionLog.GetInstance().UpdateLog("Click R to heal"));
    }

    public void PlayerHeal(InputAction.CallbackContext context)
    {
        if(currentState != 4)
        {
            return;
        }

        if (context.performed && !MissionLog.GetInstance().isUpdating)
        {
            currentState = 5;
            StartCoroutine(MissionLog.GetInstance().UpdateLog("Enter portal to exit tutorial"));
            Exit.GetInstance().EnableExit("MainMenu");
        }
    }
}
