using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Input;


public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float _SetDistance = 4;
    public Quaternion _Rotation = Quaternion.identity;
    float Timer = 0;
    public float AutoTime = 0;
    public CameraOverideZones _CurrentOZ = null;
    public Transform _Target;
    public float LerpSpeed = 1;
    float x = 0;
    float y = 0;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    Vector3 _CameraUp = Vector3.up;
    public float _UpSpeed = 2.5f;
    // Update is called once per frame
    void LateUpdate()
    {
        _CameraUp = transform.up;
        GameManagementScript._GameManagement._CameraController = this;
        if (_CurrentOZ == null)
        {
            Vector3 _Forward = Vector3.forward;
            Vector3 _Right = Vector3.right;
            Vector3 _Up = Vector3.up;

            PlayerControler _CC = GameManagementScript._GameManagement._PlayerObject.GetComponent<PlayerControler>();

            _Up = Vector3.Lerp(_CameraUp, _CC._GroundNormal, Time.fixedDeltaTime * _UpSpeed);
           

            x += (InputManager.MouseAxisX + InputManager.RightStickX * 2)*Time.deltaTime * Settings._Settings._CameraSensitivity;
            y += (InputManager.MouseAxisY + InputManager.RightStickY * 2)*Time.deltaTime * Settings._Settings._CameraSensitivity;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            if ((InputManager.MouseAxisX + InputManager.RightStickX) == 0 && (InputManager.MouseAxisY + InputManager.RightStickY) == 0)
            {
                Timer += Time.deltaTime;
                if (Timer > AutoTime)
                {
                    _Forward = Vector3.Lerp(transform.forward, _CC.Velocity, Time.deltaTime / 20);
                    _Rotation = Quaternion.FromToRotation(Vector3.up, _CC.transform.up) * Quaternion.Euler(y, x, 0);
                }
                else
                {
                    _Forward = transform.forward;
                    _Rotation = Quaternion.FromToRotation(Vector3.up, _CC.transform.up) * Quaternion.Euler(y, x, 0);
                }
                
            }
            else
            {
                Timer = 0;

                //y = Mathf.Clamp(-y, -Vector3.Angle(transform.forward, Vector3.up), Vector3.Angle(transform.forward, Vector3.down));
                _Forward = (Quaternion.FromToRotation(Vector3.up, _CC.transform.up) * Quaternion.Euler(y, x, 0)) * Vector3.forward;
                _Rotation = Quaternion.FromToRotation(Vector3.up, _CC.transform.up) * Quaternion.Euler(y, x, 0);
            }

            transform.position = _Target.position + (Quaternion.Lerp(transform.rotation, _Rotation, Time.deltaTime * 10) * new Vector3(0, 0, -_SetDistance));
            transform.rotation = Quaternion.Lerp(transform.rotation, _Rotation, Time.deltaTime * 10);

        }
        else
        {
            if (_CurrentOZ._COZMode == CameraOverideZones.COZMode.Line)
            {
                Vector3 _Forward = _CurrentOZ._Forward;
                Vector3 _Offset = _CurrentOZ._Offset;
                Vector3 _Up = _CurrentOZ._Up;

                if (_CurrentOZ._ForwardWorld == CameraOverideZones.OffsetWorldSetting.Local)
                {
                    if (_CurrentOZ._LockForward)
                    {
                        _Forward = -(_CurrentOZ._TrackingTarget.GetComponent<PathCreation.PathCreator>().path.GetClosestPointOnPath(GameManagementScript._GameManagement._PlayerObject.transform.position) - _CurrentOZ._TrackingTarget.GetComponent<PathCreation.PathCreator>().path.GetClosestPointOnPath(GameManagementScript._GameManagement._PlayerObject.transform.position + _Forward));
                    }
                    else
                    {
                        _Forward = -(_CurrentOZ._TrackingTarget.GetComponent<PathCreation.PathCreator>().path.GetClosestPointOnPath(GameManagementScript._GameManagement._PlayerObject.transform.position) - _CurrentOZ._TrackingTarget.GetComponent<PathCreation.PathCreator>().path.GetClosestPointOnPath(GameManagementScript._GameManagement._PlayerObject.transform.position + GameManagementScript._GameManagement._PlayerObject.transform.forward));
                    }
                    
                }

                if (_CurrentOZ._UpWorld == CameraOverideZones.OffsetWorldSetting.Local)
                {
                    Vector3 _Right = -Vector3.Cross(Vector3.up, _Forward);

                    _Up = Vector3.Lerp(_CameraUp, Vector3.Cross(_Right, _Forward), Time.deltaTime * _UpSpeed);

                    

                }

                if (_CurrentOZ._OffsetWorld == CameraOverideZones.OffsetWorldSetting.Local)
                {
                    _Offset = Quaternion.LookRotation(_Forward, Vector3.up) * _Offset;
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_Forward, _Up), LerpSpeed * Time.deltaTime);
                if (_CurrentOZ._OnTarget)
                {
                    transform.position = Vector3.Lerp(transform.position, _Offset + _CurrentOZ._TrackingTarget.GetComponent<PathCreation.PathCreator>().path.GetClosestPointOnPath(GetComponent<MouseOrbitImproved>().target.position), LerpSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, _Offset + GetComponent<MouseOrbitImproved>().target.position, LerpSpeed * Time.deltaTime);
                }
                
            }
            else if (_CurrentOZ._COZMode == CameraOverideZones.COZMode.Relative)
            {
                Vector3 _Forward = _CurrentOZ._Forward;
                Vector3 _Offset = _CurrentOZ._Offset;
                Vector3 _Up = _CurrentOZ._Up;

                if (_CurrentOZ._UpWorld == CameraOverideZones.OffsetWorldSetting.Local)
                {
                    Vector3 _Right = -Vector3.Cross(Vector3.up, _Forward);

                    _Up = Vector3.Lerp(_CameraUp, GameManagementScript._GameManagement._PlayerObject.transform.up, Time.deltaTime * _UpSpeed);
                    
                }

                if (_CurrentOZ._ForwardWorld == CameraOverideZones.OffsetWorldSetting.Local)
                {
                    //_Forward = Vector3.Cross(Vector3.Cross(Vector3.forward, _Up), GameManagementScript._GameManagement._PlayerObject.transform.up);

                    _Forward = Quaternion.LookRotation(Vector3.Cross(Vector3.Cross(Vector3.forward, _Up), _Up)) * _Forward;
                }

                if (_CurrentOZ._OffsetWorld == CameraOverideZones.OffsetWorldSetting.Local)
                {
                    _Offset = Quaternion.LookRotation(_Forward, Vector3.up) * _Offset;
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((_Forward), _Up), LerpSpeed * Time.deltaTime);
                
                transform.position = Vector3.Lerp(transform.position, _Offset + GetComponent<MouseOrbitImproved>().target.position, LerpSpeed * Time.deltaTime);
                

            }
            else if (_CurrentOZ._COZMode == CameraOverideZones.COZMode.Tracking)
            {
                Vector3 _Forward = _CurrentOZ._Forward;
                Vector3 _Offset = _CurrentOZ._Offset;
            }
        }


        
    }
    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
