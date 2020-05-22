using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTarget : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Interactable>().OnFarEnter = OnTrigger;
        GetComponent<Interactable>().OnFarExit = OnExit;
       
    }

    private GameObject _Player;

    public void OnTrigger(GameObject Player)
    {
        _Player = Player;
        _Player.GetComponent<PlayerControler>()._HomingTargets.Add(gameObject);
    }

    public void OnExit(GameObject Player)
    {
        Player.GetComponent<PlayerControler>()._HomingTargets.Remove(gameObject);
        //Player.GetComponent<PlayerControler>()._PlayerState = PlayerControler.PlayerState.Normal;
    }
}
