using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWaveRenderer : MonoBehaviour {
    public float areaToCover = 30f;
    public GameObject cube;
    public List<Transform> cubeList;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < (int)areaToCover; i++)
        {
            Transform temp = Instantiate(cube).transform;
            temp.position = gameObject.transform.position + Vector3.right * i;

            cubeList.Add(temp);
        }
	}

    float getHeight(float x)
    {
        return 0;
    }

	// Update is called once per frame
	void Update () {
        foreach (var item in cubeList)
        {
            item.transform.position = new Vector3(item.transform.position.x, getHeight(item.transform.position.x), item.transform.position.z);
        }
	}
}
