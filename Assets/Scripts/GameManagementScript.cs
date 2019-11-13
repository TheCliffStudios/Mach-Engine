using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagementScript : MonoBehaviour
{
    public static GameManagementScript _GameManagement;
    public GridManager _GridManager;

    public Vector3Int GridSize;
    public Vector3 GridCenter;
    public float Resolution = 10;
    public int RenderArea = 10;
    public int SimulationArea = 10;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_GameManagement == null)
        {
            GameManagementScript._GameManagement = this;
            _GridManager = new GridManager(GridSize.x, GridSize.y, GridSize.z, Resolution, RenderArea, SimulationArea, GridCenter);
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
