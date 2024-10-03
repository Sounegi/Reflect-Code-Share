using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    private static HeartManager instance;

    public GameObject heartContainer;

    private float currentHealth;
    private float maxHealth;

    List<GameObject> hearts = new List<GameObject>();

    [Header("Heart Sprites")]
    [SerializeField] Sprite full;
    [SerializeField] Sprite empty;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static HeartManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        currentHealth = PlayerManager.GetInstance().currentHealth;
        maxHealth = PlayerManager.GetInstance().maxHealth;

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject container = Instantiate(heartContainer, gameObject.transform.position + new Vector3(i * 60f + 20f, -40f, 0f), Quaternion.identity, gameObject.transform);

            hearts.Add(container);
        }
    }

    void AdjustHealthUI()
    {
        for (int i = 0; i < maxHealth; i ++)
        {
            if(i < currentHealth)
            {
                hearts[i].GetComponent<Image>().sprite = full;
            }
            else
            {
                hearts[i].GetComponent<Image>().sprite = empty;
            }
        }
    }

    void IncreaseHeartUI()
    {
        for (int i = 0; i < maxHealth; i ++)
        {
            if(i >= hearts.Count)
            {
                GameObject container = Instantiate(heartContainer, gameObject.transform.position + new Vector3(i * 60f + 20f, -40f, 0f), Quaternion.identity, gameObject.transform);

                hearts.Add(container);
            }
        }
    }

    void DecreaseHeartUI()
    {
        for (int i = 0; i < hearts.Count; i ++)
        {
            if(i >= maxHealth)
            {
                Destroy(hearts[i]);
            }
        }
    }

    void FixedUpdate()
    {
        if(currentHealth != PlayerManager.GetInstance().currentHealth)
        {
            currentHealth = PlayerManager.GetInstance().currentHealth;
            AdjustHealthUI();
        }

        if(maxHealth < PlayerManager.GetInstance().maxHealth)
        {
            maxHealth = PlayerManager.GetInstance().maxHealth;
            IncreaseHeartUI();
        }
        else if(maxHealth > PlayerManager.GetInstance().maxHealth)
        {
            maxHealth = PlayerManager.GetInstance().maxHealth;
            DecreaseHeartUI();
        }
    }
}
