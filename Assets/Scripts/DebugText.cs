using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour
{

    public static DebugText _DebugText;
    TMPro.TextMeshProUGUI _Text;

    public Vector3 _Velocity;
    public Vector3 _NormalVelocity;
    public Vector3 _TangentVelocity;
    // Start is called before the first frame update
    void Start()
    {
        if (DebugText._DebugText == null)
        {
            DebugText._DebugText = this;
        }
        else
        {
            Destroy(this);
        }

        _Text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _Text.text = "Velocity: " + Mathf.FloorToInt(_Velocity.magnitude) + "\n" + "Normal Velocity: " + _NormalVelocity.magnitude + "\nTangent Velocity: " + _TangentVelocity.magnitude;
    }
}
