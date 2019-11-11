using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestRoad))]
public class TestRoadEditor : Editor
{
    TestRoad _TestRoad;

    private void OnEnable()
    {
        _TestRoad = (TestRoad)target;
        
        if (_TestRoad._Path == null)
        {
            _TestRoad._Path = _TestRoad.GetComponent<PathCreation.PathCreator>();
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Gen Road"))
        {
            _TestRoad.GenurateRoad( _TestRoad._Width, _TestRoad._Height, _TestRoad._Resolution);
        }
    }
}
