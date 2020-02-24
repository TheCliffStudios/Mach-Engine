using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.ootii.Input;
public class BasicEnemy : MonoBehaviour
{
    public GameObject ExpolosionEffect;

    public void OnTrigger(GameObject Player)
    {
        Instantiate(ExpolosionEffect, transform.position, transform.rotation);
        if (Player.GetComponent<PlayerControler>()._PlayerState == PlayerControler.PlayerState.Homing)
        {
            Player.GetComponent<PlayerControler>().SAnimation.Event(SonicAnimationManager.SAnimationEvent.AttackHit, PlayerControler.PlayerState.Homing);
        }
        if (InputManager.IsPressed("Jump"))
        {
            Player.GetComponent<PlayerControler>().Velocity.y = 5 ;
        }
        else
        {
            Player.GetComponent<PlayerControler>().Velocity = new Vector3(0, 5, 0);
        }
        Destroy(gameObject);
    }
}
