using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (_Settings == null)
        {
            Settings._Settings = this;
        }
        else
        {
            Destroy(this);
        }
        
    }

    public static Settings _Settings;

    public float _CameraSensitivity = 0.5f;
    
}
