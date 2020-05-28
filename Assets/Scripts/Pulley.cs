using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulley : MonoBehaviour
{

    public float _Speed = 5f;
    public Transform _StartPoint;
    public Transform _EndPoint;
    public GameObject _Handle;
    public LineRenderer _LineRenderer;
    public Vector3 Offset = new Vector3(0, -1, 0);
    public bool _Reset = true;

    bool _Active = false;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Interactable>().OnNearEnter = OnNearTriggerEnter;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3[] Points = { _EndPoint.transform.position, _Handle.transform.position };
        _LineRenderer.SetPositions(Points);
        if (_Active)
        {

            GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>().Velocity = Vector3.zero;
            GameManagementScript._GameManagement._PlayerObject.GetComponent<SonicAnimationManager>().WaitTime = 2;
            GameManagementScript._GameManagement._PlayerObject.transform.rotation = _Handle.transform.rotation;
            _Handle.transform.position = ((_EndPoint.position - _StartPoint.position).normalized * Mathf.Clamp(Vector3.Distance(_Handle.transform.position, _EndPoint.position), 0, _Speed * Time.fixedDeltaTime)) + _Handle.transform.position;
            GameManagementScript._GameManagement._PlayerObject.transform.position = _Handle.transform.position + transform.TransformDirection(Offset);

            if (Vector3.Distance(_Handle.transform.position, _EndPoint.position) < 0.1f)
            {
                
                _Active = false;
                GameManagementScript._GameManagement._PlayerObject.GetComponent<SonicAnimationManager>().Event(SonicAnimationManager.SAnimationEvent.Pulley, PlayerControler.PlayerState.InteractOverride);
                GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>().Velocity = _Handle.transform.TransformDirection(Vector3.up * 15f);
                GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._PlayerState = PlayerControler.PlayerState.Normal;
                GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._Ball = true;
            }

        }
        else if (_Reset)
        {
            _Handle.transform.position = (_StartPoint.position - _EndPoint.position).normalized * Mathf.Clamp(Vector3.Distance(_Handle.transform.position, _StartPoint.position), 0, _Speed * Time.fixedDeltaTime) + _Handle.transform.position;
        }
    }

    public void OnNearTriggerEnter(GameObject _Player)
    {
        if (_Active) return;
        Debug.Log("Handle Distance: " + (Vector3.Distance(_Handle.transform.position, _StartPoint.position)));
        if (Vector3.Distance(_Handle.transform.position, _StartPoint.position) < 0.1f)
        {
            GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._Ball = false;
            _Active = true;
            GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._PlayerState = PlayerControler.PlayerState.InteractOverride;
            GameManagementScript._GameManagement._PlayerObject.GetComponent<SonicAnimationManager>().WaitTime = 2;
            GameManagementScript._GameManagement._PlayerObject.GetComponent<SonicAnimationManager>().Event(SonicAnimationManager.SAnimationEvent.Pulley, PlayerControler.PlayerState.Normal);
        }
        else
        {
            GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>()._ValidTarget = false;
        }
        

    }

}
