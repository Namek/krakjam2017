using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour {
	// updatables
	PlayerUpdateSystem playerUpdateSystem;
	public WaveUpdateSystem waveUpdateSystem;


	void Awake() {
		playerUpdateSystem = new PlayerUpdateSystem(this);
		waveUpdateSystem = new WaveUpdateSystem();
	}

	void Update() {
		playerUpdateSystem.Update(Time.deltaTime);
		waveUpdateSystem.Update(playerUpdateSystem.players, Time.deltaTime);
	}
}

