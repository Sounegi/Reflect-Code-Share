using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempShield : MonoBehaviour
{
    protected int hitPoint = 1;
    //tag as reflector
    public void SetProperties(int newHP)
    {
        hitPoint = newHP;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            hitPoint -= 1;
        }
    }

    private void Update()
    {
        if (hitPoint <= 0) Destroy(this.gameObject);
    }


}
