using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _DamageObject = _PC._DamageObject;
    }

    public GameObject _AnimationRoot;
    public GameObject _SonicBody;
    public GameObject _BallBody;
    public GameObject _SkidEmitter;
    public PlayerControler _PC;
    public LayerMask _Ground;
    DamageObject _DamageObject;

    private SAnimationEvent ThisEvent;
    private float WaitTime;

    public enum SAnimationEvent
    {
        Jumping,
        Groundpounding,
        Landing,
        HomingAttack,
        AttackHit,
        SpinDash,
        Normal,
        Kick,
        Null
    }

    private bool WasGrounded = false;
    private bool WasBall = false;
    private PlayerControler.PlayerState LastState = PlayerControler.PlayerState.Normal;

    private void Update()
    {
        ThisEvent = SAnimationEvent.Null;
    }
    float BoostAudioDelay = 0;
    float SpinAudioDelay = 0;
    private void LateUpdate()
    {

        if (_PC._Ball)
        {
            if (!WasBall)
            {
                SpinAudioDelay = 0;
                _PC._SonicAudio.Play("Spin");
            }
            SpinAudioDelay += Time.deltaTime;
            if (SpinAudioDelay > 0.5f)
            {
                if (_PC._Grounded)
                {
                    _PC._SonicAudio.Play("SpinDash");
                }
                else
                {
                    _PC._SonicAudio.Play("Spin");
                }
                SpinAudioDelay = 0;
            }
        }
        _AnimationRoot.transform.position = transform.position;
        _AnimationRoot.transform.rotation = transform.rotation;
        if (_PC.Velocity.magnitude > (_PC.MaximumSpeed * 0.75))
        {
            if (!_PC.gameObject.GetComponentInChildren<TrailRenderer>().emitting)
            {
                _PC._SonicAudio.Play("Boost1");
                BoostAudioDelay = 0;
            }
            BoostAudioDelay += Time.deltaTime;
            if (BoostAudioDelay > 1.5f)
            {
                _PC._SonicAudio.Play("Boost1_Loop");
                BoostAudioDelay = 0;
            }
            _PC.gameObject.GetComponentInChildren<TrailRenderer>().emitting = true;

        }
        else
        {
            _PC.gameObject.GetComponentInChildren<TrailRenderer>().emitting = false;
        }
        if (_BallBody.activeSelf)
        {
            _BallBody.transform.RotateAround(_BallBody.transform.position + 0.3025f * _BallBody.transform.up, _BallBody.transform.right, 2000 * Time.deltaTime);
        }

        if (_PC._PlayerState == PlayerControler.PlayerState.WallSlide)
        {
            _PC._AttackLevel = DamageObject.AttackLevel.L1;
            _PC._DefenseLevel = DamageObject.DefenseLevel.L1;
            _SonicBody.transform.localRotation = Quaternion.Euler(90, 180, 0);
            Debug.Log("WallSlideAni");
            if (LastState == PlayerControler.PlayerState.Normal )
            {
                Debug.Log("PlayingWallSlide");
                SetSpeed(1);
                Play("WallStick_Start");
            }
        }
        else
        {
            _SonicBody.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }

        if (ThisEvent == SAnimationEvent.Null && WaitTime <= 0)
        {
            
            if (!WasGrounded && _PC._Grounded)
            {

                if (LastState == PlayerControler.PlayerState.GroundPound)
                {
                    _PC._AttackLevel = DamageObject.AttackLevel.L1;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L1;
                    SetSpeed(1);
                    Play("StompLand");
                    WaitTime = 30;
                }
            }
            else
            {
                if (_PC._PlayerState == PlayerControler.PlayerState.Normal)
                {
                    _SkidEmitter.SetActive(false);
                    _PC._AttackLevel = DamageObject.AttackLevel.L1;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L1;
                    SetSpeed(1);
                    _SonicBody.SetActive(true);
                    _BallBody.SetActive(false);
                    if (_PC.Velocity.magnitude < 0.1f && _PC._Grounded && !WasBall && _PC._Ball)
                    {
                        if (Vector3.Angle(Vector3.up, _PC._GroundNormal) > 30)
                        {
                            Play("Ball_Loop");
                        }
                        else
                        {
                            Play("StandToSquat");
                            WaitTime = 30;
                        }
                        
                    }
                    else
                    {
                        if (_PC._Grounded)
                        {
                            if (_PC._Ball)
                            {
                                if (_PC.Velocity.magnitude > 0.1f) Play("Ball_Loop");
                            }
                            else if (_PC.Velocity.magnitude < 0.1f)
                            {
                                SetSpeed(1);
                                Play("Idle_Loop");
                            }
                            else if (_PC.Velocity.magnitude < (_PC.MaximumSpeed * (0.3f)))
                            {
                                SetSpeed(_PC.Velocity.magnitude / 4);
                                Play("Jog_Loop");
                            }
                            else if (_PC.Velocity.magnitude < (_PC.MaximumSpeed * (0.6f)))
                            {
                                SetSpeed(_PC.Velocity.magnitude / 4);
                                Play("Run_Loop");
                            }
                            else
                            {
                                SetSpeed(_PC.Velocity.magnitude / 3);
                                Play("Boost_Loop");
                            }
                        }
                        else
                        {
                            _PC._AttackLevel = DamageObject.AttackLevel.L2;
                            _PC._DefenseLevel = DamageObject.DefenseLevel.L2;
                            SetSpeed(1);
                            if (_PC._Ball)
                            {
                                
                                Play("Ball_Loop");
                            }
                            else
                            {
                                if (_PC.Velocity.y > 1.5f)
                                {
                                    Play("JumpUp_Loop");
                                }
                                else if (_PC.Velocity.y < -1.5f)
                                {
                                    Play("JumpDown_Loop");
                                }
                                else
                                {
                                    Play("JumpTop_Loop");
                                }
                            }
                        }
                    }

                
                }else if (_PC._PlayerState == PlayerControler.PlayerState.GroundPound)
                {
                    _SkidEmitter.SetActive(false);
                    _PC._AttackLevel = DamageObject.AttackLevel.L2;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L2;
                    SetSpeed(1);
                    Play("Stomp_Loop");

                }else if (_PC._PlayerState == PlayerControler.PlayerState.Homing)
                {
                    _SkidEmitter.SetActive(false);
                    _PC._AttackLevel = DamageObject.AttackLevel.L2;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L2;
                    SetSpeed(1);
                    if (!_PC._Ball)
                    {
                        _PC._Ball = true;
                    }
                    Play("Ball_Loop");
                }
                else if (_PC._PlayerState == PlayerControler.PlayerState.RailGrinding)
                {
                    _SkidEmitter.SetActive(false);
                    _PC._AttackLevel = DamageObject.AttackLevel.L1;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L1;
                    SetSpeed(1);
                    Play("Rail_Loop");
                }else if (_PC._PlayerState == PlayerControler.PlayerState.SpinDash)
                {
                    _SkidEmitter.SetActive(true);
                    _PC._AttackLevel = DamageObject.AttackLevel.L2;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L2;
                    SetSpeed(1);
                    if (_PC._Ball)
                    {
                        _PC._Ball = true;

                    }
                    Play("Ball_Loop");
                }
                else if (_PC._PlayerState == PlayerControler.PlayerState.WallSlide)
                {
                    _SkidEmitter.SetActive(false);
                    _PC._AttackLevel = DamageObject.AttackLevel.L1;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L1;
                    Debug.Log("WallSlideAni");
                    if (LastState == PlayerControler.PlayerState.Normal)
                    {
                        Debug.Log("PlayingWallSlide");
                        SetSpeed(1);
                        Play("WallStick_Start");
                    }
                }else if (_PC._PlayerState == PlayerControler.PlayerState.Drift)
                {
                    _SkidEmitter.SetActive(true);
                    Play("Ball_Loop");
                    _PC._AttackLevel = DamageObject.AttackLevel.L2;
                    _PC._DefenseLevel = DamageObject.DefenseLevel.L2;
                }
            }

            

        }

        WasBall = _PC._Ball;
        WasGrounded = _PC._Grounded;
        LastState = _PC._PlayerState;
        WaitTime = WaitTime - (Time.deltaTime*30);
    }

    public void Event(SAnimationEvent SEvent, PlayerControler.PlayerState _State)
    {
        if (SEvent == SAnimationEvent.Jumping)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);

            _PC._SonicAudio.Play("Jump");

            if (_State == PlayerControler.PlayerState.Normal)
            {
                //TODO: Play Jumping Animation
            }else if (_State == PlayerControler.PlayerState.RailGrinding)
            {
                //TODO: Play Jump from Rail Animation
            }
        }
        else if (SEvent == SAnimationEvent.Groundpounding)
        {
            _PC._SonicAudio.Play("Stomp_Start");
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
        else if (SEvent == SAnimationEvent.HomingAttack)
        {
            _PC._SonicAudio.Play("Homing");
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
        else if (SEvent == SAnimationEvent.AttackHit)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);

            int Num = Random.Range(1, 5);
            Play("Homing_Trick_" + Num);
            WaitTime = 20;
            _PC._PlayerState = PlayerControler.PlayerState.Normal;
        }
        else if (SEvent == SAnimationEvent.SpinDash)
        {
            _SonicBody.SetActive(false);
            _BallBody.SetActive(true);

            RaycastHit _Hit;
            _BallBody.transform.localRotation = Quaternion.identity;
            if (Physics.Raycast(transform.position, -transform.up, out _Hit, _PC.Height, _Ground))
            {
                _BallBody.transform.position = _Hit.point;
            }
        }else if (SEvent == SAnimationEvent.Normal)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }else if (SEvent == SAnimationEvent.Kick)
        {
            Play("Squat_Kick");
            SetSpeed(1);
            WaitTime = 35;
            _PC._AttackLevel = DamageObject.AttackLevel.L3;
            _PC._DefenseLevel = DamageObject.DefenseLevel.L2;
        }
    }

    void SetSpeed(float Speed)
    {
        _SonicBody.GetComponent<Animator>().speed = Speed;
    }

    void Play(string Name)
    {
        _SonicBody.GetComponent<Animator>().Play("Sonic_" + Name);
        
    }
}



