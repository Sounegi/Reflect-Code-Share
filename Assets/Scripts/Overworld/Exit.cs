using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private static Exit instance;

    private Collider2D col;
    private SpriteRenderer sprite;
    private string nextScene;
    public bool playerContact = false;

    public UnityEvent ExitTriggered;

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

    public static Exit GetInstance()
    {
        return instance;
    }

    void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;

        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    public void EnableExit(string selectedScene)
    {
        col.enabled = true;
        sprite.enabled = true;
        nextScene = selectedScene;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            playerContact = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            playerContact = false;
        }
    }

    public void ChangeScene()
    {
        if (!playerContact) return;
        ExitTriggered?.Invoke();
        LevelManager.GetInstance().LoadUpgradeScene();
    }
}
