using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.ootii.Input;
public class PlayerControler : MonoBehaviour {

    public GameObject _AnimationBody;


    

    
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

    public float ControlSpeed = 20;
    public float MaximumSpeed = 40;
    public float Acell = 0.6f;
    public float AirAcell = 1.2f;
    public float Decel = 0.4f;
    public float TurningSpeed = 2.5f;

    public bool UtopiaTurning = false;
    public bool _Grounded;

    bool Jumping = false;
    Rigidbody RB;

    public AnimationCurve HillToSpeed;

    public float _MaxGroundStandingAngle = 30;

    public enum PlayerState
    {
        Grounded,
        OnWall,
        InAir,
        WallRun
    }

    public PlayerState _PlayerState = PlayerState.Grounded;

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

        if (InputManager.IsJustPressed("GroundPound") && !_Grounded)
        {
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
        RaycastHit _Hit;
        if (_Grounded)
        {
            Debug.DrawRay(transform.position + transform.up * 0.5f, (-transform.up) * Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed), Color.blue, 0.001f);
            if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out _Hit, Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude/MaximumSpeed)))
            {
            	//AirVelocity = Vector3.zero;
                _Grounded = true;
                //transform.position = _Hit.point + _Hit.normal * Height;
                if (GroundVelocity == Vector3.zero)
                {
                    if (Vector3.Angle(Vector3.up, _Hit.normal) > _MaxGroundStandingAngle)
                    {
                        Debug.Log("Higher");
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);

                        Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.75f);
                        if (Vector3.Angle(Vector3.up, _Hit.normal) > _MaxGroundStandingAngle * 0.9f && GroundVelocity.magnitude < MaximumSpeed / 3)
                        {
                            Velocity += _Hit.normal * 3;
                            _Grounded = false;
                        }
                    }
                    else
                    {
                        Debug.Log("Lower"); 
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);
                        Rotate(Q2, 0.75f);
                    }
                    _GroundNormal = _Hit.normal;
                }
                else
                {
                    if (Vector3.Angle(Vector3.up, _Hit.normal) > _MaxGroundStandingAngle)
                    {
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);

                        Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.5f);
                    }
                    else
                    {
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);
                        Rotate(Q2, 0.75f);
                    }
                    _GroundNormal = _Hit.normal;
                }
                if (!Jumping && _Grounded && Vector3.Angle(_GroundNormal, Velocity) > 70) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; }
                else { _Grounded = false; }//|| Vector3.Angle(_GroundNormal, Velocity) > 90
            }
            else
            {
                Velocity = Velocity + (_Gravity * Time.fixedDeltaTime);
                _Grounded = false;
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
        }
        else
        {
            Debug.DrawRay(transform.position + transform.up * 0.5f, (-transform.up) * GroundRaycastLengthMin, Color.blue, 0.001f);
            if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out _Hit, GroundRaycastLengthMin))
            {
            	//AirVelocity = Vector3.zero;
                _Grounded = true;
                //transform.position = _Hit.point + _Hit.normal * Height;

                if (GroundVelocity == Vector3.zero)
                {
                    if (Vector3.Angle(Vector3.up, _Hit.normal ) > _MaxGroundStandingAngle) { 
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);

                        Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.75f);

                        if (Vector3.Angle(Vector3.up, _Hit.normal) > _MaxGroundStandingAngle * 0.9f && GroundVelocity.magnitude < MaximumSpeed / 3)
                        {
                            Velocity += _Hit.normal * 3;
                            _Grounded = false;
                        }
                    }
                    else
                    {
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);
                        Rotate(Q2, 0.75f);

                    }
                    _GroundNormal = _Hit.normal;
                }
                else
                {
                    if (Vector3.Angle(Vector3.up, _Hit.normal) > _MaxGroundStandingAngle)
                    {
                        Vector3 F1 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, transform.forward));
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));

                        Quaternion Q1 = Quaternion.LookRotation(F1, Vector3.up);
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);

                        Rotate(Quaternion.Lerp(Q1, Q2, GroundVelocity.magnitude * 5 / MaximumSpeed), 0.75f);
                    }
                    else
                    {
                        Vector3 F2 = Vector3.Cross(_Hit.normal, -Vector3.Cross(_Hit.normal, transform.forward));
                        Quaternion Q2 = Quaternion.LookRotation(F2, _Hit.normal);
                        Rotate(Q2, 0.75f);
                    }
                    _GroundNormal = _Hit.normal;
                }
                if (!Jumping && _Grounded && Vector3.Angle(_GroundNormal, Velocity) > 70) { RB.MovePosition(_Hit.point + _GroundNormal * Height); AirVelocity = Vector3.zero; } //|| Vector3.Angle(_GroundNormal, Velocity) > 90
                else { _Grounded = false; }
            }
            else
            {
                Velocity = Velocity + (_Gravity * Time.fixedDeltaTime);
                _Grounded = false;
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
        }

        
        if (_Grounded){
            //Rotate towards the forward of movement
            if (GroundVelocity != Vector3.zero)
            {
                Quaternion Q2 = Quaternion.LookRotation(GroundVelocity, _GroundNormal);
                Rotate(Q2, 0f);
            }
            if (!Jumping)
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
        float HillToSpeedCurve = HillToSpeed.Evaluate(GV.magnitude/MaximumSpeed);
        GroundVelocity = GV;
        if (transform.forward.y > 0.2f){
        	float Z = Mathf.Clamp(LocalGroundVelocity.z - (HillToSpeedCurve * Time.fixedDeltaTime), 0, Mathf.Infinity);
            LocalGroundVelocity = new Vector3(LocalGroundVelocity.x, LocalGroundVelocity.y, Z);
        }else if (transform.forward.y < -0.2f){
            float Z = LocalGroundVelocity.z + (HillToSpeedCurve * Time.fixedDeltaTime);
            LocalGroundVelocity = new Vector3(LocalGroundVelocity.x, LocalGroundVelocity.y, Z);
        }

        
        
        Vector3 NextPoint = transform.position + Velocity * Time.fixedDeltaTime;

        

        if (_Grounded)
        {
            if (Physics.Raycast(NextPoint, -transform.up, out _Hit, Mathf.Lerp(GroundRaycastLengthMin, GroundRaycastLengthMax, GroundVelocity.magnitude / MaximumSpeed)))
            {
               Velocity += DownForce(_Hit)*Time.fixedDeltaTime;
            }
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
    
}

public class Trans
{

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public Vector3 Up;
    public Vector3 Forward;

    public Trans(Vector3 newPosition, Quaternion newRotation, Vector3 newLocalScale)
    {
        position = newPosition;
        rotation = newRotation;
        localScale = newLocalScale;
        
    }

    public Trans()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        localScale = Vector3.one;
    }

    public Trans(Transform transform)
    {
        copyFrom(transform);
    }

    public void copyFrom(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
        Up = transform.up;
        Forward = transform.forward;
    }

    

}

public static class Tools
{

    public struct GroundedRaycastOut
    {
        public Vector3 forward;
        public Vector3 up;
        public List<Vector3> Points;

    }
    public static GroundedRaycastOut GroundedRaycast(Vector3 position, Vector3 forward, Vector3 up, float Distance, float MaximumAngle)
    {

        RaycastHit _Hit;

        GroundedRaycastOut GR = new GroundedRaycastOut();
        GR.Points = new List<Vector3>();
        GR.Points.Add(position);
        GR.up = up;
        GR.forward = forward;
        Vector3 lastPosition = position;
        while (Distance > 0)
        {

            float ThisDistance = Mathf.Clamp(0.1f, 0f, Distance);
            Distance = Distance - ThisDistance;

            if (Physics.Raycast(lastPosition, GR.forward, out _Hit, ThisDistance))
            {

                lastPosition = _Hit.point + GR.forward * -0.1f;
                GR.Points.Add(lastPosition);
                //Distance = Distance + 0.05f;

                if (Vector3.Angle(GR.up, _Hit.normal) > MaximumAngle) return GR;

                GR.forward = Quaternion.FromToRotation(GR.up, _Hit.normal) * GR.forward;
                GR.up = _Hit.normal;

            }
            else
            {
                lastPosition = lastPosition + GR.forward * ThisDistance;
                GR.Points.Add(lastPosition);

                if (Physics.Raycast(lastPosition, -GR.up, out _Hit, 1f))
                {
                    GR.forward = Quaternion.FromToRotation(GR.up, _Hit.normal) * GR.forward;
                    GR.up = _Hit.normal;

                    if (Vector3.Angle(GR.up, _Hit.normal) > MaximumAngle) return GR;
                }
                else
                {
                    if (Physics.Raycast(lastPosition, GR.forward, out _Hit, Distance))
                    {
                        Distance = Distance - Vector3.Distance(lastPosition, _Hit.point);
                        lastPosition = _Hit.point + GR.forward * -0.1f;
                        GR.Points.Add(lastPosition);
                        //Distance = Distance + 0.05f;
                        if (Vector3.Angle(GR.up, _Hit.normal) > MaximumAngle) return GR;

                        GR.forward = Quaternion.FromToRotation(GR.up, _Hit.normal) * GR.forward;
                        GR.up = _Hit.normal;
                    }
                    else
                    {
                        lastPosition = lastPosition + GR.forward * Distance;
                        Distance = 0f;
                        GR.Points.Add(lastPosition);
                    }

                }

            }

        }

        return GR;
    }
}
//Credits

//Thank you Murasaki for all the advice

//Input code is heavily bassed on the Hedgephysics engine's input code