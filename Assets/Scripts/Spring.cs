using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Interactable>().OnNearEnter = OnTrigger;
    }

    private GameObject _Player;
    public float _Power;

    public enum SpringMode
    {
        Addative,
        Override
    }

    public SpringMode _Settings = SpringMode.Addative;

    public void OnTrigger(GameObject Player)
    {
        //Player.GetComponent<PlayerControler>().Velocity = _Power * transform.up;

        if (_Settings == SpringMode.Addative)
        {
            Vector3 LocalV = transform.InverseTransformDirection(Player.GetComponent<PlayerControler>().Velocity);
            LocalV.y = _Power;
            Player.GetComponent<PlayerControler>().Velocity = transform.TransformDirection(LocalV);
        } else if(_Settings == SpringMode.Override)
        {
            Vector3 LocalV = new Vector3(0, _Power, 0);
            Player.GetComponent<PlayerControler>().Velocity = transform.TransformDirection(LocalV);
        }

        

        Target _T = GetComponent<Target>();

        if (_T != null)
        {
            _T.OnExit(Player);
        }

    }
}
