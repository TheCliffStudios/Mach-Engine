using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control2DOverrideZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _Inter = GetComponent<Interactable>();
        _Inter.OnNearEnter = OnNearTriggerEnter;
        _Inter.OnNearExit = OnNearTriggerExit;
    }

    public PathCreation.PathCreator _FrontPath;
    public PathCreation.PathCreator _MiddlePath;
    public PathCreation.PathCreator _BackPath;

    public int _Layer = 1;
    public int _DefaultLayer = 1;

    

    Interactable _Inter;

    public List<LayerChanger> _LayerChangers;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (LayerChanger _L in _LayerChangers)
        {
            _L.Update(this, GameManagementScript._GameManagement._PlayerObject);
        }
    }



    public void OnNearTriggerEnter(GameObject _Player)
    {
        _Player.GetComponent<PlayerControler>()._2DZone = this;
        ChangeLayer(_DefaultLayer);
    }

    public void OnNearTriggerExit(GameObject _Player)
    {
        if (_Player.GetComponent<PlayerControler>()._2DZone == this)
        {
            _Player.GetComponent<PlayerControler>()._2DZone = null;
        }
    }

    public PathCreation.VertexPath GetActivePath()
    {
        if (_Layer == 0)
        {
            return _BackPath.path;
        }
        else if (_Layer == 1)
        {
            return _MiddlePath.path;
        }
        else 
        {
            return _FrontPath.path;
        }
    }

    public void ChangeLayer(int Layer)
    {
        if (Layer < 0 || Layer > 2) return;
        else _Layer = Layer;
    }

}

[System.Serializable]
public class LayerChanger
{
    public int RightLayer = 0;
    public int LeftLayer = 1;

    public float _Width = 0.1f;
    public float _Height = 0.1f;

    public float _X = 0;
    public float _Y = 0;

    public bool Horizontal = true;
    public bool Grounded = false;

    public Transform _Transform;


    
    public LayerChanger(Transform transform)
    {
        _Transform = transform;
    }

    public void Update(Control2DOverrideZone _2DZone, GameObject _Player)
    {
        if (Grounded && !_Player.GetComponent<PlayerControler>()._Grounded) return;

        float _LowerXBounds = _X - (_Width/2);
        float _LowerYBounds = _Y - (_Height / 2);
        float _UpperXBounds = _X + (_Width / 2);
        float _UpperYBounds = _Y + (_Height / 2);

        Vector3 RelativePlayerPoint = _2DZone.transform.InverseTransformPoint(_Player.transform.position) - _2DZone.transform.InverseTransformPoint(_2DZone.transform.position);

        if ((RelativePlayerPoint.x > _LowerXBounds) && (RelativePlayerPoint.x < _UpperXBounds) && (RelativePlayerPoint.y > _LowerYBounds) && (RelativePlayerPoint.y < _UpperYBounds))
        {
            if (Horizontal)
            {
                if (RelativePlayerPoint.x < _X)
                {
                    _2DZone.ChangeLayer(LeftLayer);
                }
                else
                {
                    _2DZone.ChangeLayer(RightLayer);
                }
            }
            else
            {
                if (RelativePlayerPoint.y > _Y)
                {
                    _2DZone.ChangeLayer(LeftLayer);
                }
                else
                {
                    _2DZone.ChangeLayer(RightLayer);
                }
            }
        }

    }

}
