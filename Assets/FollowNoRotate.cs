using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowNoRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject Followed;

    // Update is called once per frame
    void Update()
    {
        if (Followed != null)
        {
            transform.position = Followed.transform.position;
        }
    }
}
