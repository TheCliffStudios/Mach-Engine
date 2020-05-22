using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Input;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour {

    //public GameObject _AnimationBody;




    public List<GameObject> _HomingTargets;
    public bool _ValidTarget = false;
    public GameObject _HomingTarget;
    public Image _TargetImage;
    
    
    

    [Header("Velocity")]
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

    

    public Vector3 _Gravity = new Vector3(0, -9.81f, 0);
    public Vector3 _GroundNormal = Vector3.up;
    public Vector3 _V;
    public AudioManager _SonicAudio;
    

    

    [Header("Input")]
    Vector3 _MoveInput = new Vector3();
    Vector3 _RawInput = new Vector3();
    public float ControlSpeed = 20;
    public float MaximumSpeed = 40;
    public float Acell = 0.6f;
    public float AirAcell = 1.2f;
    public float Decel = 0.4f;
    public float TurningSpeed = 2.5f;
    public float AirTurningSpeed = 3.5f;
    public bool UtopiaTurning = false;
    public bool _Grounded;

    [Header("Physics")]
    public LayerMask _Ground;
    public float _VelocityGroundAngle = 40f;
    public float GroundRaycastLengthMin = 0.5f;
    public float GroundRaycastLengthMax = 2f;
    public float Height = 0.25f;
    public float WallSideDistance = 0.01f;
    Rigidbody RB;
    public AnimationCurve HillToSpeed;
    bool Jumping = false;
    

    public float _MaxGroundStandingAngle = 30;
    public float _FallOffAngle = 120;
    public float _FallOffSpeed = 30;
    public float _WallSlideSpeed = 5;

    public Vector2 WallStickAngleRange = new Vector2(50, 130);

    private float SpindashPower = 0;
    public SonicAnimationManager SAnimation;
    private GameObject LastTarget;

    public enum PlayerState
    {
        Normal,
        GroundPound,
        RailGrinding,
        Homing,
        WallSlide,
        Drift,
        SpinDash
    }

    private bool PBall = false;
    public bool _Ball
    {
        get
        {
            return PBall;
        }
        set
        {
            if (value)
            {
                if (_Grounded)
                {
                    Input = false;
                }
            }
            else
            {
                Input = true;
            }

            PBall = value;
        }
    }

    public PlayerState _PlayerState = PlayerState.Normal;

    public DamageObject _DamageObject;

    private void Awake()
    {
        _DamageObject = gameObject.AddComponent<DamageObject>();
        _DamageObject._OnDamage = OnHit;
        _DamageObject._OnDeath = OnDeath;
    }

    private void Start()
    {
        _SonicAudio.Play("FlyingBatteryBMG");
        RB = GetComponent<Rigidbody>();
        GameManagementScript._GameManagement._PlayerObject = gameObject;
        _CameraTransformDuplicate = new Trans(Camera.main.transform);
        Velocity = RB.velocity;

        
    }

    public DamageObject.AttackLevel _AttackLevel = DamageObject.AttackLevel.L1;
    public DamageObject.DefenseLevel _DefenseLevel = DamageObject.DefenseLevel.L1;

    public float _SideStepLength = 5;
    float RailSpeed = 0;

    public Rail _Rail;

    Vector3 RailPoint = Vector3.zero;
    bool Input = true;

    float DriftMag = 0;
    bool CanDoubleJump = true;

    private void Update()
    {
        DebugLines(GetRayRange());
    	_CameraTransformDuplicate = new Trans(Camera.main.transform);

        //CameraControl(Time.deltaTime);

        // Get curve position

        if (InputManager.IsJustPressed("Respawn"))
        {
            OnDeath();
        }
        if (_Grounded || _PlayerState == PlayerState.WallSlide)
        {
            CanDoubleJump = true;
        }


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

        if (InputManager.IsJustPressed("LeftStep") && _PlayerState == PlayerState.Normal)
        {
            RaycastHit _Hit;
            if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.right, out _Hit, _SideStepLength, _Ground)){
                //Step onto wall
                if (Velocity.magnitude > _WallSlideSpeed)
                {
                    _GroundNormal = _Hit.normal;
                    _Grounded = true;
                    transform.position = transform.position - transform.right * (_Hit.distance - 0.5f) + transform.up * 0.5f;
                    Orientation(_Hit, 0);
                }
                else
                {
                    transform.position = transform.position - transform.right * (_Hit.distance - 0.5f);
                }

            }
            else
            {
                //SideStep
                transform.position = transform.position - transform.right * _SideStepLength;
            }
        }

        if (InputManager.IsJustPressed("RightStep") && _PlayerState == PlayerState.Normal)
        {
            RaycastHit _Hit;
            if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.right, out _Hit, _SideStepLength, _Ground))
            {
                //Step onto wall
                if (Velocity.magnitude > _WallSlideSpeed)
                {
                    _GroundNormal = _Hit.normal;
                    _Grounded = true;
                    transform.position = transform.position + transform.right * (_Hit.distance - 0.5f) + transform.up * 0.5f;
                    Orientation(_Hit, 0);
                }
                else
                {
                    transform.position = transform.position + transform.right * (_Hit.distance - 0.5f);
                }

            }
            else
            {
                //SideStep
                transform.position = transform.position + transform.right * _SideStepLength;
            }
        }

        // calculate move direction
        Vector3 moveInp = new Vector3(h, 0, v);
        //InitialInputMag = moveInp.sqrMagnitude;
        //InitialLerpedInput = Mathf.Lerp(InitialLerpedInput, InitialInputMag, Time.deltaTime);
        
        Vector3 transformedInput = Quaternion.FromToRotation(_CameraTransformDuplicate.Up, transform.up) * (_CameraTransformDuplicate.rotation * moveInp);
        transformedInput = transform.InverseTransformDirection(transformedInput);
        transformedInput.y = 0.0f;
        _RawInput = moveInp;
        moveInp = transformedInput;
        
        _MoveInput = moveInp;
        //if (Time.frameCount % 5 == 0) { Jumping = false; }

        float _Angle = 40;

        bool _PlayLockOnSound = false;

        if (PlayerState.SpinDash == _PlayerState && InputManager.IsJustPressed("Jump"))
        {
            SpindashPower += 2;
        }

        if (_Grounded && (_PlayerState == PlayerState.Normal) && Vector3.Angle(Vector3.up, _GroundNormal) < _MaxGroundStandingAngle)
        {
            if (InputManager.IsPressed("Ball") && InputManager.IsJustPressed("Jump"))
            {
                _PlayerState = PlayerState.SpinDash;
                SpindashPower = 20;
                SAnimation.Event(SonicAnimationManager.SAnimationEvent.SpinDash, _PlayerState);
            }
        }

        if (PlayerState.SpinDash == _PlayerState && InputManager.IsJustReleased("Ball"))
        {
            _PlayerState = PlayerState.Normal;
            Velocity = SpindashPower * transform.forward;
            //SAnimation.Event(SonicAnimationManager.SAnimationEvent.Normal, _PlayerState);
        }


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
            if (Target.GetComponent<RailTarget>() == null)
            {
                if (_Angle > Vector3.Angle(_CameraTransformDuplicate.Forward, Target.transform.position - _CameraTransformDuplicate.position))
                {
                    _Angle = Vector3.Angle(_CameraTransformDuplicate.Forward, Target.transform.position - _CameraTransformDuplicate.position);
                    _ValidTarget = true;
                    _HomingTarget = Target;
                    
                }
            }
            else
            {
                Rail _R = Target.GetComponentInParent<Rail>();

                Vector3 ClostestPoint = _R._Path.path.GetClosestPointOnPath(transform.position + _CameraTransformDuplicate.Forward * 5);

                if (_Angle > Vector3.Angle(_CameraTransformDuplicate.Forward, ClostestPoint - _CameraTransformDuplicate.position))
                {
                    _Angle = Vector3.Angle(_CameraTransformDuplicate.Forward, ClostestPoint - _CameraTransformDuplicate.position);
                    _ValidTarget = true;
                    _HomingTarget = Target;
                    RailPoint = ClostestPoint;
                }
            }
        }

        if (_ValidTarget && !_Grounded)
        {
            if (_PlayLockOnSound) _SonicAudio.Play("LockOn");
            if (_HomingTarget.GetComponent<RailTarget>() == null)
            {
                _TargetImage.transform.position = Camera.main.WorldToScreenPoint(_HomingTarget.transform.position);
            }
            else
            {
                _TargetImage.transform.position = Camera.main.WorldToScreenPoint(RailPoint);
            }
            _TargetImage.enabled = true;
        }
        else
        {
            _TargetImage.enabled = false;
        }

        if (InputManager.IsJustPressed("Ball") && _PlayerState == PlayerState.Normal)
        {
            _Ball = true;
        }else if (InputManager.IsJustReleased("Ball") && _PlayerState == PlayerState.Normal)
        {
            _Ball = false;
        }

        if (InputManager.IsJustPressed("Drift") && _PlayerState == PlayerState.Normal)
        {
            _Ball = true;
            _PlayerState = PlayerState.Drift;
            DriftMag = Velocity.magnitude;
        }
        else if (InputManager.IsJustReleased("Drift") && _PlayerState == PlayerState.Drift)
        {
            _Ball = false;
            _PlayerState = PlayerState.Normal;
            Velocity = DriftMag * transform.forward;
        }

        if (InputManager.IsJustPressed("Jump"))
        {
            if ((_PlayerState == PlayerState.Normal || _PlayerState == PlayerState.RailGrinding || _PlayerState == PlayerState.WallSlide) && _Grounded)
            {
                AirVelocity = _GroundNormal * 10;
                SAnimation.Event(SonicAnimationManager.SAnimationEvent.Jumping, _PlayerState);
                Jumping = true;
                _Grounded = false;
                _Ball = true;
                if (_PlayerState == PlayerState.WallSlide) Velocity.y = 10;
                
                _PlayerState = PlayerState.Normal;
                
            }else if (_PlayerState == PlayerState.Normal && !_Grounded)
            {
                if (_ValidTarget && !Jumping)
                {
                    Velocity = (_HomingTarget.transform.position - transform.position).normalized * Mathf.Clamp(AirVelocity.magnitude, MaximumSpeed / 2, MaximumSpeed);
                    SAnimation.Event(SonicAnimationManager.SAnimationEvent.HomingAttack, _PlayerState);
                    _PlayerState = PlayerState.Homing;
                    LastTarget = _HomingTarget;
                }
                else if (CanDoubleJump && !_Grounded)
                {
                    if (CanDoubleJump)
                    {
                        CanDoubleJump = false;
                        _Ball = true;
                        Velocity.y = 7.5f;
                    }
                }
            }
        }
        else if ((InputManager.PressedTime("Jump")) > 2f)
        {
            Jumping = false;
        }
        else if (InputManager.IsJustReleased("Jump"))
        {
            if (InputManager.PressedTime("Jump") < 2f && Velocity.y > 0 && Jumping) Velocity.y = 0;
            Jumping = false;
            
        }


        /*
        
        else if (InputManager.PressedTime("Jump") > 2f && _PlayerState != PlayerState.SpinDash) 
        {

            Jumping = false;

        }
        if (InputManager.IsJustReleased("Jump") && _PlayerState != PlayerState.SpinDash)
        {
            Jumping = false;
            if (InputManager.PressedTime("Jump") < 2f && Velocity.y > 0 && Jumping) Velocity.y = 0;
            
        
           
        }*/

        if (InputManager.IsJustPressed("GroundPound") && !_Grounded)
        {
            _PlayerState = PlayerState.GroundPound;
            SAnimation.Event(SonicAnimationManager.SAnimationEvent.Groundpounding, _PlayerState);
            Jumping = false;
            Velocity = new Vector3(0, -20, 0);
        }

        if (InputManager.IsJustPressed("GroundPound") && _Grounded && !_Ball)
        {
            
            SAnimation.Event(SonicAnimationManager.SAnimationEvent.Kick, _PlayerState);
            
            Velocity = new Vector3(0, -20, 0);
        }

        
    }
    //public GameObject _AnimationRoot;
    private void FixedUpdate()
    {
        Velocity = Vector3.Lerp(Velocity, RB.velocity, 10 * Time.fixedDeltaTime);
        BoundsCheck();
        GeneralPhysics();
        RB.velocity = Velocity;
        
        
    }

    public GameObject _GroundCheckCollider;

    void GeneralPhysics()
    {
        _GroundCheckCollider.transform.position = transform.position;
        //_GroundCheckCollider.transform.rotation = transform.rotation;

        if (_PlayerState == PlayerState.Normal)
        {
            NormalPhysics();
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
        else if (_PlayerState == PlayerState.Drift)
        {
            DriftPhysics();
        }
    }

    void NormalPhysics()
    {
        RaycastHit _Hit;

        AutoAline _AutoAline = _GroundCheckCollider.GetComponent<AutoAline>();

        Vector3 RayDirection = -transform.up;

        float RayRange = GetRayRange();



        if (_Ball)
        {
            Debug.Log(_AutoAline._AlinePoints.Count);
            float Angle = 900;
            foreach (AlinePoint AP in _AutoAline._AlinePoints)
            {
                if (Vector3.Angle(AP._Normal, -Vector3.up) < Angle && Vector3.Distance(AP._Pos, transform.position) <= RayRange)
                {
                    Angle = Vector3.Angle(AP._Normal, Vector3.up);
                    RayDirection = -AP._Normal;

                }
            }

            

        }
        else
        {
            
        }

        
        
        
        if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, out _Hit, RayRange, _Ground))
        {

            _GroundNormal = _Hit.normal;
            
            
            Orientation(_Hit, 1f);
            if (_PlayerState != PlayerState.Normal)
            {
                return;
            }
            _Grounded = true;
            _GroundNormal = transform.up;
            if (!Jumping && Vector3.Angle(_GroundNormal, Velocity.normalized) > _VelocityGroundAngle && _Grounded) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; }
            else if (_Ball && !Jumping && _Grounded) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; }
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


            Quaternion Q = Quaternion.LookRotation(F1, Vector3.Lerp(transform.up, Vector3.up, 5*Time.fixedDeltaTime));
            Rotate((Quaternion.Lerp(transform.rotation, Q, 30 * Time.fixedDeltaTime)), 0f);
            _GroundNormal = Vector3.up;
        }
        

        
      

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
            if (InputManager.IsPressed("Ball"))
            {
                _Ball = true;
            }
            else
            {
                _Ball = false;
            }
        }
        else
        {
            Velocity = Velocity + (_Gravity * Time.fixedDeltaTime);
            if (Velocity.y < -20)
            {
                Vector3 V = Velocity;
                V.y = -20;
                Velocity = V;
            }
        }

        


        if (Input) RunInput();

        float Mod = 1;
        if (_Ball) Mod = 1.5f;

        float HillToSpeedCurve = HillToSpeed.Evaluate(GroundVelocity.magnitude / MaximumSpeed);

        
        if (Velocity.magnitude > 0.5f || _Ball)
        {
            if (_Ball && _Grounded)
            {

                if (Vector3.Angle(Vector3.up, _GroundNormal) > 5)
                {
                    Vector3 up = Vector3.ProjectOnPlane(Vector3.up, _GroundNormal).normalized;
                    GroundVelocity = GroundVelocity - (Vector3.up * HillToSpeedCurve * Time.fixedDeltaTime * Mod);
                }
            }
            else
            {
                if (transform.forward.y > 0.1f)
                {
                    float Z = Mathf.Clamp(LocalGroundVelocity.z - (HillToSpeedCurve * Time.fixedDeltaTime * Mod), 0, Mathf.Infinity);
                    LocalGroundVelocity = new Vector3(LocalGroundVelocity.x, LocalGroundVelocity.y, Z);
                }
                else if (transform.forward.y < -0.1f)
                {
                    float Z = LocalGroundVelocity.z + (HillToSpeedCurve * Time.fixedDeltaTime * Mod);
                    LocalGroundVelocity = new Vector3(LocalGroundVelocity.x, LocalGroundVelocity.y, Z);
                }
            }
        }



        DownForce();
        
    }

    float GetRayRange()
    {
        float RayRange = 0;
        if (_Grounded)
        {
            RayRange = Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed);

        }
        else
        {
            RayRange = GroundRaycastLengthMin;
        }
        return RayRange;
    }

    void DebugLines(float RayRange)
    {
        Debug.DrawLine(transform.position + -transform.up * 0.5f, transform.position + -transform.up * 0.5f + (transform.up) * RayRange, Color.blue, 0.01f);
        Debug.DrawLine(transform.position, transform.position + _GroundNormal, Color.green, 0.01f);
        Debug.DrawRay(transform.position, Vector3.Cross(_GroundNormal, Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)).normalized, Color.red, 0.01f);
    }

    void RunInput()
    {
        float Mod = 1;
        if (_Ball) Mod = 1.5f;
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
                Input = (Vector3.Cross(_GroundNormal, -Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward)) * _RawInput.z) + (Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward) * _RawInput.x);
                //Debug.Log(Input);
                if (GV.magnitude < ControlSpeed)
                {
                    float Mag = GV.magnitude;
                    GV = GV + (Input.normalized * Acell * Time.fixedDeltaTime * Mod);
                    if (GV.magnitude > ControlSpeed)
                    {
                        GV = GV.normalized * ControlSpeed;
                    }
                    Mag = Mathf.Clamp(GV.magnitude, Mag, MaximumSpeed);
                    GV = Vector3.Lerp(GV.normalized, Input.normalized, Time.fixedDeltaTime * TurningSpeed * Mod) * Mag;
                }
                else
                {
                    float Mag = GV.magnitude;

                    GV = Vector3.Lerp(GV.normalized, Input.normalized, Time.fixedDeltaTime * TurningSpeed * Mod) * Mag;
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
                    GV = GV + (Input.normalized * AirAcell * Time.fixedDeltaTime * Mod);
                    if (GV.magnitude > ControlSpeed)
                    {
                        GV = GV.normalized * ControlSpeed;
                    }
                }
                else
                {
                    float Mag = GV.magnitude;

                    GV = Vector3.Lerp(GV.normalized, Input.normalized, Time.fixedDeltaTime * AirTurningSpeed) * Mag;
                }
                if (GV.magnitude > MaximumSpeed)
                {
                    GV = GV.normalized * MaximumSpeed;
                }
            }
        }

        GroundVelocity = GV;
    }

    void Orientation(RaycastHit _Hit, float _SpeedAdjust)
    {
        if (_Ball)
        {
            
            Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, _CameraTransformDuplicate.Forward));
            if (_Grounded)  F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

            Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

            Rotate(Q2, 1f);
            _Grounded = true;
        }
        else if (Vector3.Angle(Vector3.up, _GroundNormal) > _MaxGroundStandingAngle)
        {
            if (Vector3.Angle(Vector3.up, _GroundNormal) > _FallOffAngle)
            {
                if (Velocity.magnitude > _FallOffSpeed || _Ball)
                {
                    Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                    Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                    Rotate(Quaternion.Lerp(Q1, Q2, Velocity.magnitude / _FallOffSpeed), _SpeedAdjust);
                    _Grounded = true;
                }
                else
                {
                    Vector3 F = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Quaternion Q = Quaternion.LookRotation(F, Vector3.up);
                    Rotate((Quaternion.Lerp(transform.rotation, Q, 30 * Time.fixedDeltaTime)), _SpeedAdjust);

                }


            }
            else
            {
                if (Velocity.magnitude > _WallSlideSpeed || _Ball)
                {
                    Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                    Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                    Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                    Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                    Rotate(Quaternion.Lerp(Q1, Q2, Velocity.magnitude/_FallOffSpeed), _SpeedAdjust);
                    _Grounded = true;
                }
                else
                {
                    //TODO: Make Player SlowDown to a wallSlide Here;

                    Velocity = Velocity - Velocity * Time.fixedDeltaTime;

                    if (Velocity.magnitude > 0.1)
                    {
                        Velocity = Vector3.zero;
                        _PlayerState = PlayerState.WallSlide;
                        Vector3 F2 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, _GroundNormal));


                        Quaternion Q2 = Quaternion.LookRotation(F2, Vector3.up);
                        Rotate(Q2, _SpeedAdjust);
                        _Grounded = true;
                        return;
                    }
                    else
                    {
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                        Rotate(Quaternion.Lerp(Q1, Q2, Velocity.magnitude / _FallOffSpeed), _SpeedAdjust);
                        _Grounded = true;
                    }

                    
                }
            }
            

        }
        else
        {
            if (_Ball)
            {
                Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                Rotate(Q2, 1f);
            }
            else
            {
                Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, transform.forward));

                Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

                Rotate(Quaternion.Lerp(Q1, Q2, Velocity.magnitude / _FallOffSpeed), 1f);
            }
        }
    }
    

    void HomingPhysics()
    {
        _Ball = true;
        if (LastTarget == null)
        {
            _PlayerState = PlayerState.Normal;
            NormalPhysics();
            return;
        }
        if (LastTarget.GetComponent<RailTarget>() == null)
        {
            Velocity = (_HomingTarget.transform.position - transform.position).normalized * Mathf.Clamp(Velocity.magnitude, MaximumSpeed / 2, MaximumSpeed);
        }
        else
        {
            Velocity = (RailPoint - transform.position).normalized * Mathf.Clamp(Velocity.magnitude, MaximumSpeed / 2, MaximumSpeed);
        }
    }

    void DriftPhysics()
    {
        Velocity = Velocity - (Velocity * 0.9f) * Time.fixedDeltaTime;

        RaycastHit _Hit;

        if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out _Hit, GroundRaycastLengthMax, _Ground))
        {
            _Grounded = true;
            Vector3 F = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, Vector3.Cross(_GroundNormal, -Vector3.Cross(_CameraTransformDuplicate.Up, _CameraTransformDuplicate.Forward))));
            
            Quaternion Q = Quaternion.LookRotation(F, _GroundNormal);
            Rotate(Q, 0);
        }
        else
        {
            _Grounded = false;
            _PlayerState = PlayerState.Normal;
        }

    }

    void RailGrindingPhysics()
    {

        _Ball = false;

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
            Velocity = Velocity.normalized * (Mathf.Clamp(Velocity.magnitude + 0.1f, 5f, MaximumSpeed));
        }
        else if (Velocity.y > 0.1f)
        {
            Velocity = Velocity.normalized * (Mathf.Clamp(Velocity.magnitude - 0.1f, 5f, MaximumSpeed));
        }

        if (Vector3.Angle(Velocity, Forward) > 90)
        {
            transform.rotation = Quaternion.LookRotation(Velocity, -Vector3.Cross(Velocity, Normal));

        }
        else
        {
            transform.rotation = Quaternion.LookRotation(Velocity, Vector3.Cross(Velocity, Normal));
        }

        
        
        

        

        

        
        transform.position = BeforeMove + transform.up * Height;

        if (AfterMove == _Rail._Path.path.vertices[0] || AfterMove == _Rail._Path.path.vertices[_Rail._Path.path.vertices.Length-1])
        {
            _PlayerState = PlayerState.Normal;

        }
    }

    

    float GPTimer;

    void GroundPoundPhysics()
    {
        _Ball = false;
        float RayRange = GroundRaycastLengthMin +  Height;

        Vector3 RayDirection = Vector3.zero;
        RayDirection = -transform.up;

        RaycastHit _Hit;

        if (Physics.Raycast(transform.position + -RayDirection * 0.5f, RayDirection, out _Hit, RayRange, _Ground))
        {
            //_PlayerState = PlayerState.Normal;
            _Grounded = true;
            if (GPTimer == 30) _SonicAudio.Play("Stomp_Land");
            GPTimer = GPTimer - Time.fixedDeltaTime * 30;
            transform.position = _Hit.point + _Hit.normal * Height;
            Velocity = Vector3.zero;
            if (GPTimer < 0) _PlayerState = PlayerState.Normal;
            
        }
        else
        {
            GPTimer = 30;
            _Grounded = false;
        }
    }

    void WallSlidePhysics()
    {
        //NormalPhysics();
        _Ball = false;
        Velocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(_GroundNormal, Vector3.up);
    }
    

    void Rotate(Quaternion Rotation, float PreserveVAmount)
    {
        Vector3 LV = LocalVelocity;
        Vector3 NewHeadPos = transform.position + (Rotation * Quaternion.Inverse(transform.rotation)) * (0.5f * transform.up);

        if (Physics.Raycast(transform.position + 0.5f*transform.up, NewHeadPos - transform.position + 0.5f * transform.up, (NewHeadPos - transform.position + 0.5f * transform.up).magnitude , _Ground)){
            //Don't rotate if it would cause a clipping issue
        }
        else 
        {
            //RB.MoveRotation(Quaternion.Lerp(transform.rotation, Rotation, 50 * Time.fixedDeltaTime));

            transform.rotation = Quaternion.Lerp(transform.rotation, Rotation, 50 * Time.fixedDeltaTime);
            //RB.MoveRotation(Rotation);
            //LocalVelocity = Vector3.Lerp(LocalVelocity, LV, PreserveVAmount);
        }
        
    }
    
    void DownForce()
    {
        Vector3 NextPoint = transform.position + Velocity * Time.fixedDeltaTime;

        RaycastHit _Hit;

        if (_Grounded && Velocity.magnitude > 10)
        {
            if (Physics.Raycast(NextPoint, -transform.up, out _Hit, Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed), _Ground))
            {
                Velocity += (_Hit.distance * -_Hit.normal * 1.1f) * Time.fixedDeltaTime;
            }
        }

        //DF = Mathf.Clamp(DF.magnitude, 1, Velocity.magnitude) * DF.normalized;
        
    }

    

    void BoundsCheck()
    {
        Vector3 LocalVelocity = transform.InverseTransformDirection(Velocity);

        RaycastHit _Hit;

        if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out _Hit, 0.3f, _Ground)){
            //LocalVelocity.x = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.forward, out _Hit, 0.3f, _Ground))
        {
            //LocalVelocity.x = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.up, out _Hit, 0.3f, _Ground))
        {
            //LocalVelocity.y = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            WallHit(_Hit);
        }

        /*
         else if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.right, out _Hit, 0.3f, _Ground))
        {
            //LocalVelocity.z = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        else if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.right, out _Hit, 0.3f, _Ground))
        {
            //LocalVelocity.z = 0;
            Velocity = transform.TransformDirection(LocalVelocity);
            Debug.Log("Forward Colission Detected");
            WallHit(_Hit);
        }
        */
    }

    void WallHit(RaycastHit _Hit)
    {
        Debug.Log("CheckWallSlide");
        if (_Grounded) return;
        if (Vector3.Angle(Vector3.up, _Hit.normal) > _MaxGroundStandingAngle)
        {
            Debug.Log("Higher");
            if (Velocity.magnitude > MaximumSpeed / 5)
            {

                if (Vector3.Angle(GroundVelocity, -_Hit.normal) > 20 || _Ball)
                {
                    if (Velocity.magnitude < MaximumSpeed / 3) return;
                        Debug.Log("WallRun");
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
                    Debug.Log("WallSlide");
                    _PlayerState = PlayerState.WallSlide;

                    _GroundNormal = _Hit.normal;
                    transform.position = _Hit.point + 0.5f * _Hit.normal;
                    //Debug.Log("Higher");
                    Vector3 F2 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, _GroundNormal));

                    
                    Quaternion Q2 = Quaternion.LookRotation(F2, Vector3.up);

                    Rotate(Q2, 0);
                    _Grounded = true;
                }
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


    public GameObject PhysicsRings;
    public void OnHit()
    {
        if (GameManagementScript._GameManagement.LM.Ring > 0)
        {
            for (int RingNum = 0; RingNum < GameManagementScript._GameManagement.LM.Ring; RingNum++)
            {
                GameObject PRing = Instantiate<GameObject>(PhysicsRings);
                Rigidbody PRRB = PRing.GetComponent<Rigidbody>();

                PRing.transform.position = transform.position + (transform.rotation * new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0,1.5f), Random.Range(-1.5f, 1.5f)));
                PRRB.velocity = (PRing.transform.position - transform.position).normalized * Random.Range(1f, 3f);
            }

            GameManagementScript._GameManagement.LM.Ring = 0;
        }
        else
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        transform.position = new Vector3(0, 1, 10);
    }

}




//Credits

//Thank you Murasaki for all the advice

//Input code is heavily bassed on the Hedgephysics engine's input code