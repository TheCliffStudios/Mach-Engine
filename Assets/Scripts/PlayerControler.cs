using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Input;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour {

    public GameObject _AnimationBody;

    public List<GameObject> _HomingTargets;
    public bool _ValidTarget = false;
    public GameObject _HomingTarget;
    public Image _TargetImage;
    
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
    public AudioManager _SonicAudio;
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
    public LayerMask _Ground;
   
    bool Jumping = false;
    Rigidbody RB;

    public AnimationCurve HillToSpeed;

    public float _MaxGroundStandingAngle = 30;

    public Vector2 WallStickAngleRange = new Vector2(50, 130);

    public enum PlayerState
    {
        Normal,
        Ball,
        GroundPound,
        RailGrinding,
        Homing,
        WallSlide
    }

    public PlayerState _PlayerState = PlayerState.Normal;

    private void Start()
    {
        _SonicAudio.Play("FlyingBatteryBMG");
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

        float _Angle = 70;

        bool _PlayLockOnSound = false;

        if (_HomingTarget == null)
        {
            _PlayLockOnSound = true;
        }
        _HomingTarget = null;
        _ValidTarget = false;
        for (int Index = 0; Index < _HomingTargets.Count; Index++)
        {
            if (_HomingTargets[Index] == null)
            {
                _HomingTargets.RemoveAt(Index);
            }
        }
        foreach (GameObject Target in _HomingTargets)
        {
            if (_Angle > Vector3.Angle(_CameraTransformDuplicate.Forward, Target.transform.position - _CameraTransformDuplicate.position))
            {
                _Angle = Vector3.Angle(_CameraTransformDuplicate.Forward, Target.transform.position - _CameraTransformDuplicate.position);
                _ValidTarget = true;
                _HomingTarget = Target;
            }
        }

        if (_ValidTarget && _PlayLockOnSound)
        {
            _SonicAudio.Play("LockOn");
        }

        if (InputManager.IsJustPressed("Ball") && _PlayerState == PlayerState.Normal)
        {
            _PlayerState = PlayerState.Ball;
        }else if (InputManager.IsJustReleased("Ball") && _PlayerState == PlayerState.Ball)
        {
            _PlayerState = PlayerState.Normal;
        }

       

        

        if (InputManager.IsJustPressed("Jump")  &&  _Grounded)
        {
            AirVelocity = _GroundNormal * 10;
            _AnimationBody.GetComponent<Animator>().Play("Ball Loop");
            Jumping = true;
            _Grounded = false;
            if (_PlayerState == PlayerState.RailGrinding)
            {
                _PlayerState = PlayerState.Normal;
            }
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
                Velocity = (_HomingTarget.transform.position - transform.position).normalized * Mathf.Clamp(AirVelocity.magnitude, MaximumSpeed/2, MaximumSpeed);
                _AnimationBody.GetComponent<Animator>().Play("Ball Loop");
                _PlayerState = PlayerState.Homing;
                LastTarget = _HomingTarget;
            }
        }

        if (InputManager.IsJustPressed("GroundPound") && !_Grounded)
        {
            _PlayerState = PlayerState.GroundPound;
            _AnimationBody.GetComponent<Animator>().Play("Stomp");
            Jumping = false;
            Velocity = new Vector3(0, -20, 0);
        }

        if (_ValidTarget && !_Grounded)
        {
            _TargetImage.transform.position = Camera.main.WorldToScreenPoint(_HomingTarget.transform.position);
            _TargetImage.enabled = true;
        }
        else
        {
            _TargetImage.enabled = false;
        }
    }
    public GameObject _AnimationRoot;
    private void FixedUpdate()
    {
        //Velocity = Vector3.Lerp(Velocity, RB.velocity, 1 * Time.fixedDeltaTime);
        BoundsCheck();
        GeneralPhysics();
        RB.velocity = Velocity;
        _AnimationRoot.transform.position = transform.position;
        DebugText._DebugText._Velocity = Velocity;
    }

    void GeneralPhysics()
    {
        if (_PlayerState == PlayerState.Normal)
        {
            NormalPhysics();
        }
        else if (_PlayerState == PlayerState.Ball)
        {
            BallPhysics();
        }
        else if (_PlayerState == PlayerState.Homing)
        {
            HomingPhysics();
        }
        else if (_PlayerState == PlayerState.GroundPound)
        {
            GroundPoundPhysics();
        }
        else if (_PlayerState == PlayerState.RailGrinding)
        {
            RailGrindingPhysics();
        }
        else if (_PlayerState == PlayerState.WallSlide)
        {
            WallSlidePhysics();
        }
    }

    void NormalPhysics()
    {
        //PlayerState StartState = _PlayerState;
        float lerp = 0.1f;
        RaycastHit _Hit;
        Vector3 RayDirection = Vector3.zero;

        
        RayDirection = -transform.up;
        

        float RayRange = 0;

        if (_Grounded)
        {
            RayRange = Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed);
            
        }
        else
        {
            RayRange = GroundRaycastLengthMin;
        }
        Debug.DrawLine(transform.position + -RayDirection * 0.5f, transform.position + -RayDirection * 0.5f + (RayDirection) * RayRange, Color.blue, 0.01f);
        //Debug.DrawRay(transform.position + -RayDirection * 0.5f, (RayDirection) * RayRange, Color.blue, 0.01f);
        if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, out _Hit, RayRange, _Ground))
        {

            _GroundNormal = _Hit.normal;
            _Grounded = true;
            //CheckWallSlide(_Hit);
            if (_PlayerState != PlayerState.Normal) return;

            if (GroundVelocity == Vector3.zero)
            {
                if (Vector3.Angle(Vector3.up, _GroundNormal) > _MaxGroundStandingAngle)
                {
                    //Debug.Log("Higher");
                    Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                    Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                    Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.75f);
                    _Grounded = false;
                }
                else
                {
                    //Debug.Log("Lower");
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);
                    Rotate(Q2, 0.75f);
                }
                //_GroundNormal = Vector3.Lerp(_GroundNormal, Normal, lerp);
                
            }
            else
            {
                if (Vector3.Angle(Vector3.up, _GroundNormal) > _MaxGroundStandingAngle)
                {
                    Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                    Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                    Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.5f);
                    if (Velocity.magnitude < MaximumSpeed / 5) _Grounded = false;
                }
                else
                {
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);
                    Rotate(Q2, 0.75f);
                }
                //_GroundNormal = Vector3.Lerp(_GroundNormal, Normal, lerp);
            }
            if (!Jumping && Vector3.Angle(_GroundNormal, Velocity) > _VelocityGroundAngle && _Grounded) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; }
            else { _Grounded = false; }//|| Vector3.Angle(_GroundNormal, Velocity) > 90
        }
        else
        {
            
            _Grounded = false;
            RB.isKinematic = false;
           
            Vector3 F1;
            if (GroundVelocity == Vector3.zero)
            {
                F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
            }
            else
            {
                F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, GroundVelocity));
            }


            Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.Lerp(transform.up, Vector3.up, 5*Time.fixedDeltaTime));
            Rotate(Q1, 0f);
            _GroundNormal = Vector3.up;
        }

        if (!_Grounded) Velocity = Velocity + (_Gravity * Time.fixedDeltaTime);

        Debug.DrawLine(transform.position, transform.position + _GroundNormal, Color.green, 0.01f);

        if (!_Grounded)
        {
            _PlayerState = PlayerState.Normal;
        }

        //Debug.Log(GroundVelocity);

        if (_Grounded)
        {
            //Rotate towards the forward of movement
            if (GroundVelocity.magnitude > 0.1f)
            {
                Quaternion Q2 = Quaternion.LookRotation(GroundVelocity, _GroundNormal);
                Rotate(Q2, 0.5f);
            }
            else
            {
                Quaternion Q2 = Quaternion.LookRotation(transform.forward, _GroundNormal);
                Rotate(Q2, 0.5f);
            }
            if (!Jumping && _PlayerState == PlayerState.Normal)
            {
                //Process Animation
                if (GroundVelocity.magnitude < 0.1f)
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
        
            Debug.DrawRay(transform.position, Vector3.Cross(_GroundNormal, Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)).normalized, Color.red, 0.01f);
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
                Input = (Vector3.Cross(_GroundNormal, -Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)) * _RawInput.z) +  (Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward) * _RawInput.x);
                //Debug.Log(Input);
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
                Input = (Vector3.Cross(_GroundNormal, -Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)) * _RawInput.z) + (Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward) * _RawInput.x);
                if (GV.magnitude < ControlSpeed)
                {
                    GV = GV + (Input.normalized * AirAcell * Time.fixedDeltaTime);
                    if (GV.magnitude > ControlSpeed)
                    {
                        GV = GV.normalized * ControlSpeed;
                    }
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
        }

            GroundVelocity = GV;
        
        float HillToSpeedCurve = HillToSpeed.Evaluate(GroundVelocity.magnitude / MaximumSpeed);

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
        



        Vector3 NextPoint = transform.position + Velocity * Time.fixedDeltaTime;



        if (_Grounded)
        {
            if (Physics.Raycast(NextPoint, -transform.up, out _Hit, Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed), _Ground))
            {
                Velocity += DownForce(_Hit) * Time.fixedDeltaTime;
            }
        }

        
    }

    private GameObject LastTarget;

    void HomingPhysics()
    {
        
        if (LastTarget == null)
        {
            _PlayerState = PlayerState.Normal;
            NormalPhysics();
            return;
        }
        Velocity = (_HomingTarget.transform.position - transform.position).normalized * Mathf.Clamp(AirVelocity.magnitude, MaximumSpeed / 2, MaximumSpeed);
    }

    float RailSpeed = 0;

    public Rail _Rail;


    void RailGrindingPhysics()
    {
        Vector3 BeforeMove = _Rail._Path.path.GetClosestPointOnPath(transform.position);
        Vector3 AfterMove = _Rail._Path.path.GetClosestPointOnPath(transform.position + Velocity*Time.fixedDeltaTime);

        Vector3 Direction = AfterMove - BeforeMove;

        Vector3 Normal = _Rail._Path.path.GetNormalAtDistance(
            _Rail._Path.path.GetClosestDistanceAlongPath(BeforeMove),
            PathCreation.EndOfPathInstruction.Stop
            );
        Vector3 Forward = _Rail._Path.path.GetDirectionAtDistance(
            _Rail._Path.path.GetClosestDistanceAlongPath(BeforeMove),
            PathCreation.EndOfPathInstruction.Stop
            );
        Velocity = Direction.normalized * Velocity.magnitude;

        if (Velocity.y < -0.1f)
        {
            Velocity = Velocity.normalized * (Mathf.Clamp(Velocity.magnitude + 0.1f, 0.5f, MaximumSpeed));
        }
        else if (Velocity.y > 0.1f)
        {
            Velocity = Velocity.normalized * (Mathf.Clamp(Velocity.magnitude - 0.1f, 0.5f, MaximumSpeed));
        }

        if (Vector3.Angle(Velocity, Forward) > 90)
        {
            transform.rotation = Quaternion.LookRotation(Velocity, -Vector3.Cross(Velocity, Normal));

        }
        else
        {
            transform.rotation = Quaternion.LookRotation(Velocity, Vector3.Cross(Velocity, Normal));
        }

        
        
        

        

        

        _AnimationBody.GetComponent<Animator>().Play("RailLoop");
        transform.position = BeforeMove + transform.up * Height;

        if (AfterMove == _Rail._Path.path.vertices[0] || AfterMove == _Rail._Path.path.vertices[_Rail._Path.path.vertices.Length-1])
        {
            _PlayerState = PlayerState.Normal;

        }
    }

    void BallPhysics()
    {
        //PlayerState StartState = _PlayerState;
        float lerp = 0.1f;
        RaycastHit _Hit;
        Vector3 RayDirection = Vector3.zero;


        RayDirection = -transform.up;


        float RayRange = 0;

        if (_Grounded)
        {
            RayRange = Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed);

        }
        else
        {
            RayRange = GroundRaycastLengthMin;
        }
        Debug.DrawLine(transform.position + -RayDirection * 0.5f, transform.position + -RayDirection * 0.5f + (RayDirection) * RayRange, Color.blue, 0.01f);
        //Debug.DrawRay(transform.position + -RayDirection * 0.5f, (RayDirection) * RayRange, Color.blue, 0.01f);
        if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, out _Hit, RayRange, _Ground))
        {

            _GroundNormal = _Hit.normal;
            _Grounded = true;
            //CheckWallSlide(_Hit);
            if (_PlayerState != PlayerState.Ball) return;

            if (GroundVelocity == Vector3.zero)
            {
                if (Vector3.Angle(Vector3.up, _GroundNormal) > _MaxGroundStandingAngle)
                {
                    //Debug.Log("Higher");
                    Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                    Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                    Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.75f);
                    _Grounded = false;
                }
                else
                {
                    //Debug.Log("Lower");
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);
                    Rotate(Q2, 0.75f);
                }
                //_GroundNormal = Vector3.Lerp(_GroundNormal, Normal, lerp);

            }
            else
            {
                if (Vector3.Angle(Vector3.up, _GroundNormal) > _MaxGroundStandingAngle)
                {
                    Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                    Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                    Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.5f);
                    if (Velocity.magnitude < MaximumSpeed / 5) _Grounded = false;
                }
                else
                {
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);
                    Rotate(Q2, 0.75f);
                }
                //_GroundNormal = Vector3.Lerp(_GroundNormal, Normal, lerp);
            }
            if (!Jumping && Vector3.Angle(_GroundNormal, Velocity) > _VelocityGroundAngle && _Grounded) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; }
            else { _Grounded = false; }//|| Vector3.Angle(_GroundNormal, Velocity) > 90
        }
        else
        {

            _Grounded = false;
            RB.isKinematic = false;

            Vector3 F1;
            if (GroundVelocity == Vector3.zero)
            {
                F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
            }
            else
            {
                F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, GroundVelocity));
            }


            Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.Lerp(transform.up, Vector3.up, 5 * Time.fixedDeltaTime));
            Rotate(Q1, 0f);
            _GroundNormal = Vector3.up;
        }

        if (!_Grounded) Velocity = Velocity + (_Gravity * Time.fixedDeltaTime);

        Debug.DrawLine(transform.position, transform.position + _GroundNormal, Color.green, 0.01f);

        if (!_Grounded)
        {
            _PlayerState = PlayerState.Ball;
        }

        //Debug.Log(GroundVelocity);

        if (_Grounded)
        {
            //Rotate towards the forward of movement
            if (GroundVelocity.magnitude > 0.1f)
            {
                Quaternion Q2 = Quaternion.LookRotation(GroundVelocity, _GroundNormal);
                Rotate(Q2, 0.5f);
            }
            else
            {
                Quaternion Q2 = Quaternion.LookRotation(transform.forward, _GroundNormal);
                Rotate(Q2, 0.5f);
            }
            
        }

        if (Velocity.y < -20 && !_Grounded)
        {
            Vector3 V = Velocity;
            V.y = -20;
            Velocity = V;
        }

        Debug.DrawRay(transform.position, Vector3.Cross(_GroundNormal, Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)).normalized, Color.red, 0.01f);
        Vector3 GV = GroundVelocity;
        

        GroundVelocity = GV;

        float HillToSpeedCurve = HillToSpeed.Evaluate(GroundVelocity.magnitude / MaximumSpeed)*2;

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

        _AnimationBody.GetComponent<Animator>().Play("Ball Loop");


        Vector3 NextPoint = transform.position + Velocity * Time.fixedDeltaTime;



        if (_Grounded)
        {
            if (Physics.Raycast(NextPoint, -transform.up, out _Hit, Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed), _Ground))
            {
                Velocity += DownForce(_Hit) * Time.fixedDeltaTime;
            }
        }
    }

    void GroundPoundPhysics()
    {
        float RayRange = GroundRaycastLengthMin;

        Vector3 RayDirection = Vector3.zero;
        RayDirection = -transform.up;

        

        if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, RayRange, _Ground))
        {
            _PlayerState = PlayerState.Normal;
            _Grounded = true;
        }
    }

    void WallSlidePhysics()
    {
        NormalPhysics();
    }

    void CheckWallSlide(RaycastHit _Hit)
    {
        
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
            GeneralPhysics();
            return;
        }
        else
        {
            RB.isKinematic = false;
        }
    }

    void Rotate(Quaternion Rotation, float PreserveVAmount)
    {
        Vector3 LV = LocalVelocity;

        //RB.MoveRotation(Quaternion.Lerp(transform.rotation, Rotation, 20 * Time.fixedDeltaTime));
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
        //_AnimationBody.transform.position = transform.position;
        //_AnimationBody.transform.rotation = Quaternion.Lerp(_AnimationBody.transform.rotation, transform.rotation * Quaternion.Euler(90, 0, 0), 0.9f);
        _AnimationRoot.transform.rotation = transform.rotation;
    }

    void BoundsCheck()
    {
        Vector3 LocalVelocity = transform.InverseTransformDirection(Velocity);

        RaycastHit _Hit;

        if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out _Hit, 0.2f, _Ground)){
            LocalVelocity.x = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.forward, out _Hit, 0.2f, _Ground))
        {
            LocalVelocity.x = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.right, out _Hit, 0.2f, _Ground))
        {
            LocalVelocity.z = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.right, out _Hit, 0.2f, _Ground))
        {
            LocalVelocity.z = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.up, out _Hit, 0.2f, _Ground))
        {
            LocalVelocity.y = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            WallHit(_Hit);
        }

        
    }

    void WallHit(RaycastHit _Hit)
    {
        if (_Grounded) return;
        if (Vector3.Angle(Vector3.up, _GroundNormal) > _MaxGroundStandingAngle)
        {
            if (Velocity.magnitude > MaximumSpeed / 2)
            {
                _GroundNormal = _Hit.normal;
                //Debug.Log("Higher");
                Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0);
                _Grounded = true;
            }
            else
            {

            }
        }
        else
        {
            _GroundNormal = _Hit.normal;
            //Debug.Log("Lower");
            Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));
            Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);
            Rotate(Q2, 0.75f);
        }
    }

}




//Credits

//Thank you Murasaki for all the advice

//Input code is heavily bassed on the Hedgephysics engine's input code