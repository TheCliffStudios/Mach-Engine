using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagementScript : MonoBehaviour
{
    public static GameManagementScript _GameManagement;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (_GameManagement == null)
        {
            GameManagementScript._GameManagement = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public GameObject _PlayerObject; 

    // Update is called once per frame
    void Update()
    {
        
    }
}
