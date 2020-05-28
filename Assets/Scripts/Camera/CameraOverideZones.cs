using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class CameraOverideZones : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Interactable _I = GetComponent<Interactable>();

        _I.OnNearEnter = NearTriggerStay;
        _I.OnNearStay = NearTriggerStay;
        _I.OnNearExit = NearTrigerExit;
    }

    
    public enum COZMode
    {
        Line,
        Tracking,
        Relative
    }

    public enum OffsetWorldSetting
    {
        World,
        Local
    }

    public COZMode _COZMode = COZMode.Tracking;
    public OffsetWorldSetting _ForwardWorld = OffsetWorldSetting.Local;
    public OffsetWorldSetting _OffsetWorld = OffsetWorldSetting.Local;
    public OffsetWorldSetting _UpWorld = OffsetWorldSetting.Local;

    public float _Priority = 1;
    public Vector3 _Offset = new Vector3(0, 1, -1);
    public Vector3 _Forward = new Vector3(0, 0, -1);
    public Vector3 _Up = new Vector3(0, 1, 0);

    public GameObject _TrackingTarget;

    public bool _OnTarget = false;
    public bool _LockForward = false;

    public void NearTriggerStay(GameObject _Player)
    {
        CameraController _CC = GameManagementScript._GameManagement._CameraController;
        if (_CC._CurrentOZ == null)
        {

            _CC._CurrentOZ = this;

        }
        else if ((_CC._CurrentOZ._Priority < _Priority && _CC._CurrentOZ != this))
        {
            _CC._CurrentOZ = this;
        }
    }

    public void NearTrigerExit(GameObject _Player)
    {
        CameraController _CC = GameManagementScript._GameManagement._CameraController;
        if (_CC._CurrentOZ == this)
        {
            _CC._CurrentOZ = null;
        }
    }


}
