using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class ProceduralMesh : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices= new List<Vector3>();
    int[] triangles;
    public void Reset()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh= mesh;
        mesh.MarkDynamic();
    }

    public void Start()
    {
        MakeMesh();
        UpdateMesh();
    }

    void MakeMesh() {
        vertices.Add(Vector3.zero);
        vertices.Add(Vector3.up);
        vertices.Add(Vector3.right);
        triangles = new int[] { 0, 1, 2 };

    }

    void UpdateMesh() {
        
        mesh.SetVertices(vertices);
        mesh.triangles = triangles;
    }
}
