using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour {
	// data
	public float laneLeft = 0;
	public float laneRight = 100;

	// updatables
	PlayerUpdateSystem playerUpdateSystem;
	public WaveUpdateSystem waveUpdateSystem;


	public float laneWidth {
		get { return laneRight - laneLeft; }
	}


	void Awake() {
		playerUpdateSystem = new PlayerUpdateSystem(this);
		waveUpdateSystem = new WaveUpdateSystem(this);


	}

	void Update() {
		playerUpdateSystem.Update(Time.deltaTime);
		waveUpdateSystem.Update(playerUpdateSystem.players, Time.deltaTime);
	}
}

