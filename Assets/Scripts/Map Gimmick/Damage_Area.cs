using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Area : MonoBehaviour
{
    [SerializeField] private int damageDeal;
    [SerializeField] private float damageDelay;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Spike_Damage());
        }
    }

    IEnumerator Spike_Damage()
    {

        yield return new WaitForSeconds(damageDelay);
        PlayerManager.GetInstance().AdjustHealth(-damageDeal);
    }
}
