using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouncingBullet : BaseBulletBehavior
{
    //Bouncing Bullet Behavior: Reflectable/bouncing again wall and obstacle/destroy when end of life time
    //not trigger
    //Checked
    //Bouncing against wall and obstacle
    [SerializeField] protected Collider2D col;

    protected override void OnEnable()
    {
        base.OnEnable();
        col = GetComponent<Collider2D>();

        //col.enabled = false; 
        col.isTrigger = true;//prevent it to crash with enemy collider
    }

    //special case of our only non-trigger type of bullet
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (status == Status.OWNED_BY_PLAYER)
            {
                Vector2 inNorm = collision.contacts[0].normal;
                //ReflectBullet
                ReflectBullet(inNorm);
            
            }
            else
            {
                PlayerManager.GetInstance().AdjustHealth(-bulletDamage);
                Destroy(this.gameObject);
            }
            
                
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (status == Status.OWNED_BY_PLAYER)
            {
                collision.gameObject.GetComponent<BaseEnemyBehavior>().AdjustHealth(-bulletDamage);
                Destroy(gameObject);
            }
            else
            {
                Vector2 inNorm = collision.contacts[0].normal;
                //ReflectBullet
                ReflectBullet(inNorm);
            }
        }
        //All Reflectable should have this
        else if (collision.gameObject.CompareTag("Reflector"))
        {
            Vector2 inNorm = CameraInstance.GetInstance().GetCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue()) - GameObject.Find("Player").transform.position;

            ReflectBullet(inNorm);

            status = Status.OWNED_BY_PLAYER; //allow bullet to hit enemy maybe reverse back to owned by enemy when we add enemy that can also reflect bullet in the future
        }
        else if (collision.gameObject.CompareTag("Wall") | collision.gameObject.CompareTag("Obstacles"))
        {
            //Debug.Log("Hit " + collision.gameObject.tag);
            //Get direction to bounce
            Vector2 inNorm = collision.contacts[0].normal;
            //ReflectBullet
            ReflectBullet(inNorm);
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            //uncommend this if choose bouncing to not collapse with other bouncing
            col.isTrigger = true;
            rb.velocity = lastvelocity;
            //uncommend this to make bouncing bounce each other
            // Vector2 inNorm = collision.contacts[0].normal;
            //ReflectBullet
            // ReflectBullet(inNorm);
        }


    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //this isn't repeated code this make sur reflector work either it is triiger or not
        //do nothing since this is only non-trigger bullet
        if (collision.gameObject.CompareTag("Reflector"))
        {
            Vector2 inNorm = CameraInstance.GetInstance().GetCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue()) - GameObject.Find("Player").transform.position;

            ReflectBullet(inNorm);

            status = Status.OWNED_BY_PLAYER; //allow bullet to hit enemy maybe reverse back to owned by enemy when we add enemy that can also reflect bullet in the future
        }
        else if (collision.gameObject.CompareTag("RotatingReflect"))
        {
            Vector2 inNorm = CameraInstance.GetInstance().GetCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue()) - GameObject.Find("Player").transform.position;

            ReflectBullet(inNorm);

            status = Status.OWNED_BY_PLAYER; //allow bullet to hit enemy maybe reverse back to owned by enemy when we add enemy that can also reflect bullet in the future
        }

    }

    //make it act normally after leaving enemy
    //another fix: make the bullet appear outside of the enemy collider
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            col.isTrigger = false;
        }
        else if (collider.CompareTag("Bullet"))
        {
            //uncommend this if choose bouncing to not collapse with other bouncing
            //col.isTrigger = false;
        }
        else if (collider.CompareTag("Player") && status == Status.OWNED_BY_PLAYER)
        {
            col.isTrigger = false;
        }
    }

}
