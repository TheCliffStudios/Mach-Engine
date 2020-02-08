using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public float _Range = 10;
    public GameObject _ExplosionEffect;
    // Update is called once per frame
    void Update()
    {
        RaycastHit _Hit;
        Debug.DrawRay(transform.position, GameManagementScript._GameManagement._PlayerObject.transform.position - transform.position);
        if (Physics.Raycast(transform.position, GameManagementScript._GameManagement._PlayerObject.transform.position - transform.position, out _Hit, Mathf.Clamp(_Range, 0, Vector3.Distance(GameManagementScript._GameManagement._PlayerObject.transform.position, transform.position) - 0.1f)))
        {
            Debug.Log("Hit");
            if (_Hit.collider.gameObject == GameManagementScript._GameManagement._PlayerObject)
            {
                GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._HomingTargets.Add(transform.position);
            }
        }
        else
        {
            GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._HomingTargets.Add(transform.position);
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManagementScript._GameManagement._PlayerObject == other.gameObject)
        {
            if (GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._PlayerState == PlayerControler.PlayerState.Homing){
                Rigidbody RB = GameManagementScript._GameManagement._PlayerObject.GetComponent<Rigidbody>();
                Vector3 V = RB.velocity;
                V.y = -V.y + 1;
                GameManagementScript._GameManagement._PlayerObject.GetComponent<Rigidbody>().velocity = V;
                GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._PlayerState = PlayerControler.PlayerState.Normal;

                Instantiate(_ExplosionEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
