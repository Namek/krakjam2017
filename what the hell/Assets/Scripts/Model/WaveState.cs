using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WaveState {
	public float xCenter;
	public float xCenterOnStart;
	public float altitude;
	public HorzDir horzDir;
	public float speed;
	public bool isWasted = false;
	public bool isCollidable = true;
	public bool hasDealtDamage = false;
}
