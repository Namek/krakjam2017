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
			new PlayerState(0).setHouse(HorzDir.Left).setTransform(playerTransforms[0]).refreshPositionData(0),
			new PlayerState(1).setHouse(HorzDir.Right).setTransform(playerTransforms[1]).refreshPositionData(0)
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
        rememberPlayerStates();
        forgetOldCollisions(currentTime);


        for (var i = 0; i < players.Length; ++i) {
			var player = players[i];
            var waveHeight = waveSystem.getWaveHeight(player.transform.position.x);
			player.refreshPositionData(waveHeight);
			var playerBeforeUpdate = playersBeforeUpdate[i];

            //Debug.Log(player.x+" sigh " +playerBeforeUpdate.y+" - "+waveHeight + " * " + player.y + " - " + waveHeight + " / " + player.y );
            if ((playerBeforeUpdate.previousHeightDiff) * (player.previousHeightDiff) < 0 && player.y <= waveHeight) {
                bool tooSoon = false;
                foreach (var item in existingCollisions)
                {
                    if (isCollisionTooSoon(item, currentTime, player.id))
                        tooSoon = true;
                }
                if (!tooSoon)
                {
                    //legit collision
                    //Debug.Log("Porcaputtana "+existingCollisions);
                    //existingCollisions.PrintToLog(currentTime+" legit collision mammt");
                    Debug.Log("OnPushDown");
					waveSystem.PushDown(player.x, getPreferredPush(player), playerBeforeUpdate.y -player.y);
                    existingCollisions.Add(new PlayerFluidCollision() {
                        player=player, collisionTime=currentTime, collisionPosition=player.transform.position
                    });
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
                    ||
                    ((players[playerId].transform.position - item.collisionPosition).magnitude < refractaryCollisionDistance)
                );
    }

    HorzDir getPreferredPush(PlayerState player) { return player.playerHousePosition == HorzDir.Left ? HorzDir.Right : HorzDir.Left; }

	private struct PlayerFluidCollision {
		public PlayerState player;
        public float collisionTime;
        public Vector3 collisionPosition;
        public override string ToString()
        {
            return player.id+" "+collisionTime+" "+collisionPosition;
        }
    }
}
