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

    private void Start()
    {
        _GameUIController.MaxSpeed = _PlayerObject.GetComponent<PlayerControler>().MaximumSpeed;
        StartLevel();
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    public GameUIController _GameUIController;
    public GameObject _PlayerObject;
    bool LevelActive = false;
    public float Timer = 0;
    public int Lives = 3;
    public LevelManager LM;

    // Update is called once per frame
    void Update()
    {
        if (LevelActive)
        {
            Timer += (Time.deltaTime * 60);
            _GameUIController.Speed = _PlayerObject.GetComponent<PlayerControler>().Velocity.magnitude;
            _GameUIController.Timer = Timer;
            _GameUIController.Lives = 3;
            _GameUIController.RingCount = LM.Ring;
        }
        
    }

    void StartLevel()
    {
        LevelActive = true;
        Timer = 0;
        LM = gameObject.AddComponent<LevelManager>();
        LM._Player = _PlayerObject;
    }

}
