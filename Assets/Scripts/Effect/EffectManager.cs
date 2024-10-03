using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
//Require: Player, EnemyManager, Player

//      ATTENTION: PLEASE NEVER CALL THIS SCRIPT FROM ANY OTHER SCRIPT
//      EXCEPT IN Awake() and OnDestroy(), BUT ADD ? AFTER GetInstance()
//      
//      TO COMMUNICATE WITH THIS SCRIPT, INVOKE THE EVENT THIS SCRIPT TO WHICH
//      THIS IS SUBSCRIBING.

public class EffectManager : MonoBehaviour
{
    AudioSource playerAudioSource;

    public static EffectManager instance;

    public UnityEvent bullet_reflected;
    public UnityEvent shield_broken;
    public UnityEvent bullet_captured;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        //DontDestroyOnLoad(gameObject);
    }

    public static EffectManager GetInstance()
    {
        return instance;
    }

    #region Effect
    [Header("Visual Effects")]
    [SerializeField] EffectCollection effectSettings;
    private Dictionary<string, Tuple<GameObject, GameObject, GameObject>> effectDict;    

    [Header("Non-Enemy effects")]
    [SerializeField] private GameObject healVFX;
    [SerializeField] private AudioClip bulletBounceSFX;
    [SerializeField] private AudioClip shieldBreakSFX;
    [SerializeField] private AudioClip playerWalkSFX;
    [SerializeField] private AudioClip playerDashSFX;
    [SerializeField] private AudioClip playerHurtSFX;
    [SerializeField] private AudioClip playerDeathSFX;
    [SerializeField] private AudioClip staminaDepletedSFX;
    [SerializeField] private AudioClip bulletCapturedSFX;
    #endregion
    EnemyManager enemyManagerCopy;// = EnemyManager.GetInstance();
    PlayerManager PlayerManagerCopy;// = PlayerManager.GetInstance();

    private void Start() {
        effectDict = effectSettings.Dict();
        playerAudioSource = PlayerControlScript.GetInstance().NormalPitchSource;
         enemyManagerCopy = EnemyManager.GetInstance();
         PlayerManagerCopy = PlayerManager.GetInstance();
        if (enemyManagerCopy != null)
        {
            enemyManagerCopy.EnemyHurt.AddListener(SpawnHurtEffect);
            enemyManagerCopy.EnemyDie.AddListener(SpawnDeathEffect);
            enemyManagerCopy.EnemyShoot.AddListener(SpawnShootingEffect);
        }

        if (PlayerManagerCopy != null)
        {
            PlayerManager.GetInstance().playerHurtEvent.AddListener(PlayPlayerHurt);
            PlayerManager.GetInstance().playerDieEvent.AddListener(PlayerDeadSound);
        }

    }

    private void OnDisable() {
        if (enemyManagerCopy != null)
        {
            EnemyManager.GetInstance().EnemyHurt.RemoveListener(SpawnHurtEffect);
            EnemyManager.GetInstance().EnemyDie.RemoveListener(SpawnDeathEffect);
            EnemyManager.GetInstance().EnemyShoot.RemoveListener(SpawnShootingEffect);
        }
        if (PlayerManagerCopy != null)
        {
            PlayerManager.GetInstance().playerHurtEvent.RemoveListener(PlayPlayerHurt);
            PlayerManager.GetInstance().playerDieEvent.RemoveListener(PlayerDeadSound);
        }
    }

    //Positional Effects
    public void SpawnShootingEffect(Vector3 position, string enemyName) {
        Instantiate(effectDict[enemyName].Item1, position, Quaternion.identity);
    }

    public void SpawnHurtEffect(Vector3 position, string enemyName) {
        Instantiate(effectDict[enemyName].Item2, position, Quaternion.identity);
    }

    public void SpawnDeathEffect(Vector3 position, string enemyName) {
        Instantiate(effectDict[enemyName].Item3, position, Quaternion.identity);
    }

    public void SpawnHealEffect(Vector3 position, string enemyName) {
        Instantiate(healVFX, position, Quaternion.identity);
    }

    public void PlayPlayerHurt()
    {
        playerAudioSource.PlayOneShot(playerHurtSFX);
    }

    public void PlayerDeadSound()
    {
        playerAudioSource.PlayOneShot(playerDeathSFX);
    }
}
