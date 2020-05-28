using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public delegate void OnDamage();
    public delegate void OnDeath();

    public OnDamage _OnDamage;
    public OnDeath _OnDeath;

    public enum AttackLevel
    {
        L1,
        L2,
        L3,
        InstaDeath

    }

    public enum DefenseLevel
    {
        L1,
        L2,
        L3
    }

    public void TakeDamage(AttackLevel Attack, DefenseLevel Defense)
    {
        if (System.Convert.ToInt32(Defense) > System.Convert.ToInt32(Attack))
        {

        }
        else
        {
            if (Attack == AttackLevel.InstaDeath)
            {
                _OnDeath();
            }
            else
            {
                _OnDamage();
            }
        }
    }

    

}
