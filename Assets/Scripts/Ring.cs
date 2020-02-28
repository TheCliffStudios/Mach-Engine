using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Interactable>().OnNearEnter = OnTrigger;
    }

    public int Value = 1;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTrigger(GameObject _Player)
    {
        GameManagementScript._GameManagement.LM.Ring += Value;
        Destroy(gameObject);
    }
}
