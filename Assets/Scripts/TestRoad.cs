using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PathCreation.PathCreator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class TestRoad : MonoBehaviour
{
    public PathCreation.PathCreator _Path;
    public float _Width = 1;
    public float _Height = 1;
    public float _Resolution = 0.1f;

    

    

    public void GenurateRoad(float Width, float Height, float Resolution)
    {
        Mesh _Mesh = new Mesh();
        
        
        Debug.Log("EqualPoints");
        float _Length = _Path.path.length;
        Debug.Log("Length");
        List<Vector3> _Verts = new List<Vector3>();
        List<int> _Tris = new List<int>();

        Vector3 RightUpperCorner;
        Vector3 RightLowerCorner;
        Vector3 LeftUpperCorner;
        Vector3 LeftLowerCorner;
        Quaternion _Rotation = Quaternion.identity;
        int _Count;
        float _Distance = 0;

        while(_Distance < _Length)
        {
            _Rotation =  Quaternion.LookRotation(_Path.path.GetPointAtDistance(_Distance + Resolution, PathCreation.EndOfPathInstruction.Stop) - _Path.path.GetPointAtDistance(_Distance, PathCreation.EndOfPathInstruction.Stop), _Path.path.GetNormalAtDistance(_Distance)); //_Path.GetNormal(_Distance)
            RightUpperCorner = _Rotation * (Vector3.right * Width / 2 + Vector3.up * Height / 2) + _Path.path.GetPointAtDistance(_Distance) - _Path.transform.position;
            RightLowerCorner = _Rotation * (Vector3.right * Width / 2 + Vector3.up * -Height / 2) + _Path.path.GetPointAtDistance(_Distance) - _Path.transform.position;
            LeftUpperCorner = _Rotation * (Vector3.right * -Width / 2 + Vector3.up *  Height / 2) + _Path.path.GetPointAtDistance(_Distance) - _Path.transform.position;
            LeftLowerCorner = _Rotation * (Vector3.right * -Width / 2 + Vector3.up * -Height / 2) + _Path.path.GetPointAtDistance(_Distance) - _Path.transform.position;
             _Count = _Verts.Count;
            Debug.Log("Corners");
            _Verts.Add(RightUpperCorner);
            _Verts.Add(RightLowerCorner);
            _Verts.Add(LeftUpperCorner);
            _Verts.Add(LeftLowerCorner);
            Debug.Log("SetCorners");

            
            //top
            _Tris.Add(_Count); _Tris.Add(_Count + 2); _Tris.Add(_Count + 4);
            _Tris.Add(_Count + 4); _Tris.Add(_Count + 2); _Tris.Add(_Count + 6);
            //bottom
            _Tris.Add(_Count + 1); _Tris.Add(_Count + 5); _Tris.Add(_Count + 3);
            _Tris.Add(_Count + 7); _Tris.Add(_Count + 3); _Tris.Add(_Count + 5);
            //Left Side
            _Tris.Add(_Count + 2); _Tris.Add(_Count + 3); _Tris.Add(_Count + 6);
            _Tris.Add(_Count + 6); _Tris.Add(_Count + 3); _Tris.Add(_Count + 7);
            //Right side
            _Tris.Add(_Count + 1); _Tris.Add(_Count); _Tris.Add(_Count + 4);
            _Tris.Add(_Count + 1); _Tris.Add(_Count + 4); _Tris.Add(_Count + 5);
            Debug.Log("SetTris");
            _Distance += Resolution;
        }


        _Distance = _Length;
        //_Rotation = Quaternion.LookRotation(_Path.path.GetPointAtDistance(_Distance + Resolution, PathCreation.EndOfPathInstruction.Stop) - _Path.path.GetPointAtDistance(_Distance, PathCreation.EndOfPathInstruction.Stop)); //_Path.GetNormal(_Distance)
        Debug.Log("Distance = Length");
        RightUpperCorner = _Rotation * (Vector3.right * Width / 2 + Vector3.up * Height / 2) + _Path.path.GetPointAtDistance(_Distance, PathCreation.EndOfPathInstruction.Stop) - _Path.transform.position;
        RightLowerCorner = _Rotation * (Vector3.right * Width / 2 + Vector3.up * -Height / 2) + _Path.path.GetPointAtDistance(_Distance, PathCreation.EndOfPathInstruction.Stop) - _Path.transform.position;
        LeftUpperCorner = _Rotation * (Vector3.right * -Width / 2 + Vector3.up * Height / 2) + _Path.path.GetPointAtDistance(_Distance, PathCreation.EndOfPathInstruction.Stop) - _Path.transform.position;
        LeftLowerCorner = _Rotation * (Vector3.right * -Width / 2 + Vector3.up * -Height / 2) + _Path.path.GetPointAtDistance(_Distance, PathCreation.EndOfPathInstruction.Stop) - _Path.transform.position;
        Debug.Log("SetEnd");
        _Count = _Verts.Count;
        _Verts.Add(RightUpperCorner);
        _Verts.Add(RightLowerCorner);
        _Verts.Add(LeftUpperCorner);
        _Verts.Add(LeftLowerCorner);
        
        _Tris.Add(0); _Tris.Add(1); _Tris.Add(2);
        _Tris.Add(1); _Tris.Add(3); _Tris.Add(2);
        Debug.Log("Count: " + _Count);
        _Tris.Add(_Count + 0); _Tris.Add(_Count + 2); _Tris.Add(_Count + 1);
        _Tris.Add(_Count + 1); _Tris.Add(_Count + 2); _Tris.Add(_Count + 3);

        _Mesh.vertices = _Verts.ToArray();
        Debug.Log("SetVerts");
        _Mesh.triangles = _Tris.ToArray();
        Debug.Log("SetTris");
        _Mesh.RecalculateNormals();

        Debug.Log(_Mesh.triangles.Length/3);
        Debug.Log(_Mesh.vertexCount);

        _Mesh.Optimize();
        GetComponent<MeshCollider>().sharedMesh = _Mesh;
        GetComponent<MeshFilter>().mesh = _Mesh;

    }
}
