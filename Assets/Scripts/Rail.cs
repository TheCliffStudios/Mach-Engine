using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Utility;

public class Rail : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Interactable>().OnNearEnter = OnTrigger;
    }

    public PathCreation.PathCreator _Path;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTrigger(GameObject Player)
    {
        Player.GetComponent<PlayerControler>()._PlayerState = PlayerControler.PlayerState.RailGrinding;
        Player.GetComponent<PlayerControler>()._Rail = this;
    }
}
