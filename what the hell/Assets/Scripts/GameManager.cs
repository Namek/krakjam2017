using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public float[] playerBaseHealth;

    // data
    public float damageFactor = 1f;
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

    gameState gamePhase;
    public gameState GamePhase { get { return gamePhase; } private set {
            switch (value)
            {
                case gameState.mainMenu: eventHandlerManager.globalBroadcast(this, eventChannels.inGame, (int)inGameChannelEvents.gameReset, null);
                    break;
                case gameState.game:
                    eventHandlerManager.globalBroadcast(this, eventChannels.inGame, (int)inGameChannelEvents.gameStart, null);
                    break;
                case gameState.endGameMenu:
                    eventHandlerManager.globalBroadcast(this, eventChannels.inGame, (int)inGameChannelEvents.gameOver, null);
                    break;
                default:
                    break;
            }
            gamePhase = value;
        } }
    public float laneWidth {
		get { return laneRight - laneLeft; }
	}


	void Awake() {
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.baseHitByWave, onBaseHit );
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

    void onBaseHit(object o)
    {
        WaveState w = o as WaveState;
        playerBaseHealth[(int)w.horzDir] -= w.altitude * damageFactor;
    }

    public enum gameState {
        mainMenu, game, endGameMenu
    }
}

