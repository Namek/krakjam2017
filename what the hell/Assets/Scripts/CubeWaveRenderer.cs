﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWaveRenderer : MonoBehaviour {
    public GameObject cube;
    public List<Transform> cubeList;
	public GameManager gameManager;

	private int cubesPerUnit = 10;

	// Use this for initialization
	void Start () {
		//int cubeCount = gameManager.laneWidth * cubesPerUnit

        for (int i = 0; i < (int)gameManager.laneWidth; i++)
        {
            Transform temp = Instantiate(cube).transform;
            temp.position = gameObject.transform.position + Vector3.right * i;

            cubeList.Add(temp);
        }
	}

    float getHeight(float x)
    {
        return gameManager.waveUpdateSystem.getWaveHeight(x);
    }

	// Update is called once per frame
	void Update () {
        foreach (var item in cubeList)
        {
            item.transform.position = new Vector3(item.transform.position.x, getHeight(item.transform.position.x), item.transform.position.z);
        }
	}
}
