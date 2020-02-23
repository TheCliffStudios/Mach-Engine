using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Utility;


public class DashPad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public LayerMask PlayerLayer;
    public bool AdditiveMode = true;
    public float Speed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTrigger(GameObject Player)
    {

        if (AdditiveMode)
        {
            Debug.Log("Additive");
            Player.GetComponent<PlayerControler>().Velocity += Speed * transform.forward;
        }
        else
        {
            //TODO: Stuff
        }
        
    }
}
