using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (_AlinePoints.Count > 0)
        {
            Vector3 _GroundNormal = Vector3.up;

            float Angle = 900;

            foreach (AlinePoint AP in _AlinePoints2)
            {
                if (Vector3.Angle(AP._Normal, Vector3.up) < Angle)
                {
                    Angle = Vector3.Angle(AP._Normal, Vector3.up);
                    _GroundNormal = AP._Normal;

                }
                
            }

            Vector3 F2 = Vector3.Cross(_GroundNormal, -Vector3.Cross(_GroundNormal, Vector3.forward));

            Quaternion Q2 = Quaternion.LookRotation(F2, _GroundNormal);

            transform.rotation = Q2;

            Debug.Log("AutoAline: Ground Normal: " + _GroundNormal);
        }
        else
        {
            Vector3 F2 = Vector3.Cross(Vector3.up, -Vector3.Cross(Vector3.up, Vector3.forward));

            Quaternion Q2 = Quaternion.LookRotation(F2, Vector3.up);

            transform.rotation = Q2;
        }


        //_AlinePoints.Clear();
    }

    private void Update()
    {
        //_AlinePoints.Clear();
        //_AlinePoints = new List<AlinePoint>();
    }

    private void LateUpdate()
    {
        _AlinePoints = _AlinePoints2;
        _AlinePoints2.Clear();
    }
    public List<AlinePoint> _AlinePoints = new List<AlinePoint>();
    List<AlinePoint> _AlinePoints2 = new List<AlinePoint>();
    public LayerMask _Ground;


    private void OnCollisionStay(Collision collision)
    {
        //if (collision.collider.gameObject.layer != 10) return;

        List<ContactPoint> contactPoints = new List<ContactPoint>();
        
        collision.GetContacts(contactPoints);

        //if (contactPoints.Count == 0) return;

        Debug.Log(contactPoints.Count + ": Contact Points");

        Vector3 MeanPos = Vector3.zero;

        Vector3 MeanNorm = Vector3.zero;

        foreach (ContactPoint CP in contactPoints)
        {
            MeanNorm += CP.normal;

            MeanPos += CP.point;

            if (CP.normal == Vector3.zero)
            {

            }
            else
            {
                _AlinePoints.Add(new AlinePoint(CP.normal, CP.point));
            }

            //_AlinePoints.Add(new AlinePoint(collision.GetContact(0).normal, collision.GetContact(0).point));
        }

        //TODO:Run a check and remove unessairy entiries (Both being 0,0,0)

        //_AlinePoints2.Add(new AlinePoint(collision.GetContact(0).normal, collision.GetContact(0).point));
        Debug.Log(collision.GetContact(0).normal + "Contact Normal");

        MeanNorm = MeanNorm / contactPoints.Count;
        MeanPos = MeanPos / contactPoints.Count;
        //if (MeanNorm == Vector3.zero) return;

        //_AlinePoints.Add(new AlinePoint(MeanNorm, MeanPos));
        
    }
    
}
[System.Serializable]
public struct AlinePoint
{
    [SerializeField]
    public Vector3 _Normal;
    [SerializeField]
    public Vector3 _Pos;

    public AlinePoint(Vector3 Normal, Vector3 Pos)
    {
        _Normal = Normal;
        _Pos = Pos;
    }
}
