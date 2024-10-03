using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : BaseBulletBehavior
{
    //Normal Bullet Behavior: Reflectable/detroy when hit wall or at end of life time
    //is trigger
    //Checked
    //Detroy on wall contact
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        //this bullet doesn't bounce against wall and obstacles
        if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacles"))
        {
            Destroy(this.gameObject);
        }
    }
}
