using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidTrigger : MonoBehaviour
{
    public GameObject _Player;

    private void OnTriggerEnter(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null)
        {
            Inter.OnMidEnter(_Player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null)
        {
            Inter.OnMidStay(_Player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null)
        {
            Inter.OnMidExit(_Player);
        }
    }
}
