using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockLava : ProceduralMesh {
	public float additionalHeight;
	public override float calculateHeight (WaveUpdateSystem waveSystem, float currentXvalue)
	{float mockx = ((float) currentXvalue) / (float)fieldLenght;
		return mockWave.Evaluate(mockx)+additionalHeight;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMesh (null);
	}
}
