using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class ProceduralMesh : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices= new List<Vector3>();
    List<int> topIndexVertexList = new List<int>();
    List<int> midIndexVertexList = new List<int>();
    List<int> botIndexVertexList = new List<int>();
    int[] triangles;
    public int fieldLenght;
    public int resolution;
    float vertexHorizontalDistance;
    public float topQuadHeight;

    void Awake()
    {
        //mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh= mesh;
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.MarkDynamic();

    }

    public void Start()
    {
        MakeMesh();

    }

    void MakeMesh() {
        vertexHorizontalDistance = 1f / (float)resolution;
        triangles = new int[(fieldLenght*resolution -1)*12];
		float currentXvalue = 0;
		int triangleIndexCounter = 0;
		int uvIndexCounter = 0;
        int vertexIndexCounter = 0;
        for (int i = 0; i < fieldLenght*resolution; i++)
        {
            vertices.Add(new Vector3(currentXvalue, 0, 0));//vertexIndexCounter
            vertices.Add(new Vector3(currentXvalue, 1, 0));//vertexIndexCounter+1
			vertices.Add(new Vector3(currentXvalue, 2, 0));//vertexIndexCounter+2

            botIndexVertexList.Add(vertexIndexCounter);
            midIndexVertexList.Add(vertexIndexCounter+1);
            topIndexVertexList.Add(vertexIndexCounter+2);

            if (i > 0)
            {
                triangles[triangleIndexCounter]   = vertexIndexCounter-2; //1
                triangles[triangleIndexCounter+1] = vertexIndexCounter;//3
                triangles[triangleIndexCounter+2] = vertexIndexCounter - 3;//0

                triangles[triangleIndexCounter+3] = vertexIndexCounter-2;//1
                triangles[triangleIndexCounter+4] = vertexIndexCounter+1;//4
				triangles[triangleIndexCounter + 5] = vertexIndexCounter;//3

                triangles[triangleIndexCounter + 6] = vertexIndexCounter-1;//2
                triangles[triangleIndexCounter + 7] = vertexIndexCounter+1;//4
                triangles[triangleIndexCounter + 8] = vertexIndexCounter-2;//1

                triangles[triangleIndexCounter + 9] = vertexIndexCounter-1;//2
                triangles[triangleIndexCounter + 10] = vertexIndexCounter+2;//5
                triangles[triangleIndexCounter + 11] = vertexIndexCounter+1;//4


                triangleIndexCounter += 12;
            }
            vertexIndexCounter += 3;
            currentXvalue += vertexHorizontalDistance;
			uvIndexCounter += 3;
		}
		Vector2[] uvs = new Vector2[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			uvs[i]=new Vector2(vertices[i].x/(float)fieldLenght,vertices[i].y/2f);
		}
		mesh.SetVertices(vertices);
		mesh.uv = uvs;

    }
    public AnimationCurve mockWave;
    public virtual void UpdateMesh(WaveUpdateSystem waveSystem)
	{
        float currentXvalue = 0;
        for (int i = 0; i < fieldLenght* resolution; i++)
        {
            float mockx = ((float) currentXvalue) / (float)fieldLenght;//normally: getHeight((float) currentXvalue) instead of mockWave.Evaluate(mockx)
			float y = calculateHeight(waveSystem,currentXvalue);
            vertices[topIndexVertexList[i]] = Vector3.right * i * vertexHorizontalDistance + Vector3.up *y ;
            vertices[midIndexVertexList[i]] = Vector3.right * i * vertexHorizontalDistance + Vector3.up *Mathf.Max (0, y - topQuadHeight);
            currentXvalue += vertexHorizontalDistance;
        }

        mesh.SetVertices(vertices);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
	public virtual float calculateHeight(WaveUpdateSystem waveSystem, float currentXvalue){
		return waveSystem.getWaveHeight((float) currentXvalue);
	}
}
