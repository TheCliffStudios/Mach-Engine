using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.ootii.Input;
public class BasicEnemy : MonoBehaviour
{
    public GameObject ExpolosionEffect;
    DamageObject.DefenseLevel _DL = DamageObject.DefenseLevel.L2;
    DamageObject.AttackLevel _AL = DamageObject.AttackLevel.L1;
    DamageObject _DM;

    private void Start()
    {
        _DM = gameObject.AddComponent<DamageObject>();
        _DM._OnDamage = OnHit;
        _DM._OnDeath = OnDeath;
    }

    public void OnDeath()
    {
        Instantiate(ExpolosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void OnHit()
    {
        if (_Player.GetComponent<PlayerControler>()._PlayerState == PlayerControler.PlayerState.Homing)
        {
            _Player.GetComponent<PlayerControler>().SAnimation.Event(SonicAnimationManager.SAnimationEvent.AttackHit, PlayerControler.PlayerState.Homing);
        }
        if (_Player.GetComponent<PlayerControler>()._PlayerState == PlayerControler.PlayerState.Homing || ((_Player.GetComponent<PlayerControler>()._PlayerState == PlayerControler.PlayerState.Ball || _Player.GetComponent<PlayerControler>()._PlayerState == PlayerControler.PlayerState.Normal) && !_Player.GetComponent<PlayerControler>()._Grounded))
        {
            if (InputManager.IsPressed("Jump"))
            {
                _Player.GetComponent<PlayerControler>().Velocity.y = 5;
            }
            else
            {
                _Player.GetComponent<PlayerControler>().Velocity = new Vector3(0, 5, 0);
            }
        }

        OnDeath();

    }

    GameObject _Player;

    public void OnTrigger(GameObject Player)
    {
        _Player = Player;

        _DM.TakeDamage(_Player.GetComponent<PlayerControler>()._AttackLevel, _DL);
        _Player.GetComponent<DamageObject>().TakeDamage(_AL, _Player.GetComponent<PlayerControler>()._DefenseLevel);
        
    }
}
