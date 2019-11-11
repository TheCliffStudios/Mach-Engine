using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathB))]
public class PathBEditor : Editor
{
    
    PathB _Path;
    private void OnEnable()
    {
        _Path = (PathB)target;
        if (_Path.Points.Count == 0)
        {
            _Path.Create();

        }

    }

    private void OnSceneGUI()
    {
        

        for (int i = 0; i < _Path.Points.Count / 3; i++)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(_Path.Points[i * 3 + 0] + _Path.transform.position, _Path.Points[i * 3 + 1] + _Path.transform.position);
            Handles.DrawLine(_Path.Points[i * 3 + 3] + _Path.transform.position, _Path.Points[i * 3 + 2] + _Path.transform.position);
            Handles.DrawBezier(_Path.Points[i * 3] + _Path.transform.position, _Path.Points[i * 3 + 3] + _Path.transform.position, _Path.Points[i * 3 + 1] + _Path.transform.position, _Path.Points[i * 3 + 2] + _Path.transform.position, Color.green, null, 2);

            Handles.color = Color.red;
            Handles.DrawLine(_Path.Points[i * 3] + _Path.Normals[i] + _Path.transform.position, _Path.Points[i * 3] + _Path.transform.position);
            Handles.DrawLine(_Path.Points[i * 3 + 3] + _Path.Normals[i + 1] + _Path.transform.position, _Path.Points[i * 3 + 3] + _Path.transform.position);
            
            //Vector3 _NewPos = Handles.FreeMoveHandle(_Path.Points[i * 3] + _Path.Normals[i] + _Path.transform.position, Quaternion.identity, 0.5f, Vector3.zero, Handles.CircleHandleCap);
            //if (_Path.Points[i * 3] + _Path.Normals[i] + _Path.transform.position != _NewPos)
            //{
                //_Path.Normals[i] = _NewPos - _Path.Points[i * 3] - _Path.transform.position;
            //}

            
        }

        if (DrawNormals)
        {
            for (int i = 0; i < _Path.EqualPoints.Count; i++)
            {
                Handles.color = Color.red;
                Handles.DrawLine(_Path.EqualPoints[i] + _Path.transform.position, _Path.EqualPoints[i] + _Path.transform.position + _Path.EqualNormals[i]);
            }
        }

        for (int i = 0; i < _Path.Points.Count; i++)
        {
            if (i % 3 == 0)
            {
                
                Handles.color = Color.white;
                Vector3 _NewPos = Handles.PositionHandle(_Path.Points[i] + _Path.transform.position, Quaternion.identity);
                Handles.FreeMoveHandle(_Path.Points[i] + _Path.transform.position, Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);
                
                //Handles.CubeHandleCap(0, _Path.Points[i], Quaternion.LookRotation(Vector3.up), 0.5f, EventType.DragPerform)
                if (_Path.Points[i] != _NewPos)
                {
                    
                    _Path.Points[i] = _NewPos - _Path.transform.position;
                }
            }
            else
            {
                Handles.color = Color.blue;
                Handles.FreeMoveHandle(_Path.Points[i] + _Path.transform.position, Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);
                //Vector3 _NewPos = Handles.FreeMoveHandle(_Path.Points[i] + _Path.transform.position, Quaternion.identity, 0.5f, Vector3.zero, Handles.CircleHandleCap);
                //if (_Path.Points[i] + _Path.transform.position != _NewPos)
                //{
                // _Path.Points[i] = _NewPos - _Path.transform.position;
                //}
            }
        }
    }

    bool DrawNormals = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //float NewPos = EditorGUILayout.FloatField("Start Position", _Path.Position);

        //if (NewPos != _Path.Position)
        //{
        //    _MovingPlatform.Position = NewPos;
        //    _MovingPlatform.transform.position = _MovingPlatform._Path.GetPoint(NewPos);
        //}

        if (GUILayout.Button("AddAncor"))
        {
            _Path.Add();
        }

        if (GUILayout.Button("SpawnEqualObjects"))
        {
            _Path.GetEqualPoints(0.1f, 0.01f);
        }

        if (GUILayout.Button("DrawNormals"))
        {
            DrawNormals = !DrawNormals;
        }

    }
}
