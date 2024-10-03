using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadBullet : BaseBulletBehavior
{
    // Spread Bullet Behavior: Reflectable/detroy at end of life time/ spread into smaller bullet(other bullet variation) when getting reflected or pass some time?
    //is trigger
    public GameObject childBulletPrefeb;
    [SerializeField] private int numberOfBulletsOut = 3;
    [SerializeField] private float spreadRange = 15f;
    public float childBulletSpeed;
    //Make bullet spread when it end lifetime
    protected override void EndLifetime()
    {
        if (lifetimeCount >= bulletLifeTime)
        {
            SpreadingBullet();
        }
        
    }
    //Make bullet spread when getting reflected
    public override void ReflectBullet(Vector2 inNorm)
    {
        //base.ReflectBullet(inNorm); //reflect first make it buggy

        SpreadingBullet();
    }

    private void SpreadingBullet()
    {
        Vector2 ori_dir = rb.velocity.normalized;
        //case that number of bullet out = 1
        if (numberOfBulletsOut <= 1)
        {
            GameObject childbullet = Instantiate(childBulletPrefeb, transform.position, transform.rotation);
            Rigidbody2D childRb = childbullet.GetComponent<Rigidbody2D>();
            //set direction
            childRb.velocity = ori_dir * bulletSpeed;
            childbullet.transform.right = ori_dir;
        }
        else
        {
            for (int i = 0; i < numberOfBulletsOut; i++)
            {
                //find direction to go or new target
                float angle = spreadRange * (i - (numberOfBulletsOut - 1) / 2) / (numberOfBulletsOut - 1); //this make bullet outb = 1 bug
                Quaternion spreadQt = Quaternion.AngleAxis(angle, transform.forward) * transform.rotation;
                Vector2 spreadAngle = spreadQt * Vector2.one;
                //Transform spreadTarget = transform; //set new target for child bullet to shoot at *not work*
                GameObject childbullet = Instantiate(childBulletPrefeb, transform.position, transform.rotation);
                Rigidbody2D childRb = childbullet.GetComponent<Rigidbody2D>();
                //set direction
                childRb.velocity = spreadAngle * bulletSpeed;
                childbullet.transform.right = spreadAngle;
                //spreadedbullet.GetComponent<BaseBulletBehavior>().ShootAt(spreadTarget); //*not work*
            }
        }
        Destroy(this.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        //this bullet doesn't bounce against wall and obstacles
        if (collision.gameObject.CompareTag("Wall") | collision.gameObject.CompareTag("Obstacles"))
        {
            Destroy(this.gameObject);
        }
    }
}
