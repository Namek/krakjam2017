using System;

public class PlayerState {
	public float x;
	public float y;
	public float speed;
	public int id;

	public PlayerState(int id) {
		this.id = id;
	}		   

	public PlayerState setX(float x) {
		this.x = x;
		return this;
	}

	public PlayerState setY(float y) {
		this.y = y;
		return this;
	}

	public void setValues(PlayerState other) {
		this.x = other.x;
		this.y = other.y;
		this.speed = other.speed;
	}
}