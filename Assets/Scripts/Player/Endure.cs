using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endure : MonoBehaviour
{
    [SerializeField] private float duration;

    public void SetProperties(float newdur)
    {
        duration = newdur;
    }
    private void OnEnable()
    {
        PlayerManager.GetInstance().AdjustHealth(1);
        StartCoroutine(DurationCountdown());

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator DurationCountdown()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
