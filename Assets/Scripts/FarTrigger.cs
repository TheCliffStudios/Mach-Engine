using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarTrigger : MonoBehaviour
{
    

    public GameObject _Player;

    private void OnTriggerEnter(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null && Inter.OnFarEnter != null)
        {
            Inter.OnFarEnter(_Player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null && Inter.OnFarStay != null)
        {
            Inter.OnFarStay(_Player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null && Inter.OnFarExit != null)
        {
            Inter.OnFarExit(_Player);
        }
    }
}
