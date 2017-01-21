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
			new PlayerState(1).setHouse(HorzDir.Left).setX(gameManager.laneLeft + 1),
			new PlayerState(2).setHouse(HorzDir.Right).setX(gameManager.laneRight - 1)
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

		if (Input.GetKeyDown(KeyCode.A)) {
			gameManager.waveUpdateSystem.CreateWave(players[0].x, HorzDir.Right);
		}

		if (Input.GetKeyDown(KeyCode.D)) {
			gameManager.waveUpdateSystem.CreateWave(players[1].x, HorzDir.Left);
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
				var preferredWavePushDir = player.playerHousePosition == HorzDir.Left ? HorzDir.Right : HorzDir.Left;
				gameManager.waveUpdateSystem.PushDown(player.x, preferredWavePushDir);
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
