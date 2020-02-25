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
        Debug.Log("NearTriggerEnter");
        DashPad _DP = other.gameObject.GetComponent<DashPad>();

        if (_DP != null)
        {
            _DP.OnTrigger(_Player);
        }

        BasicEnemy _BE = other.gameObject.GetComponent<BasicEnemy>();

        if (_BE != null)
        {
            _BE.OnTrigger(_Player);
        }

        Spring _S = other.gameObject.GetComponent<Spring>();

        if (_S != null)
        {
            _S.OnTrigger(_Player);
        }

        Rail _R = other.gameObject.GetComponent<Rail>();

        if (_R != null)
        {
            _R.OnTrigger(_Player);
        }

        Ring _RI = other.gameObject.GetComponent<Ring>();

        if (_RI != null)
        {
            _RI.OnTrigger();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("NearTriggerStay");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("NearTriggerExit");
    }
}
