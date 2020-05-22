using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GridPoint
{
    public int _X;
    public int _Y;
    public int _Z;

    public GridPoint(int X, int Y, int Z)
    {
        _X = X;
        _Y = Y;
        _Z = Z;
    }
}

public class GridManager
{
    int _SizeX;
    int _SizeY;
    int _SizeZ;

    float _Resolution = 10f;
    int _RenderArea = 10;
    int _SimulateArea = 5;

    Vector3 _Center;

    public GridManager(int X, int Y, int Z, float Res, int RenderArea, int SimulateArea, Vector3 Center)
    {
        _SizeX = X;
        _SizeY = Y;
        _SizeX = Z;

        _Resolution = Res;
        _RenderArea = RenderArea;
        _SimulateArea = SimulateArea;

        _Center = Center;
    }
    
    

    public bool IsGridPointInRenderArea(Vector3 PlayerPoint, Vector3Int StartPoint, Vector3Int EndPoint)
    {
        Vector3 _RelativePos = PlayerPoint - _Center;
        _RelativePos = _RelativePos - new Vector3(_Resolution, _Resolution, _Resolution) / 2;
        int PosX = Mathf.FloorToInt(_RelativePos.x / _Resolution) + Mathf.FloorToInt(_SizeX / 2);
        int PosY = Mathf.FloorToInt(_RelativePos.y / _Resolution) + Mathf.FloorToInt(_SizeY / 2);
        int PosZ = Mathf.FloorToInt(_RelativePos.z / _Resolution) + Mathf.FloorToInt(_SizeZ / 2);

        if (StartPoint.x - _RenderArea <= PosX && EndPoint.x + _RenderArea >= PosX && StartPoint.y - _RenderArea <= PosX && EndPoint.y + _RenderArea >= PosX && StartPoint.z - _RenderArea <= PosX && EndPoint.z + _RenderArea >= PosX) return true;

        return false;
    }

   

    public bool IsGridPointInSimulateArea(Vector3 PlayerPoint, Vector3Int StartPoint, Vector3Int EndPoint)
    {
        Vector3 _RelativePos = PlayerPoint - _Center;
        _RelativePos = _RelativePos - new Vector3(_Resolution, _Resolution, _Resolution) / 2;
        int PosX = Mathf.FloorToInt(_RelativePos.x / _Resolution) + Mathf.FloorToInt(_SizeX / 2);
        int PosY = Mathf.FloorToInt(_RelativePos.y / _Resolution) + Mathf.FloorToInt(_SizeY / 2);
        int PosZ = Mathf.FloorToInt(_RelativePos.z / _Resolution) + Mathf.FloorToInt(_SizeZ / 2);

        if (StartPoint.x - _SimulateArea <= PosX && EndPoint.x + _SimulateArea >= PosX && StartPoint.y - _SimulateArea <= PosX && EndPoint.y + _SimulateArea >= PosX && StartPoint.z - _SimulateArea <= PosX && EndPoint.z + _SimulateArea >= PosX) return true;

        return false;
    }

    public Vector3Int GetGrid(Vector3 Point)
    {
        Vector3 _RelativePos = Point - _Center;
        _RelativePos = _RelativePos - new Vector3(_Resolution, _Resolution, _Resolution) / 2;
        int PosX = Mathf.FloorToInt(_RelativePos.x / _Resolution) + Mathf.FloorToInt(_SizeX / 2);
        int PosY = Mathf.FloorToInt(_RelativePos.y / _Resolution) + Mathf.FloorToInt(_SizeY / 2);
        int PosZ = Mathf.FloorToInt(_RelativePos.z / _Resolution) + Mathf.FloorToInt(_SizeZ / 2);
        Vector3Int Out = new Vector3Int(PosX, PosY, PosZ);
        return Out;
    }
}

public class Trans
{

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public Vector3 Up;
    public Vector3 Forward;

    public Trans(Vector3 newPosition, Quaternion newRotation, Vector3 newLocalScale)
    {
        position = newPosition;
        rotation = newRotation;
        localScale = newLocalScale;

    }

    public Trans()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        localScale = Vector3.one;
    }

    public Trans(Transform transform)
    {
        copyFrom(transform);
    }

    public void copyFrom(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        localScale = transform.localScale;
        Up = transform.up;
        Forward = transform.forward;
    }



}

public static class GenericTools
{

    public struct GroundedRaycastOut
    {
        public Vector3 forward;
        public Vector3 up;
        public List<Vector3> Points;

    }
    public static GroundedRaycastOut GroundedRaycast(Vector3 position, Vector3 forward, Vector3 up, float Distance, float MaximumAngle)
    {

        RaycastHit _Hit;

        GroundedRaycastOut GR = new GroundedRaycastOut();
        GR.Points = new List<Vector3>();
        GR.Points.Add(position);
        GR.up = up;
        GR.forward = forward;
        Vector3 lastPosition = position;
        while (Distance > 0)
        {

            float ThisDistance = Mathf.Clamp(0.1f, 0f, Distance);
            Distance = Distance - ThisDistance;

            if (Physics.Raycast(lastPosition, GR.forward, out _Hit, ThisDistance))
            {

                lastPosition = _Hit.point + GR.forward * -0.1f;
                GR.Points.Add(lastPosition);
                //Distance = Distance + 0.05f;

                if (Vector3.Angle(GR.up, _Hit.normal) > MaximumAngle) return GR;

                GR.forward = Quaternion.FromToRotation(GR.up, _Hit.normal) * GR.forward;
                GR.up = _Hit.normal;

            }
            else
            {
                lastPosition = lastPosition + GR.forward * ThisDistance;
                GR.Points.Add(lastPosition);

                if (Physics.Raycast(lastPosition, -GR.up, out _Hit, 1f))
                {
                    GR.forward = Quaternion.FromToRotation(GR.up, _Hit.normal) * GR.forward;
                    GR.up = _Hit.normal;

                    if (Vector3.Angle(GR.up, _Hit.normal) > MaximumAngle) return GR;
                }
                else
                {
                    if (Physics.Raycast(lastPosition, GR.forward, out _Hit, Distance))
                    {
                        Distance = Distance - Vector3.Distance(lastPosition, _Hit.point);
                        lastPosition = _Hit.point + GR.forward * -0.1f;
                        GR.Points.Add(lastPosition);
                        //Distance = Distance + 0.05f;
                        if (Vector3.Angle(GR.up, _Hit.normal) > MaximumAngle) return GR;

                        GR.forward = Quaternion.FromToRotation(GR.up, _Hit.normal) * GR.forward;
                        GR.up = _Hit.normal;
                    }
                    else
                    {
                        lastPosition = lastPosition + GR.forward * Distance;
                        Distance = 0f;
                        GR.Points.Add(lastPosition);
                    }

                }

            }

        }

        return GR;
    }
}
