using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathB : MonoBehaviour
{
    [SerializeField]
    public List<Vector3> Points = new List<Vector3>();

    [SerializeField]
    public List<Vector3> Normals = new List<Vector3>();

    [SerializeField]
    [HideInInspector]
    public List<Vector3> EqualPoints = new List<Vector3>();

    [SerializeField]
    [HideInInspector]
    public List<Vector3> EqualNormals = new List<Vector3>();

    [SerializeField]
    public float _GapSize;

    [SerializeField]
    public float Length = 0;

    public void Create()
    {
        Points.Add(new Vector3(0, 0, 0)); //Ancor
        Points.Add(new Vector3(0, 0, 0.5f));
        Points.Add(new Vector3(0, 0, 1.5f));
        Points.Add(new Vector3(0, 0, 2f)); //Ancor
        Normals.Add(new Vector3(0, 1, 0));
        Normals.Add(new Vector3(0, 1, 0));
    }

    public void Add()
    {
        Vector3 NewAnchor = Points[Points.Count - 1] + new Vector3(0, 0, 2);
        Vector3 LastAncor = Points[Points.Count - 1];
        Vector3 Forward = NewAnchor - LastAncor;

        Points.Add(LastAncor + Forward * 0.33f);
        Points.Add(NewAnchor - Forward * 0.33f);
        Points.Add(NewAnchor);
        Normals.Add(new Vector3(0, 1, 0));
    }

    public Vector3 GetPoint(float LengthOfPoint)
    {
        if (Length == 0)
        {
            Length = GetLength();
        }
        
        int Index = Mathf.FloorToInt(Mathf.Clamp((LengthOfPoint / Length) * (EqualPoints.Count - 1), 0, EqualPoints.Count - 1));
        if (Index >= EqualPoints.Count - 1)
        {
            return EqualPoints[Index];
        }
        float T = Mathf.Clamp((LengthOfPoint / Length) * (EqualPoints.Count - 1), 0, EqualPoints.Count - 1) - Index;
        return Vector3.Lerp(EqualPoints[Index], EqualPoints[Index + 1], T);

    }

    public Vector3 GetNormal(float LengthOfNormal)
    {
        if (Length == 0)
        {
            Length = GetLength();
        }
        
        
        float Distance = 0;
        for (int Index = 0; Index < (EqualPoints.Count - 2); Index++)
        {
            Distance += Vector3.Distance(EqualPoints[Index], EqualPoints[Index + 1]);

            if (Distance > LengthOfNormal)
            {
                Distance = (Distance - LengthOfNormal)/ Vector3.Distance(EqualPoints[Index], EqualPoints[Index + 1]);
                return Vector3.Lerp(EqualNormals[Index], EqualNormals[Index + 1], Distance);
            }

        }
        Distance += Vector3.Distance(EqualPoints[EqualPoints.Count - 2], EqualPoints[EqualPoints.Count - 1]);
        if (Distance > LengthOfNormal)
        {
            Distance = (Distance - LengthOfNormal) / Vector3.Distance(EqualPoints[EqualPoints.Count - 2], EqualPoints[EqualPoints.Count - 1]);
            return Vector3.Lerp(EqualNormals[EqualPoints.Count - 2], EqualNormals[EqualPoints.Count - 1], Distance);
        }
        else
        {
            return EqualNormals[EqualPoints.Count - 1];
        }
    }

    public void GetEqualPoints(float GapSize, float Resolution)
    {
        
        _GapSize = GapSize;
        EqualPoints.Clear();
        EqualNormals.Clear();
        Vector3 LastPoint = Points[0];
        Vector3 Normal = Normals[0];
        EqualPoints.Add(LastPoint);
        EqualNormals.Add(Normal);
        float Distance = 0;
        for (int Index = 0; Index < (Points.Count - 1) / 3; Index++)
        {

            float T = 0;
            while (T <= 1)
            {
                T += Resolution;
                Vector3 PointOnCurve = CubicCurve(Points[Index * 3], Points[Index * 3 + 1], Points[Index * 3 + 2], Points[Index * 3 + 3], T);
                Distance += Vector3.Distance(LastPoint, PointOnCurve);

                while (Distance > GapSize)
                {
                    float OvershootDistance = Distance - GapSize;
                    Vector3 NewEvenPoint = PointOnCurve + (LastPoint - PointOnCurve).normalized * OvershootDistance;
                    Normal = Vector3.Lerp(Normals[Index].normalized, Normals[Index + 1].normalized, T); //(T) / (Distance *(Distance - OvershootDistance))
                    EqualPoints.Add(NewEvenPoint);
                    EqualNormals.Add(Normal);
                    Distance = OvershootDistance;
                    LastPoint = NewEvenPoint;
                }

                LastPoint = PointOnCurve;
            }
        }




    }

    public float GetLength()
    {
        Length = 0;
        Vector3 _lastPoint = Points[0];
        for (int Index = 0; Index < Points.Count / 3; Index++)
        {
            Debug.Log(Index);
            for (int i = 1; i <= 20; i++)
            {
                Debug.Log("Curve t = " + i);
                Vector3 Point = CubicCurve(Points[Index * 3], Points[Index * 3 + 1], Points[Index * 3 + 2], Points[Index * 3 + 3], i / 20f);

                Length += Vector3.Distance(Point, _lastPoint);
                _lastPoint = Point;

            }


        }

        return Length;
    }

    Segment GetSegment(int Index)
    {
        Segment _Segment = new Segment();
        _Segment.AncorA = Points[Index * 3];
        _Segment.ArmA = Points[Index * 3 + 1];
        _Segment.AncorB = Points[Index * 3 + 3];
        _Segment.ArmB = Points[Index * 3 + 2];

        _Segment.NormalA = Normals[Index];
        _Segment.NormalB = Normals[Index+1];

        return _Segment;
    }

    Vector3 QuadraticCurve(Vector3 V1, Vector3 V2, Vector3 V3, float t)
    {
        Vector3 P1 = Vector3.Lerp(V1, V2, t);
        Vector3 P2 = Vector3.Lerp(V2, V3, t);
        return Vector3.Lerp(P1, P2, t);
    }

    Vector3 CubicCurve(Vector3 V1, Vector3 V2, Vector3 V3, Vector3 V4, float t)
    {
        Vector3 P1 = QuadraticCurve(V1, V2, V3, t);
        Vector3 P2 = QuadraticCurve(V2, V3, V4, t);
        return Vector3.Lerp(P1, P2, t);
    }
}

public class Segment
{
    public Vector3 AncorA;
    public Vector3 AncorB;
    public Vector3 ArmA;
    public Vector3 ArmB;
    public Vector3 NormalA;
    public Vector3 NormalB;

    public Segment()
    {


        
    }
}