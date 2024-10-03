using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingBullet : BaseBulletBehavior
{
    // Piercing Bullet Behavior: Reflectable/destroy on wall hit or end of life time/can pierce through obstacle with low speed
    //is trigger
    //Checked
    //possible bug: player reflect this bullet while it still inside wall
    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
        //pierce through obstacle lower its speed
        else if(collision.gameObject.CompareTag("Obstacles"))
        {
            Vector2 cur_dir = rb.velocity.normalized;
            float cur_speed = rb.velocity.magnitude;
            rb.velocity = cur_dir * (cur_speed * 0.5f);
        }
    }

    //getting on of obstacle
    private void OnTriggerExit2D(Collider2D collision)
    {
        //getting on off obstacle return to its normal speed
        if(collision.gameObject.CompareTag("Obstacles"))
        {
            Vector2 cur_dir = rb.velocity.normalized;
            float cur_speed = rb.velocity.magnitude;
            rb.velocity = cur_dir * (cur_speed / 0.5f);
        }
    }

    public override void Activate() {
        PlayerForceOwnership();
        enabled = true;
        GetComponent<EdgeCollider2D>().enabled = true;
    }
}
