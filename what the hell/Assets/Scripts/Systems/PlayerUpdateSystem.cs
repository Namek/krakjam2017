using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerUpdateSystem {
	public PlayerState[] players;
	private PlayerState[] playersBeforeUpdate;

	private List<PlayerFluidCollision> existingCollisions = new List<PlayerFluidCollision>();
    float refractaryCollisionPeriod;
    float refractaryCollisionDistance;
	GameManager gameManager;
    WaveUpdateSystem waveSystem;

	public PlayerUpdateSystem(
        GameManager gameManager, 
        Transform[] playerTransforms, 
        WaveUpdateSystem waveSystem,
        float refractaryCollisionPeriod,
        float refractaryCollisionDistance
        ) {
		this.gameManager = gameManager;
        this.waveSystem = waveSystem;

		players = new PlayerState[] {
			new PlayerState(0).setHouse(HorzDir.Left).setTransform(playerTransforms[0]).refreshPositionData(),
			new PlayerState(1).setHouse(HorzDir.Right).setTransform(playerTransforms[1]).refreshPositionData()
        };
		playersBeforeUpdate = new PlayerState[players.Length];
		for (var i = 0; i < players.Length; ++i) {
			playersBeforeUpdate[i] = new PlayerState(players[i].id);
		}
		rememberPlayerStates();

        this.refractaryCollisionPeriod = refractaryCollisionPeriod;
        this.refractaryCollisionDistance = refractaryCollisionDistance;

    }			
	
	private void rememberPlayerStates() {
		for (var i = 0; i < players.Length; ++i) {
			playersBeforeUpdate[i].setValues(players[i]);
		}
	}									   

	public void Update(float currentTime, float deltaTime) {
        if (Input.GetKeyDown(KeyCode.A))
        {
            waveSystem.CreateWave(players[1].x, HorzDir.Right);
        }
        //var _player = players[1].refreshPositionData();
        //var _playerBeforeUpdate = playersBeforeUpdate[0];
        //var _waveHeight = waveSystem.getWaveHeight(_player.x);
        rememberPlayerStates();
        forgetOldCollisions(currentTime);


        for (var i = 0; i < players.Length; ++i) {
			var player = players[i].refreshPositionData();
			var playerBeforeUpdate = playersBeforeUpdate[i];
            var waveHeight = waveSystem.getWaveHeight(player.x);

            Debug.Log(player.x+" sigh " +playerBeforeUpdate.y+" - "+waveHeight + " * " + player.y + " - " + waveHeight + " / " + player.y );
            if ((playerBeforeUpdate.y - waveHeight) * (player.y - waveHeight) < 0&& player.y<=waveHeight) {
                bool tooSoon = false;
                foreach (var item in existingCollisions)
                {
                    if (isCollisionTooSoon(item, currentTime, player.id))
                        tooSoon = true;
                }
                if (!tooSoon)
                {
                    //legit collision
                    Debug.Log("legit collision mammt");
                    waveSystem.PushDown(player.x, getPreferredPush(player));
                }
			}
		}

	}

    void forgetOldCollisions(float currentTime) {
        
        existingCollisions.RemoveAll(delegate (PlayerFluidCollision item)
        {
            return !isCollisionTooSoon(item, currentTime, item.player.id);  
        });
    }
    bool isCollisionTooSoon(PlayerFluidCollision item, float currentTime, int playerId)
    {
        return  playerId == item.player.id &&(
                    (currentTime - item.collisionTime < refractaryCollisionPeriod) 
                    //||
                    //((players[playerId].transform.position - item.collisionPosition).magnitude < refractaryCollisionDistance )
                );
    }

    HorzDir getPreferredPush(PlayerState player) { return player.playerHousePosition == HorzDir.Left ? HorzDir.Right : HorzDir.Left; }

	private struct PlayerFluidCollision {
		public PlayerState player;
        public float collisionTime;
        public Vector3 collisionPosition;
	}
}
