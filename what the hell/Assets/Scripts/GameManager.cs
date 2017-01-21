using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float[] startingPlayerBaseHealth;
    float[] playerBaseHealth= new float[2];

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
    public gameState GamePhase { get { return gamePhase; } set
        {
            gamePhase = value;
            switch (value)
            {
                case gameState.mainMenu: eventHandlerManager.globalBroadcast(this, eventChannels.inGame, (int)inGameChannelEvents.gameReset, null);
                    eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openMainMenu, null);
                    eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.closeEndMenu, null);

                    break;
                case gameState.game:
                    eventHandlerManager.globalBroadcast(this, eventChannels.inGame, (int)inGameChannelEvents.gameStart, null);
                    eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.closeMainMenu, null);
                    eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openGameUI, null);
                    break;
                case gameState.endGameMenu:
                    eventHandlerManager.globalBroadcast(this, eventChannels.inGame, (int)inGameChannelEvents.gameOver, playerBaseHealth);
                    eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.closeGameUI, null);
                    eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openEndMenu, null);
                    break;
                default:
                    break;
            }
        } }
    public float laneWidth {
		get { return laneRight - laneLeft; }
	}


	void Awake() {
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.baseHitByWave, onBaseHit );
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameStart, onGameStart );
        
        waveUpdateSystem = new WaveUpdateSystem(this);
		playerUpdateSystem = new PlayerUpdateSystem(this, characters,waveUpdateSystem, refractaryCollisionPeriod,refractaryCollisionDistance );
		proceduralMesh.fieldLenght = (int)laneWidth;
        inputManager.Initialize(characters);

    }
    void Start() {
        eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openMainMenu, null);
    }
	void Update() {
        RenderWave();
        switch (gamePhase)
        {
            case gameState.mainMenu:
                inputManager.waitForStart(this);
                break;
            case gameState.game:
                inputManager.UpdateCharacterMovements();
		        playerUpdateSystem.Update(Time.time,Time.deltaTime);
                break;
            case gameState.endGameMenu:
                inputManager.waitForRestart(this);
                break;
            default:
                break;
        }

	}

    void RenderWave()
    {
        waveUpdateSystem.Update(playerUpdateSystem.players, Time.deltaTime);
        proceduralMesh.UpdateMesh(waveUpdateSystem);
    }
    void onGameStart(object o)
    {
        for (int i = 0; i < startingPlayerBaseHealth.Length; i++)
        {
            playerBaseHealth[i] = startingPlayerBaseHealth[i];
        }
    }
    void onBaseHit(object o)
    {
        if(gamePhase==gameState.game)
        { 
            WaveState w = o as WaveState;
            playerBaseHealth[(int)w.horzDir] -= w.altitude * damageFactor;
            if (playerBaseHealth[(int)w.horzDir] <= 0)
                GamePhase = gameState.endGameMenu;

        }
    }

    public enum gameState {
        mainMenu, game, endGameMenu
    }
}

