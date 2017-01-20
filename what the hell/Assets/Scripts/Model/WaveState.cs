using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WaveState {
	public float xCenter;
	public float altitude;
	public HorzDir horzDir;
	public float speed;

	public enum HorzDir {
		Left, Right
	}					
}
