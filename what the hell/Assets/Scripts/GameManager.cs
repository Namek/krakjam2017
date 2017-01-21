using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour {
	// data
	public float laneLeft = 0;
	public float laneRight = 100;
    public float refractaryCollisionPeriod;
    public float refractaryCollisionDistance;

    // updatables
    PlayerUpdateSystem playerUpdateSystem;
	public WaveUpdateSystem waveUpdateSystem;

    [SerializeField]
    ProceduralMesh proceduralMesh;
    [SerializeField]
    InputManager inputManager;
    [SerializeField]
    Transform[] characters;


    public float laneWidth {
		get { return laneRight - laneLeft; }
	}


	void Awake() {
		waveUpdateSystem = new WaveUpdateSystem(this);
		playerUpdateSystem = new PlayerUpdateSystem(this, characters,waveUpdateSystem, refractaryCollisionPeriod,refractaryCollisionDistance );
		proceduralMesh.fieldLenght = (int)laneWidth;
        inputManager.Initialize(characters);

    }

	void Update() {
		waveUpdateSystem.Update(playerUpdateSystem.players, Time.deltaTime);
		proceduralMesh.UpdateMesh(waveUpdateSystem);
        inputManager.UpdateCharacterMovements();
		playerUpdateSystem.Update(Time.time,Time.deltaTime);
	}
}

