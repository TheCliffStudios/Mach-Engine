using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerList : MonoBehaviour
{
    public List<Collider> _Coliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        _Coliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _Coliders.Remove(other);
    }
}
