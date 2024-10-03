using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[CreateAssetMenu]
public class DashBlink : BaseAbillity
{
    Vector2 dashDirection;
    public IEnumerator GoDash(PlayerControlScript control)
    {
        
        if(control.dashCounter <= 0) yield break;
        Debug.Log("Dash blinking activated");
        control.BlinkParticle.Play();

        control.canDash = false;
        control.currentlyDashing = true;
        Vector2 dashDirection = CameraInstance.GetInstance().GetCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue()) - control.GetRigidBody().transform.position;
        yield return new WaitForSeconds(control.dashCastTime);

        float desired_range = dashDirection.magnitude;
        float max_range = Mathf.Min(control.blinkRange, desired_range);

        dashDirection.Normalize(); // Normalize the direction

        control.gameObject.transform.position = (Vector2)control.gameObject.transform.position + dashDirection * max_range;
        yield return new WaitForSeconds(control.dashDuration);
        control.currentlyDashing = false;
        control.BlinkParticle.Stop();
        if (control.dashCounter > 0)
        {
            control.dashCounter--;
            control.canDash = true;
            yield break;
        }
    }
}
