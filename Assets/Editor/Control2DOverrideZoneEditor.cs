using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(Control2DOverrideZone))]
public class Control2DOverrideZoneEditor : Editor
{
    //Control2DOverrideZone _2DZone;

    public void OnSceneGUI()
    {

        //Transform _Transform = Selection.gameObjects.Select(go => go.transform).ToArray()[0];
        
        //base.OnInspectorGUI();
        Handles.SphereHandleCap(0, Vector3.zero, Quaternion.identity, 10, EventType.Ignore);
        Control2DOverrideZone _2DZone = (Control2DOverrideZone)target; //_Transform.gameObject.GetComponent<Control2DOverrideZone>()

        //Debug.Log("Number of Layer Changers: " + _LayerChangers.Count());
        if (_2DZone == null) return;

        foreach (LayerChanger _LC in _2DZone._LayerChangers)
        {
            Vector3 WorldRightUpper = _2DZone.transform.TransformPoint(new Vector3(_LC._X + (_LC._Width / 2), _LC._Y + (_LC._Height / 2), 0)) ;
            Vector3 WorldRightLower = _2DZone.transform.TransformPoint(new Vector3(_LC._X + (_LC._Width / 2), _LC._Y - (_LC._Height / 2), 0)) ;
            Vector3 WorldLeftLower = _2DZone.transform.TransformPoint(new Vector3(_LC._X - (_LC._Width / 2), _LC._Y - (_LC._Height / 2), 0)) ;
            Vector3 WorldLeftUpper = _2DZone.transform.TransformPoint(new Vector3(_LC._X - (_LC._Width / 2), _LC._Y + (_LC._Height / 2), 0));// + _2DZone.transform.position;
            Vector3 WorldMidUpper = _2DZone.transform.TransformPoint(new Vector3(_LC._X, _LC._Y + (_LC._Height / 2), 0));
            Vector3 WorldMidLower = _2DZone.transform.TransformPoint(new Vector3(_LC._X, _LC._Y - (_LC._Height / 2), 0));
            Vector3 WorldRightMid = _2DZone.transform.TransformPoint(new Vector3(_LC._X + (_LC._Width / 2), _LC._Y, 0));
            Vector3 WorldLeftMid = _2DZone.transform.TransformPoint(new Vector3(_LC._X - (_LC._Width / 2), _LC._Y, 0));
            Handles.color = Color.blue;

            if (_LC.Horizontal)
            {
                SetColor(_LC.RightLayer);
                Handles.DrawLine(WorldRightUpper, WorldRightLower);
                Handles.DrawLine(WorldMidLower, WorldMidUpper);
                Handles.DrawLine(WorldRightUpper, WorldMidUpper);
                Handles.DrawLine(WorldRightLower, WorldMidLower);
                SetColor(_LC.LeftLayer);
                Handles.DrawLine(WorldLeftUpper, WorldLeftLower);
                Handles.DrawLine(WorldMidLower, WorldMidUpper);
                Handles.DrawLine(WorldLeftUpper, WorldMidUpper);
                Handles.DrawLine(WorldLeftLower, WorldMidLower);
            }
            else
            {
                SetColor(_LC.RightLayer); //Now top
                Handles.DrawLine(WorldRightLower, WorldLeftLower);
                Handles.DrawLine(WorldRightLower, WorldRightMid);
                Handles.DrawLine(WorldLeftLower, WorldLeftMid);
                Handles.DrawLine(WorldRightMid, WorldLeftMid);
                SetColor(_LC.LeftLayer); //now bottom
                Handles.DrawLine(WorldRightUpper, WorldLeftUpper);
                Handles.DrawLine(WorldRightUpper, WorldRightMid);
                Handles.DrawLine(WorldLeftUpper, WorldLeftMid);
                Handles.DrawLine(WorldRightMid, WorldLeftMid);
            }

            

            


        }

        
        void SetColor(int Layer)
        {
            if (Layer == 0)
            {
                Handles.color = Color.blue;
            }else if (Layer == 1)
            {
                Handles.color = Color.green;
            }else if (Layer == 2)
            {
                Handles.color = Color.red;
            }
        }
    }
}
