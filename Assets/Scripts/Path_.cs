using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Patha
{
    [SerializeField]
    public List<Vector3> Points = new List<Vector3>();

    [SerializeField]
    public List<Vector3> EqualPoints = new List<Vector3>();

    [SerializeField]
    public float _GapSize;

    [SerializeField]
    public float Length = 0;

    public void Create(Vector3 _Origin)
    {
        Points.Add(new Vector3(0, 0, 0) + _Origin); //Ancor
        Points.Add(new Vector3(0, 0, 0.5f) + _Origin);
        Points.Add(new Vector3(0, 0, 1.5f) + _Origin); 
        Points.Add(new Vector3(0, 0, 2f) + _Origin); //Ancor
    }

    public void Add()
    {
        Vector3 NewAnchor = Points[Points.Count - 1] + new Vector3(0, 0, 2);
        Vector3 LastAncor = Points[Points.Count - 1];
        Vector3 Forward = NewAnchor - LastAncor;

        Points.Add(LastAncor + Forward * 0.33f);
        Points.Add(NewAnchor - Forward * 0.33f);
        Points.Add(NewAnchor);
    }

    public Vector3 GetPoint(float LengthOfPoint)
    {
        if (Length == 0)
        {
           Length = GetLength();
        }

        int Index = Mathf.FloorToInt(Mathf.Clamp((LengthOfPoint / Length) * (EqualPoints.Count - 1), 0, EqualPoints.Count - 1));
        float T = Mathf.Clamp((LengthOfPoint / Length) * (EqualPoints.Count - 1), 0, EqualPoints.Count - 1) - Index;
        return Vector3.Lerp(EqualPoints[Index], EqualPoints[Index + 1], T);
        
    }

    public void GetEqualPoints(float GapSize, float Resolution)
    {
        _GapSize = GapSize;
        EqualPoints.Clear();
        Vector3 LastPoint = Points[0];
        EqualPoints.Add(LastPoint);
        float Distance = 0;
        for (int Index = 0; Index < (Points.Count - 1) / 3; Index++)
        {
            
            float T = 0;
            while (T <= 1)
            {
                T += 0.1f / Resolution;
                Vector3 PointOnCurve = CubicCurve(Points[Index * 3], Points[Index * 3 + 1], Points[Index * 3 + 2], Points[Index * 3 + 3], T);
                 Distance += Vector3.Distance(LastPoint, PointOnCurve);

                while (Distance > GapSize)
                {
                    float OvershootDistance = Distance - GapSize;
                    Vector3 NewEvenPoint = PointOnCurve + (LastPoint - PointOnCurve).normalized * OvershootDistance;
                    EqualPoints.Add(NewEvenPoint);
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
