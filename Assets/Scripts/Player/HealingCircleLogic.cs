using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCircleLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public float totalHealing;
    public float healing_duration;
    public float current_time;
    private bool playerInsideCircle = false;
    private float healthIncrement;
    private float heal_done;
    public void SetProperties(float totalHeal, float healTime, float radius)
    {
        totalHealing = totalHeal;
        healing_duration = healTime;
        transform.localScale *= radius;
        Debug.Log("Healing total is: " + totalHealing);
        Debug.Log("Healing duration is: " + healing_duration);
        heal_done = 0f;
        
    }

    private void OnEnable()
    {
        current_time = 0f;
        PlayerManager.GetInstance().EmptyVial();
    }

    void Update()
    {
        current_time += Time.deltaTime;
        if (current_time > healing_duration)
        {
            Debug.Log("Healing done: " + heal_done);
            PlayerManager.GetInstance().RoundHP();// AdjustHealth(healthIncrement);
            Destroy(gameObject);
            return;
        }
        if (playerInsideCircle)
        {
            healthIncrement = totalHealing / healing_duration * Time.deltaTime;
            //Debug.Log("Increase health by: " + healthIncrement);
            PlayerManager.GetInstance().AdjustHealth(healthIncrement);
            heal_done += healthIncrement;
        }
    }

    /*
    private IEnumerator HealSmoothly(float total_heal)
    {
        float healpertick = total_heal / healing_duration;
        float elasped_time = 0f;

        while(elapsedTime < healing_duration)
        {
            float current_heal = healpertick * Time.deltaTime;
            PlayerManager.GetInstance().AdjustHealth(current_heal);
        }
    }
    */
    Color newColor;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCircle = true;
            ColorUtility.TryParseHtmlString("#00FF1D", out newColor);
            other.GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCircle = false;
            ColorUtility.TryParseHtmlString("#FFFFFF", out newColor);
            other.GetComponent<SpriteRenderer>().color = newColor;
        }
    }

    private void OnDestroy()
    {
        ColorUtility.TryParseHtmlString("#FFFFFF", out newColor);
        PlayerControlScript.GetInstance().GetComponent<SpriteRenderer>().color = newColor;
    }
}
