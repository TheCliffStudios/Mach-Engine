using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Input;
public class PlayerControler : MonoBehaviour {

    public GameObject _AnimationBody;

    public List<Vector3> _HomingTargets;
    public bool _ValidTarget = false;
    public Vector3 _HomingTarget;
    
    public Vector3 AirVelocity
    {
        get
        {
            return Velocity - GroundVelocity;
        }
        set
        {
            Velocity = value + GroundVelocity;
        }
    }

    public Vector3 Velocity = Vector3.zero;
    

    public Vector3 LocalVelocity
    {
        get
        {
            return transform.InverseTransformDirection(Velocity);
        }
        set
        {
            Velocity = transform.TransformDirection(value);
        }
    }

    public Vector3 GroundVelocity
    {
        get
        {
            return Vector3.ProjectOnPlane(Velocity, _GroundNormal);
        }
        set
        {

            Velocity = value + AirVelocity;
        }
    }

    public Vector3 LocalGroundVelocity
    {
        get
        {
            return transform.InverseTransformDirection(Vector3.ProjectOnPlane(Velocity, _GroundNormal));
        }
        set
        {
            Velocity = transform.TransformDirection(value) + AirVelocity;
        }
    }

    Trans _CameraTransformDuplicate;

    Vector3 _MoveInput = new Vector3();
    Vector3 _RawInput = new Vector3();

    public Vector3 _Gravity = new Vector3(0, -9.81f, 0);
    public Vector3 _GroundNormal = Vector3.up;
    public Vector3 _V;

    float InitialInputMag;
    float InitialLerpedInput;

    public float GroundRaycastLengthMin = 0.5f;
    public float GroundRaycastLengthMax = 2f;

    public float Height = 0.25f;
    public float WallSideDistance = 0.01f;

    public float ControlSpeed = 20;
    public float MaximumSpeed = 40;
    public float Acell = 0.6f;
    public float AirAcell = 1.2f;
    public float Decel = 0.4f;
    public float TurningSpeed = 2.5f;
    public float _VelocityGroundAngle = 40f;
    public bool UtopiaTurning = false;
    public bool _Grounded;
   
    bool Jumping = false;
    Rigidbody RB;

    public AnimationCurve HillToSpeed;

    public float _MaxGroundStandingAngle = 30;

    public Vector2 WallStickAngleRange = new Vector2(50, 130);

    public enum PlayerState
    {
        Normal,
        GroundPound,
        RailGrinding,
        Homing,
        WallSlide
    }

    public PlayerState _PlayerState = PlayerState.Normal;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        GameManagementScript._GameManagement._PlayerObject = gameObject;
        _CameraTransformDuplicate = new Trans(Camera.main.transform);
        Velocity = RB.velocity;
    }

    private void Update()
    {
    	_CameraTransformDuplicate = new Trans(Camera.main.transform);

        //CameraControl(Time.deltaTime);

        // Get curve position

        

        

        // Get the axis and jump input.

        float h = InputManager.LeftStickX; //Input.GetAxis("Horizontal");
        float v = InputManager.LeftStickY; //Input.GetAxis("Vertical");

        if (InputManager.IsPressed("Up"))
        {
            v = 1;
        }

        if (InputManager.IsPressed("Down"))
        {
            v = -1;
        }

        if (InputManager.IsPressed("Right"))
        {
            h = 1;
        }

        if (InputManager.IsPressed("Left"))
        {
            h = -1;
        }
        
        // calculate move direction
        Vector3 moveInp = new Vector3(h, 0, v);
        InitialInputMag = moveInp.sqrMagnitude;
        InitialLerpedInput = Mathf.Lerp(InitialLerpedInput, InitialInputMag, Time.deltaTime);
        
        Vector3 transformedInput = Quaternion.FromToRotation(_CameraTransformDuplicate.Up, transform.up) * (_CameraTransformDuplicate.rotation * moveInp);
        transformedInput = transform.InverseTransformDirection(transformedInput);
        transformedInput.y = 0.0f;
        _RawInput = moveInp;
        moveInp = transformedInput;
        
        _MoveInput = moveInp;
        //if (Time.frameCount % 5 == 0) { Jumping = false; }

        float _Angle = 90;

        _ValidTarget = false;

        foreach (Vector3 Target in _HomingTargets)
        {
            if (_Angle > Vector3.Angle(_CameraTransformDuplicate.Forward, Target - _CameraTransformDuplicate.position))
            {
                _Angle = Vector3.Angle(_CameraTransformDuplicate.Forward, Target - _CameraTransformDuplicate.position);
                _ValidTarget = true;
                _HomingTarget = Target;
            }
        }

        if (_Grounded)
        {
            _ValidTarget = false;
        }

       

        _HomingTargets.Clear();

        if (InputManager.IsJustPressed("Jump")  &&  _Grounded)
        {
            AirVelocity = _GroundNormal * 10;
            _AnimationBody.GetComponent<Animator>().Play("Ball Loop");
            Jumping = true;
            _Grounded = false;
        }
        else if (InputManager.PressedTime("Jump") > 2f) 
        {

            Jumping = false;

        }
        if (InputManager.IsJustReleased("Jump"))
        {
            
        }
        else
        {
            if (InputManager.IsJustPressed("Jump") && _ValidTarget && !Jumping)
            {
                Velocity = (_HomingTarget - transform.position).normalized * Mathf.Clamp(AirVelocity.magnitude, MaximumSpeed/2, MaximumSpeed);
                _AnimationBody.GetComponent<Animator>().Play("Ball Loop");
                _PlayerState = PlayerState.Homing;
            }
        }

        if (InputManager.IsJustPressed("GroundPound") && !_Grounded)
        {
            _PlayerState = PlayerState.GroundPound;
            _AnimationBody.GetComponent<Animator>().Play("Stomp");
            Jumping = false;
            Velocity = new Vector3(0, -20, 0);
        }

        
    }

    private void FixedUpdate()
    {
        Velocity = Vector3.Lerp(Velocity, RB.velocity, 1 * Time.fixedDeltaTime);
        GeneralPhysics();
        RB.velocity = Velocity;
        DebugText._DebugText._Velocity = Velocity;
    }

    void GeneralPhysics()
    {
        if (_PlayerState == PlayerState.Normal)
        {
            PlayerState StartState = _PlayerState;
            float lerp = 0.1f;
            RaycastHit _Hit;
            Vector3 RayDirection = Vector3.zero;

            if (_PlayerState == PlayerState.Normal)
            {
                RayDirection = -transform.up;
            }
            else
            {
                RayDirection = transform.forward;
            }

            float RayRange = 0;

            if (_Grounded)
            {
                RayRange = Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed);
                if (_PlayerState == PlayerState.WallSlide)
                {
                    RayRange += 0.5f;
                }
            }
            else
            {
                RayRange = GroundRaycastLengthMin;
            }

            Debug.DrawRay(transform.position + -RayDirection * 0.5f, (RayDirection) * RayRange, Color.blue, 0.01f);
            if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, out _Hit, RayRange))
            {
                Vector3 Normal = Vector3.zero;

                for (float X = -0.1f; X < 0.1f; X += 0.1f)
                {
                    for (float Y = -0.1f; Y < 0.1f; Y += 0.1f)
                    {
                        Vector3 XY = transform.TransformDirection(new Vector3(X, Y, 0));
                        RaycastHit Hit = new RaycastHit();
                        if (Physics.Raycast(transform.position + XY + -RayDirection * 0.5f, RayDirection, out Hit, RayRange))
                        {
                            Normal += Hit.normal;

                        }
                    }
                }
                Normal = Normal.normalized;
                //AirVelocity = Vector3.zero;
                _Grounded = true;
                //transform.position = _Hit.point + _Hit.normal * Height;
                if (Vector3.Angle(Vector3.up, _Hit.normal) > WallStickAngleRange.x && Vector3.Angle(Vector3.up, _Hit.normal) < WallStickAngleRange.y && GroundVelocity.magnitude < MaximumSpeed / 10 && _PlayerState == PlayerState.Normal)
                {
                    _PlayerState = PlayerState.WallSlide;

                    Velocity = Vector3.zero;
                    _Grounded = true;
                    _GroundNormal = _Hit.normal;
                    Vector3 Up = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, Vector3.up));
                    Quaternion Q = Quaternion.LookRotation(-_Hit.normal, Up);
                    Rotate(Q, 0);
                    RB.isKinematic = true;
                    RB.MovePosition(_Hit.point + _GroundNormal * WallSideDistance);
                }
                else if (_PlayerState == PlayerState.Normal)
                {
                    _PlayerState = PlayerState.Normal;
                    RB.isKinematic = false;
                }
                else
                {

                }
                Debug.Log(Vector3.Angle(Vector3.up, Normal));


                if (GroundVelocity == Vector3.zero)
                {
                    if (Vector3.Angle(Vector3.up, Normal) > _MaxGroundStandingAngle && _PlayerState == PlayerState.Normal)
                    {
                        Debug.Log("Higher");
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(Normal, -Vector3.Cross(Normal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, Normal);

                        Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.75f);

                    }
                    else if (_PlayerState == PlayerState.Normal)
                    {
                        Debug.Log("Lower");
                        Vector3 F2 = Vector3.Cross(Normal, -Vector3.Cross(Normal, transform.forward));
                        Quaternion Q2 = Quaternion.LookRotation(F2, Normal);
                        Rotate(Q2, 0.75f);
                    }
                    _GroundNormal = Vector3.Lerp(_GroundNormal, Normal, lerp);
                }
                else
                {
                    if (Vector3.Angle(Vector3.up, Normal) > _MaxGroundStandingAngle && _PlayerState == PlayerState.Normal)
                    {
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(Normal, -Vector3.Cross(Normal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, Normal);

                        Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.5f);

                    }
                    else if (_PlayerState == PlayerState.Normal)
                    {
                        Vector3 F2 = Vector3.Cross(Normal, -Vector3.Cross(Normal, transform.forward));
                        Quaternion Q2 = Quaternion.LookRotation(F2, Normal);
                        Rotate(Q2, 0.75f);
                    }
                    _GroundNormal = Vector3.Lerp(_GroundNormal, Normal, lerp);
                }
                if (!Jumping && _Grounded && Vector3.Angle(_GroundNormal, Velocity) > _VelocityGroundAngle && _PlayerState == PlayerState.Normal) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; }
                else if (_PlayerState == PlayerState.Normal) { _Grounded = false; }//|| Vector3.Angle(_GroundNormal, Velocity) > 90
            }
            else
            {
                Velocity = Velocity + (_Gravity * Time.fixedDeltaTime);
                _Grounded = false;
                RB.isKinematic = false;
                _PlayerState = PlayerState.Normal;
                Vector3 F1;
                if (GroundVelocity == Vector3.zero)
                {
                    F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                }
                else
                {
                    F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, GroundVelocity));
                }


                Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                Rotate(Q1, 0f);
                _GroundNormal = Vector3.up;
            }

            if (!_Grounded)
            {
                _PlayerState = PlayerState.Normal;
            }

            if (_Grounded)
            {
                //Rotate towards the forward of movement
                if (GroundVelocity != Vector3.zero)
                {
                    Quaternion Q2 = Quaternion.LookRotation(GroundVelocity, _GroundNormal);
                    Rotate(Q2, 0f);
                }
                if (!Jumping && _PlayerState == PlayerState.Normal)
                {
                    //Process Animation
                    if (GroundVelocity == Vector3.zero)
                    {
                        _AnimationBody.GetComponent<Animator>().Play("IdleLoop");
                    }
                    else
                    {
                        _AnimationBody.GetComponent<Animator>().Play("Boost");
                    }
                }
            }

            if (Velocity.y < -20 && !_Grounded)
            {
                Vector3 V = Velocity;
                V.y = -20;
                Velocity = V;
            }
            if (_PlayerState != PlayerState.WallSlide)
            {
                Debug.DrawRay(transform.position, Vector3.Cross(_GroundNormal, Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)).normalized, Color.red, 20);
                Vector3 GV = GroundVelocity;
                if (_RawInput == Vector3.zero)
                {
                    if (_Grounded)
                    {
                        float Mag = GV.magnitude;
                        Mag = Mathf.Clamp(Mag - (Decel * Time.fixedDeltaTime), 0, MaximumSpeed);
                        if (Mag < 0.05) Mag = 0;
                        GV = GV.normalized * Mag;

                    }


                }
                else
                {
                    Vector3 Input = _RawInput;
                    if (_Grounded)
                    {

                        Input = Quaternion.FromToRotation(Vector3.forward, Vector3.Cross(_GroundNormal, -Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward))) * Input;
                        Debug.Log(Input);
                        if (GV.magnitude < ControlSpeed)
                        {
                            float Mag = GV.magnitude;
                            GV = GV + (Input.normalized * Acell * Time.fixedDeltaTime);
                            if (GV.magnitude > ControlSpeed)
                            {
                                GV = GV.normalized * ControlSpeed;
                            }
                            Mag = Mathf.Clamp(GV.magnitude, Mag, MaximumSpeed);
                            GV = Vector3.Lerp(GV.normalized, Input.normalized, Time.fixedDeltaTime * TurningSpeed) * Mag;
                        }
                        else
                        {
                            float Mag = GV.magnitude;

                            GV = Vector3.Lerp(GV.normalized, Input.normalized, Time.fixedDeltaTime * TurningSpeed) * Mag;
                        }
                        if (GV.magnitude > MaximumSpeed)
                        {
                            GV = GV.normalized * MaximumSpeed;
                        }
                    }
                    else
                    {
                        Input = Quaternion.FromToRotation(Vector3.right, Vector3.Cross(_GroundNormal, _CameraTransformDuplicate.Forward)) * Input;
                        if (GV.magnitude < ControlSpeed)
                        {
                            GV = GV + (Input.normalized * AirAcell * Time.fixedDeltaTime);
                            if (GV.magnitude > ControlSpeed)
                            {
                                GV = GV.normalized * ControlSpeed;
                            }
                        }
                        if (GV.magnitude > MaximumSpeed)
                        {
                            GV = GV.normalized * MaximumSpeed;
                        }
                    }
                }

                GroundVelocity = GV;
            }
            float HillToSpeedCurve = HillToSpeed.Evaluate(GroundVelocity.magnitude / MaximumSpeed);
            if (_PlayerState != PlayerState.WallSlide)
            {
                if (transform.forward.y > 0.2f)
                {
                    float Z = Mathf.Clamp(LocalGroundVelocity.z - (HillToSpeedCurve * Time.fixedDeltaTime), 0, Mathf.Infinity);
                    LocalGroundVelocity = new Vector3(LocalGroundVelocity.x, LocalGroundVelocity.y, Z);
                }
                else if (transform.forward.y < -0.2f)
                {
                    float Z = LocalGroundVelocity.z + (HillToSpeedCurve * Time.fixedDeltaTime);
                    LocalGroundVelocity = new Vector3(LocalGroundVelocity.x, LocalGroundVelocity.y, Z);
                }
            }



            Vector3 NextPoint = transform.position + Velocity * Time.fixedDeltaTime;



            if (_Grounded)
            {
                if (Physics.Raycast(NextPoint, -transform.up, out _Hit, Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed)))
                {
                    Velocity += DownForce(_Hit) * Time.fixedDeltaTime;
                }
            }

            if (StartState != _PlayerState)
            {
                if (_PlayerState == PlayerState.WallSlide)
                {
                    _AnimationBody.GetComponent<Animator>().Play("WallStickStart");
                }
            }
        }
        else if (_PlayerState == PlayerState.Homing)
        {
            
        }
        else if (_PlayerState == PlayerState.GroundPound)
        {
            float RayRange = GroundRaycastLengthMin;

            Vector3 RayDirection = Vector3.zero;
            RayDirection = -transform.up;

            if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, RayRange))
            {
                _PlayerState = PlayerState.Normal;
                _Grounded = true;
            }
        }
        else if (_PlayerState == PlayerState.RailGrinding)
        {

        }
        else if (_PlayerState == PlayerState.WallSlide)
        {

        }
    }
    
    void Rotate(Quaternion Rotation, float PreserveVAmount)
    {
        Vector3 LV = LocalVelocity;

        transform.rotation = Quaternion.Lerp(transform.rotation, Rotation, 20 * Time.fixedDeltaTime);
        
        LocalVelocity = Vector3.Lerp(LocalVelocity, LV, PreserveVAmount);
    }
    
    Vector3 DownForce(RaycastHit _Hit)
    {
        Vector3 DF = (_Hit.distance * -_Hit.normal * 1.1f);

        DF = Mathf.Clamp(DF.magnitude, 1, Velocity.magnitude) * DF.normalized;

        return DF;
    }

    private void LateUpdate()
    {
        _AnimationBody.transform.position = transform.position;
        _AnimationBody.transform.rotation = Quaternion.Lerp(_AnimationBody.transform.rotation, transform.rotation * Quaternion.Euler(90, 0, 0), 0.9f);
    }
}




//Credits

//Thank you Murasaki for all the advice

//Input code is heavily bassed on the Hedgephysics engine's input code