using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private GameObject _Player;
    public float _Power;

    public void OnTrigger(GameObject Player)
    {
        //Player.GetComponent<PlayerControler>().Velocity = _Power * transform.up;

        Vector3 LocalV = transform.InverseTransformDirection(Player.GetComponent<PlayerControler>().Velocity);
        LocalV.y = _Power;
        Player.GetComponent<PlayerControler>().Velocity = transform.TransformDirection(LocalV);

        Target _T = GetComponent<Target>();

        if (_T != null)
        {
            _T.OnExit(Player);
        }

    }
}
