using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MovingPlatform : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        _LastPosition = transform.position;
        //_Platform = transform.GetChild(0).gameObject;
        _MaxLength = _Path.GetLength();
        Debug.Log(_MaxLength);
        _Path.GetEqualPoints(0.3f, 2f);
        Debug.Log(_Path.EqualPoints.Count);
    }
    public GameObject _PlatformTemplate;
    public GameObject _Platform;
    Vector3 _LastPosition;
    Vector3 _LastVelocity;
    public float _Speed = 1f;
    [SerializeField]
    public float Position = 0;
    public Patha _Path = new Patha();
    float _MaxLength;
    float Direction = 1;
    GameObject _Player = null;
    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 _TempPos = transform.position;
        Position += _Speed * Time.fixedDeltaTime * Direction;
        if (Position > _MaxLength || Position < 0)
        {
            Direction = Direction * -1;
            Position += _Speed * Time.fixedDeltaTime * Direction;
        }
        
        
        transform.position = _Path.GetPoint(Position);

        if (_Player != null)
        {
            _Player.GetComponent<CharacterController>().Move(transform.position - _TempPos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _Player = other.gameObject;
            Debug.Log("Enter");
        }
    }

    public void CreatePlatform()
    {
        //_Platform = Instantiate(_PlatformTemplate, gameObject.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _Player = null;
        }
    }
}
