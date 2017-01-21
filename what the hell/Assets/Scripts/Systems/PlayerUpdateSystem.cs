using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerUpdateSystem {
	public PlayerState[] players;
	private PlayerState[] playersBeforeUpdate;

	private List<PlayerWaveCollision> existingCollisions = new List<PlayerWaveCollision>();

	GameManager gameManager;


	public PlayerUpdateSystem(GameManager gameManager) {
		this.gameManager = gameManager;

		players = new PlayerState[] {
			new PlayerState(1).setX(5),
			new PlayerState(2).setX(10)
		};
		playersBeforeUpdate = new PlayerState[players.Length];
		for (var i = 0; i < players.Length; ++i) {
			playersBeforeUpdate[i] = new PlayerState(players[i].id);
		}
		rememberPlayerStates();
	}			
	
	private void rememberPlayerStates() {
		for (var i = 0; i < players.Length; ++i) {
			playersBeforeUpdate[i].setValues(players[i]);
		}
	}									   

	public void Update(float deltaTime) {
		rememberPlayerStates();

		if (Input.GetKeyDown(KeyCode.Space)) {
			// TODO just a debug of wave rendering
			gameManager.waveUpdateSystem.CreateWave(players[0].x, WaveState.HorzDir.Right);
		}

		for (var i = 0; i < players.Length; ++i) {
			var player = players[i];
			var playerBeforeUpdate = playersBeforeUpdate[i];

			// 1. update player position based on input
			// TODO

			// 2. check player whether player collides wave or floor
			var waveHeight = gameManager.waveUpdateSystem.getWaveHeight(player.x);

			// detect player-wave
			if (player.y <= waveHeight) {
			}

			// otherwise, detect player-floor

			if ((playerBeforeUpdate.y - waveHeight) * (player.y - waveHeight) > 0) {
				// player hit the floor
				//eventHandlerManager.globalBroadcast(null, eventChannels.inGame, inGameChannelEvents.playerHitByWave)
				// TODO
			}
		}

	}


	private struct PlayerWaveCollision {
		PlayerState player;
		WaveState wave;
	}
}
