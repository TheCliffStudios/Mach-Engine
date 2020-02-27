using UnityEngine;
using System.Collections;

using com.ootii.Input;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {
 
    public Transform target;
    public PlayerControler PC;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
 
    public float distanceMin = .5f;
    public float distanceMax = 15f;
 
    private Rigidbody rigidbody;
    public LayerMask Ground;

    float x = 0.0f;
    float y = 0.0f;
 
    // Use this for initialization
    void Start () 
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
 
        rigidbody = GetComponent<Rigidbody>();
 
        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    float Timer = 0;
 
    void LateUpdate () 
    {
        if (target) 
        {



            x += (Input.GetAxis("Mouse X") + InputManager.RightStickX) * xSpeed * distance * 0.02f;
            y += (Input.GetAxis("Mouse Y") + InputManager.RightStickY) * ySpeed * 0.02f;
 
            y = ClampAngle(y, yMinLimit, yMaxLimit);

            if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
            {
                //Timer += 0.5f*Time.deltaTime;
            }
            else
            {
                Timer = 0;
            }

            Vector3 Forward = (PC.Velocity);

            if(Forward.magnitude < 0.1f)
            {
                Forward = transform.forward;
            }

            Quaternion F = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Forward, PC.gameObject.transform.up), 50f * Time.deltaTime);

            Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, PC.gameObject.transform.up) * Quaternion.Euler(y, x, 0), 10f *Time.deltaTime); //Quaternion.Lerp(transform.rotation, target.rotation, 1 * Time.deltaTime) || Quaternion.FromToRotation(Vector3.up, PC.gameObject.transform.up)
            //Quaternion rotation =  Quaternion.Euler(y, x, 0);

            F = Quaternion.Lerp(rotation, F, Timer);

            
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
            float NDistance = distance;
            RaycastHit hit;
            if (Physics.Linecast (target.position, transform.position, out hit, Ground)) 
            {
                distance -=  hit.distance;
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = F * negDistance + target.position;
 
            transform.rotation = F;
            transform.position = position;
        }
    }
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

