using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject _AnimationRoot;
    public GameObject _SonicBody;
    public GameObject _BallBody;
    public PlayerControler _PC;
    public LayerMask _Ground;

    public enum SAnimationEvent
    {
        Jumping,
        Groundpounding,
        Landing,
        HomingAttack,
        AttackHit,
        SpinDash,
        Normal
    }

    private void LateUpdate()
    {
        if (_BallBody.activeSelf)
        {
            _BallBody.transform.RotateAround(_BallBody.transform.position + 0.3025f * _BallBody.transform.up, _BallBody.transform.right, 2000 * Time.deltaTime);
        }
    }

    public void Event(SAnimationEvent SEvent, PlayerControler.PlayerState _State)
    {
        if (SEvent == SAnimationEvent.Jumping)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
        else if (SEvent == SAnimationEvent.Groundpounding)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
        else if (SEvent == SAnimationEvent.HomingAttack)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
        else if (SEvent == SAnimationEvent.AttackHit)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
        else if (SEvent == SAnimationEvent.SpinDash)
        {
            _SonicBody.SetActive(false);
            _BallBody.SetActive(true);

            RaycastHit _Hit;

            if (Physics.Raycast(transform.position, -transform.up, out _Hit, _PC.Height, _Ground))
            {
                _BallBody.transform.position = _Hit.point;
            }
        }else if (SEvent == SAnimationEvent.Normal)
        {
            _SonicBody.SetActive(true);
            _BallBody.SetActive(false);
        }
    }
}
