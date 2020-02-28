using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public GameObject _Player;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null)
        {
            Inter.OnNearEnter(_Player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null)
        {
            Inter.OnNearStay(_Player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable Inter = other.gameObject.GetComponent<Interactable>();
        if (Inter != null)
        {
            Inter.OnNearExit(_Player);
        }
    }
}
