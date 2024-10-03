using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[CreateAssetMenu]
public class Dash : BaseAbillity
{
    Vector2 dashDirection;

    public IEnumerator GoDash(PlayerControlScript control)
    {
        if(control.GetRigidBody().velocity.magnitude < 0.1f || control.dashCounter<=0) yield break;

        control.canDash = false;
        control.currentlyDashing = true;

        dashDirection = control.GetComponent<Rigidbody2D>().velocity.normalized;
        control.GetRigidBody().velocity = dashDirection * control.dashSpeed;
        
        yield return new WaitForSeconds(control.dashDuration);
        control.currentlyDashing = false;

        //add dash charges
        if (control.dashCounter > 0)
        {
            control.dashCounter--;
            control.canDash = true;
            yield break;
        }
        //yield return new WaitForSeconds(control.dashCooldown);
        //control.canDash = true;
    }
}
