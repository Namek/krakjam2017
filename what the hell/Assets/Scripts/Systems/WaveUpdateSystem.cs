using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveUpdateSystem {
	private const float WAVE_MIN_PERCENT_X_CENTER_DIFF_TO_COLLIDE = 0.01f;

	GameManager gameManager;

	private List<WaveState> waves = new List<WaveState>();
	private List<WaveState> wavesToRemove = new List<WaveState>();
	private List<WaveState> wavesToAdd = new List<WaveState>();


	public WaveUpdateSystem(GameManager gameManager) {
		this.gameManager = gameManager;
	}										  
	

	public void Update(PlayerState[] playerStates, float deltaTime) {
		// move waves
		foreach (var wave in waves) {
			wave.xCenter += (wave.horzDir == WaveState.HorzDir.Left ? -1 : 1) * deltaTime * wave.speed;
			float width2 = calcWaveWidth(wave)/2;

			if (wave.xCenter - width2 > gameManager.laneRight || wave.xCenter + width2 < gameManager.laneLeft) {
				// this wave completely disappears
				wavesToRemove.Add(wave);
			}
		}
		foreach (var wave in wavesToRemove) {
			waves.Remove(wave);
		}
		wavesToRemove.Clear();

		// detect wave collision on players
		foreach (var p in playerStates) {
			foreach (var wave in waves) {
				// TODO
			}
		}

		// detect collisions between them
		foreach (var wave1 in waves) {
			foreach (var wave2 in waves) {
				float width1 = calcWaveWidth(wave1);
				float width2 = calcWaveWidth(wave2);

				var w1w2Diff = wave1.xCenter - wave2.xCenter;
				var w2w1Diff = wave2.xCenter - wave1.xCenter;
				bool rightCollision = w1w2Diff >= 0;
				bool leftCollision = w2w1Diff >= 0;
				bool doesCollide = leftCollision || rightCollision;
			
				if (!doesCollide) 
					continue;

				// collision result depends on directions:

				// 1. same way and overlapped lot enough? Merge them into one stronger wave
				if (wave1.horzDir == wave2.horzDir) {
					// nothing will happen if both have same speed
					if (Math.Abs(wave1.speed - wave2.speed) < 0.001f) {
						if (leftCollision && w1w2Diff >= WAVE_MIN_PERCENT_X_CENTER_DIFF_TO_COLLIDE
							|| rightCollision && w2w1Diff >= WAVE_MIN_PERCENT_X_CENTER_DIFF_TO_COLLIDE
						) {
							// TODO merge waves into one
							wavesToAdd.Add(mergeSameWayWaves(wave1, wave2));
							wavesToRemove.Add(wave1);
							wavesToRemove.Add(wave2);
						}
                    }
				}

				// 2. they're coming at each other - merge them to a weaker one
				//    or remove both if they equal in altitude
				else {
					if (Math.Abs(wave1.altitude - wave2.altitude) < 0.001f) {
						wavesToAdd.Add(mergeOppositeWaves(wave1, wave2));
						wavesToRemove.Add(wave1);
						wavesToRemove.Add(wave2);
					}
					else {
						if (leftCollision) {
							// TODO
						}
						else if (rightCollision) {
							// TODO
						}
					}
				}
			}
		}


		foreach (var wave in wavesToRemove) {
			waves.Remove(wave);
		}
		wavesToRemove.Clear();

		foreach (var wave in wavesToAdd) {
			waves.Add(wave);
		}
		wavesToAdd.Clear();
	}

	public void PushDown(float x) {
		// TODO

	}

	public void CreateWave(float xLeftOnStart, WaveState.HorzDir dir) {
		var wave = new WaveState();
		wave.xCenterOnStart = xLeftOnStart;
		wave.altitude = 3;
		wave.horzDir = dir;
		wave.speed = 16f;
		this.waves.Add(wave);
	}

	public float getWaveHeight(float x) {
		float altitude = 0;

		foreach (var wave in waves) {
			float waveWidth = calcWaveWidth(wave);
			float xRight = wave.xCenter + waveWidth/2;
			float dirX = wave.horzDir == WaveState.HorzDir.Left ? -1 : 1;

			if (isPointInWaveRegion(wave, x)) {
				// should be within [0, 1]
				float startPointDistFactor = Math.Min(1, Math.Abs(wave.xCenterOnStart - x) / waveWidth*2);

				// should be within [-0.5, 0.5]
				float centerDistFactor = (wave.xCenter - x)*2 / waveWidth;
				float y = Mathf.Cos(centerDistFactor * Mathf.PI/2) * wave.altitude * startPointDistFactor;

				if (y > altitude) {
					altitude = y;
				}							
            }
        }

		return altitude;
	}

	private static bool isPointInWaveRegion(WaveState wave, float x) {
		float width2 = calcWaveWidth(wave);
		return x >= (wave.xCenter - width2) && x <= (wave.xCenter + width2);
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

		newWave.xCenter = dir == WaveState.HorzDir.Left && wave1.xCenter < wave2.xCenter
			? wave1.xCenter : wave2.xCenter;

		return newWave;
	}

	private static WaveState mergeOppositeWaves(WaveState wave1, WaveState wave2) {
		//Assert(wave1.horzDir != wave2.horzDir && wave1.altitude );
		var newWave = new WaveState();

		return null;
		//wave1.
		// TODO
	}
}
