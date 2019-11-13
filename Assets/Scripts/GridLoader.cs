using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
            _Bounds = GetComponent<Collider>().bounds;
        
    }
    public List<Component> RenderObjects;
    public List<Component> SimulationObjects;

    public Bounds _Bounds;
    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (GameManagementScript._GameManagement._GridManager.IsGridPointInSimulateArea(GameManagementScript._GameManagement._PlayerObject.transform.position, GameManagementScript._GameManagement._GridManager.GetGrid(_Bounds.min), GameManagementScript._GameManagement._GridManager.GetGrid(_Bounds.max)))
        {
            
            Debug.Log("SimulateArea");
            foreach (Component Render in RenderObjects)
            {
                bool Bool = true;
                if (Render.GetType() == typeof(MeshRenderer))
                {
                    (Render as MeshRenderer).enabled = Bool;
                }
                if (Render.GetType() == typeof(MeshCollider))
                {
                    (Render as Collider).enabled = Bool;
                }
                if (Render.GetType() == typeof(MonoBehaviour))
                {
                    (Render as MonoBehaviour).enabled = Bool;
                }
            }

            foreach (Component Simulate in SimulationObjects)
            {
                bool Bool = true;
                if (Simulate.GetType() == typeof(MeshRenderer))
                {
                    (Simulate as MeshRenderer).enabled = Bool;
                }
                if (Simulate.GetType() == typeof(MeshCollider))
                {
                    (Simulate as Collider).enabled = Bool;
                }
                if (Simulate.GetType() == typeof(MonoBehaviour))
                {
                    (Simulate as MonoBehaviour).enabled = Bool;
                }
            }
        }
        else if (GameManagementScript._GameManagement._GridManager.IsGridPointInRenderArea(GameManagementScript._GameManagement._PlayerObject.transform.position, GameManagementScript._GameManagement._GridManager.GetGrid(_Bounds.min), GameManagementScript._GameManagement._GridManager.GetGrid(_Bounds.max)))
        {

            foreach (Component Render in RenderObjects)
            {
                bool Bool = true;
                if (Render.GetType() == typeof(MeshRenderer))
                {
                    (Render as MeshRenderer).enabled = Bool;
                }
                if (Render.GetType() == typeof(MeshCollider))
                {
                    (Render as Collider).enabled = Bool;
                }
                if (Render.GetType() == typeof(MonoBehaviour))
                {
                    (Render as MonoBehaviour).enabled = Bool;
                }
            }

            foreach (Component Simulate in SimulationObjects)
            {
                bool Bool = false;
                if (Simulate.GetType() == typeof(MeshRenderer))
                {
                    (Simulate as MeshRenderer).enabled = Bool;
                }
                if (Simulate.GetType() == typeof(MeshCollider))
                {
                    (Simulate as Collider).enabled = Bool;
                }
                if (Simulate.GetType() == typeof(MonoBehaviour))
                {
                    (Simulate as MonoBehaviour).enabled = Bool;
                }
            }
        }
        else
        {
            foreach (Component Render in RenderObjects)
            {
                bool Bool = false;
                if (Render.GetType() == typeof(MeshRenderer))
                {
                    (Render as MeshRenderer).enabled = Bool;
                }
                if (Render.GetType() == typeof(MeshCollider))
                {
                    (Render as Collider).enabled = Bool;
                }
                if (Render.GetType() == typeof(MonoBehaviour))
                {
                    (Render as MonoBehaviour).enabled = Bool;
                }
            }

            foreach (Component Simulate in SimulationObjects)
            {
                bool Bool = false;
                if (Simulate.GetType() == typeof(MeshRenderer))
                {
                    (Simulate as MeshRenderer).enabled = Bool;
                }
                if (Simulate.GetType() == typeof(MeshCollider))
                {
                    (Simulate as Collider).enabled = Bool;
                }
                if (Simulate.GetType() == typeof(MonoBehaviour))
                {
                    (Simulate as MonoBehaviour).enabled = Bool;
                }
            }
        }
    }
}
