using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Area : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Kill player or add some special anim
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //Kill or add some special anim
        }
    }
}
