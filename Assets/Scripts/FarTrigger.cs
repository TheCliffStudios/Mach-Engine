using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarTrigger : MonoBehaviour
{
    

    public GameObject _Player;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("FarTriggerEnter");
        Target _T = other.gameObject.GetComponent<Target>();

        if (_T != null)
        {
            _T.OnTrigger(_Player);
        }

        RailTarget _RT = other.gameObject.GetComponent<RailTarget>();

        if (_RT != null)
        {
            _RT.OnTrigger(_Player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("FarTriggerExit");
        Target _T = other.gameObject.GetComponent<Target>();

        if (_T != null)
        {
            _T.OnExit(_Player);
        }

        RailTarget _RT = other.gameObject.GetComponent<RailTarget>();

        if (_RT != null)
        {
            _RT.OnExit(_Player);
        }
    }
}
