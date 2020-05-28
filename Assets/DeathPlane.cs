using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _DamageObject = gameObject.AddComponent<DamageObject>();
        _DamageObject._OnDamage = OnHit;
        _DamageObject._OnDeath = OnDeath;

        GetComponent<Interactable>().OnNearEnter = OnTrigger;
    }
    public DamageObject _DamageObject;
    public DamageObject.AttackLevel _AL = DamageObject.AttackLevel.InstaDeath;

    public DamageObject.DefenseLevel _DL = DamageObject.DefenseLevel.L3;

    public GameObject _Player;

    public void OnHit()
    {
       

    }

    public void OnDeath()
    {

    }
    public void OnTrigger(GameObject Player)
    {
        _Player = Player;

        _DamageObject.TakeDamage(_Player.GetComponent<PlayerControler>()._AttackLevel, _DL);
        _Player.GetComponent<DamageObject>().TakeDamage(_AL, _Player.GetComponent<PlayerControler>()._DefenseLevel);

    }
}
