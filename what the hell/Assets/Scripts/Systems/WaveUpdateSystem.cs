﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveUpdateSystem {
	private const float WAVE_MIN_WIDTH_DIFF_TO_COLLIDE = 0.01f;
	private const float WAVE_TOP_PERCENT_WIDTH_TO_PUSH_DOWN = 0.2f;
	private const float WAVE_SPEEDUP_FACTOR = 2f;
	private const float WAVE_ALTITUDE_GROW_FACTOR = 1.1f;
	private const float WAVE_MAX_ALTITUDE = 10;

	GameManager gameManager;

	public List<WaveState> waves = new List<WaveState>();
	private List<WaveState> wavesToAdd = new List<WaveState>();
	private List<WaveState> wavesToRemove = new List<WaveState>();


	public WaveUpdateSystem(GameManager gameManager) {
		this.gameManager = gameManager;
	}										  
	

	public void Update(PlayerState[] playersStates, float deltaTime) {
		foreach (var wave in wavesToAdd) {
			waves.Add(wave);
		}
		wavesToAdd.Clear();

		// move waves
		for (int i = 0; i < waves.Count; ++i) {
			var wave = waves[i];
			bool removeMe = false;

			if (wave.isCollidable) {
				wave.xCenter += (wave.horzDir == HorzDir.Left ? -1 : 1) * deltaTime * wave.speed;
				float width2 = calcWaveWidth(wave)/2;

				if (wave.xCenter < gameManager.laneLeft || wave.xCenter > gameManager.laneRight) {
					if (!wave.hasDealtDamage) {
						wave.hasDealtDamage = true;
						eventHandlerManager.globalBroadcast(null, eventChannels.inGame, (int)inGameChannelEvents.baseHitByWave, wave);
					}
				}

				if (wave.xCenter - width2 > gameManager.laneRight || wave.xCenter + width2 < gameManager.laneLeft) {
					// this wave completely disappears
					removeMe = true;
				}
			}
			else {
				// these animate shrinking
				wave.altitude -= deltaTime * wave.speed;

				if (wave.altitude < 0) {
					removeMe = true;
				}
			}

			if (removeMe) {
				waves.RemoveAt(i);
				--i; continue;
			}
		}

		// detect collisions between waves
		for (int i = 0; i < waves.Count; ++i) {
			for (int j = 1; j < waves.Count; ++j) {
				if (i == j)
					continue;

				var wave1 = waves[i];
				var wave2 = waves[j];

				if (wave1.isWasted || wave2.isWasted || !wave1.isCollidable || !wave2.isCollidable)
					continue;

				float width1 = calcWaveWidth(wave1);
				float width2 = calcWaveWidth(wave2);

				// wave1 to wave2
				bool doesCollide = wave1.xCenter + width1/2 > wave2.xCenter - width2/2
					&& wave2.xCenter + width2/2 > wave1.xCenter - width1/2;
				
				var w1w2Diff = Math.Abs(wave1.xCenter - wave2.xCenter);
			
				if (!doesCollide) 
					continue;
				

				// collision result depends on directions:

				// 1. same way and overlapped lot enough? Merge them into one stronger wave
				if (wave1.horzDir == wave2.horzDir) {
					// nothing will happen if both have same speed
					if (Math.Abs(wave1.speed - wave2.speed) < 0.001f) {
						wave1.isWasted = true;
						wave2.isWasted = true;
                    }
				}

				// 2. they're coming at each other - merge them
				//    or remove both if they equal in altitude
				else {
					var diff = wave1.xCenter - wave2.xCenter;

					if (Math.Abs(diff) > 0.4)
						continue;

					if (Math.Abs(wave1.altitude - wave2.altitude) > 0.001f) {
						wavesToAdd.Add(mergeOppositeWaves(wave1, wave2));
					}
					else {
						var newWave = mergeOppositeWaves(wave1, wave2);
						newWave.isCollidable = false;
						newWave.altitude = (wave2.altitude + wave1.altitude) / 2;
						wavesToAdd.Add(newWave);
					}

					wave1.isWasted = true;
					wave2.isWasted = true;
				}
			}
		}

		for (var i = 0; i < waves.Count; ++i) {
			if (waves[i].isWasted) {
				waves.RemoveAt(i);
				--i;
			}
		}

		// maybe some wave will take the player with it...
		foreach (var player in playersStates) {
			if (player.isCapturedByWave)
				continue;

			float closestDist = float.MaxValue;
			WaveState closestWave = null;

			foreach (var wave in waves) {
				float dist = Math.Abs(wave.xCenter - player.x);

				if (dist < closestDist) {
					// TODO
					//closestDist = dist;
					//closestWave = wave;
				}
			}

			if (closestWave != null) {
				eventHandlerManager.globalBroadcast(null, eventChannels.inGame, (int)inGameChannelEvents.playerHitByWave, player);
				continue;
			}
		}
	}

	public void PushDown(float x, HorzDir preferredPushDir) {
		WaveState closestWave = null;
		float highestAltitude = 0;

		foreach (var wave in waves) {
			if (!wave.isCollidable)
				continue;

			if (isPointInWaveRegion(wave, x)) {
				float y = calcWaveHeight(wave, x);
				if (highestAltitude < y) {
					highestAltitude = y;
					closestWave = wave;
				}
			}
		}

		if (closestWave == null) {
			// create new wave
			this.CreateWave(x, preferredPushDir);
		}

		else {
			// detect collision region: front/back/center
			float width = calcWaveWidth(closestWave);
			float topWidth2 = WAVE_TOP_PERCENT_WIDTH_TO_PUSH_DOWN * width / 2;

			// stepped on the tight center, make it faster and push backwards!
			// ... and make it little bigger
			bool didHitTightCenter = Math.Abs(x - closestWave.xCenter) <= topWidth2;
			HorzDir pushDir = preferredPushDir;

			if (didHitTightCenter) {
				closestWave.speed *= WAVE_SPEEDUP_FACTOR;
				closestWave.altitude = Math.Min(
					closestWave.altitude * WAVE_ALTITUDE_GROW_FACTOR,
					WAVE_MAX_ALTITUDE
				);
			}
			else {
				// stepped on the left, push to the right
				if (x < closestWave.xCenter) {
					pushDir = HorzDir.Right;
				}

				// stepped on the right, push to the left
				else if (x > closestWave.xCenter) {
					pushDir = HorzDir.Left;
				}
			}

			closestWave.horzDir = pushDir;
		}
	}

	public void CreateWave(float xCenterOnStart, HorzDir dir) {
		var wave = new WaveState();
		wave.xCenterOnStart = xCenterOnStart;
		wave.xCenter = xCenterOnStart;
		wave.altitude = 3;
		wave.horzDir = dir;
		wave.speed = 16f;
		this.wavesToAdd.Add(wave);
	}

	// gets altitude "sum" of all waves
	public float getWaveHeight(float x) {
		float altitude = 0;

		foreach (var wave in waves) {
			float waveWidth = calcWaveWidth(wave);
			float xRight = wave.xCenter + waveWidth/2;
			float dirX = wave.horzDir == HorzDir.Left ? -1 : 1;

			if (isPointInWaveRegion(wave, x)) {
				float y = calcWaveHeight(wave, x);

				if (y > altitude) {
					altitude = y;
				}							
            }
        }

		return altitude+1f;
	}

	private static bool isPointInWaveRegion(WaveState wave, float x) {
		float width2 = calcWaveWidth(wave)/2f;
		return x > (wave.xCenter - width2) && x < (wave.xCenter + width2);
	}

	// the `x` is global, this ignores merging waves
	private static float calcWaveHeight(WaveState wave, float x) {
		float waveWidth = calcWaveWidth(wave);

		// should be within [0, 1]
		float startPointDistFactor = wave.isCollidable
			? Math.Min(1, Math.Abs(wave.xCenterOnStart - x) / waveWidth*2)
			: 1;

		// should be within [-0.5, 0.5]
		float centerDistFactor = (wave.xCenter - x)*2 / waveWidth;
		float y = Mathf.Cos(centerDistFactor * Mathf.PI/2) * wave.altitude * startPointDistFactor;

		return y;
	}

	private static float calcWaveWidth(WaveState wave) {
		return wave.altitude * 2;
	}
	
	private static WaveState mergeSameWayWaves(WaveState wave1, WaveState wave2) {
		//Assert(wave1.horzDir == wave2.horzDir);
		var newWave = new WaveState();
		var dir = wave1.horzDir;

		newWave.horzDir = wave1.horzDir;
		newWave.altitude = (wave1.altitude + wave2.altitude)/2;
		newWave.speed = (wave1.speed + wave2.speed)/2;

		newWave.xCenter = dir == HorzDir.Left && wave1.xCenter < wave2.xCenter
			? wave1.xCenter : wave2.xCenter;

		return newWave;
	}

	private static WaveState mergeOppositeWaves(WaveState wave1, WaveState wave2) {
		//Assert(wave1.horzDir != wave2.horzDir );
		var newWave = new WaveState();
		newWave.horzDir = wave1.altitude > wave2.altitude ? wave1.horzDir : wave2.horzDir;
		newWave.altitude = Math.Abs(wave2.altitude - wave1.altitude);
		newWave.speed = (wave1.speed + wave2.speed)/2;
		newWave.xCenter = (wave1.xCenter + wave2.xCenter)/2;

		return newWave;
	}
}
