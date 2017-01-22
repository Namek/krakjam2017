using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WaveState {
	public float xCenter;
	public float xCenterOnStart;
	public float lifeTime = 0;//range: [0, 1]
	public float altitude;
	public float targetAltitude;
	public HorzDir horzDir;
	public float speed;
	public bool isWasted = false;
	public bool isCollidable = true;
	public bool hasDealtDamage = false;
}
