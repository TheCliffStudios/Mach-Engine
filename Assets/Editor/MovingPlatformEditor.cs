using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    MovingPlatform _MovingPlatform;

    private void OnEnable()
    {
        _MovingPlatform = (MovingPlatform)target;
        if (_MovingPlatform._Path.Points.Count == 0)
        {
            _MovingPlatform._Path.Create(_MovingPlatform.transform.position);
            
        }
        
    }

    private void OnSceneGUI()
    {
        Patha _Path = _MovingPlatform._Path;

        for (int i = 0; i < _Path.Points.Count/3; i++) {
            Handles.color = Color.blue;
            Handles.DrawLine(_Path.Points[i * 3 + 0], _Path.Points[i * 3 + 1]);
            Handles.DrawLine(_Path.Points[i * 3 + 3], _Path.Points[i * 3 + 2]);
            Handles.DrawBezier(_Path.Points[i * 3], _Path.Points[i * 3 + 3], _Path.Points[i * 3 + 1], _Path.Points[i * 3 + 2], Color.green, null, 2);
        }

        

        for (int i = 0; i < _Path.Points.Count; i++)
        {
            Handles.color = Color.white;
            Vector3 _NewPos = Handles.PositionHandle(_Path.Points[i], Quaternion.identity);
            //Handles.CubeHandleCap(0, _Path.Points[i], Quaternion.LookRotation(Vector3.up), 0.5f, EventType.DragPerform)
            if (_Path.Points[i] != _NewPos)
            {
                //Undo.RecordObject(_MovingPlatform, "Move Point");
                _Path.Points[i] = _NewPos;
            }
        }
    }



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        float NewPos = EditorGUILayout.FloatField("Start Position", _MovingPlatform.Position);

        if (NewPos != _MovingPlatform.Position)
        {
            _MovingPlatform.Position = NewPos;
            _MovingPlatform.transform.position = _MovingPlatform._Path.GetPoint(NewPos);
        }

        if (GUILayout.Button("AddAncor"))
        {
            _MovingPlatform._Path.Add();
        }
        
        if (GUILayout.Button("SpawnEqualObjects"))
        {
            _MovingPlatform._Path.GetEqualPoints(0.3f, 2f);
        }

        

    }



}
