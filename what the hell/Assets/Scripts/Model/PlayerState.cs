using System;

public class PlayerState {
    public UnityEngine.Transform transform;
    public float x;
    public float y;
    public float speed;
	public int id;
	public HorzDir playerHousePosition;
	public bool isCapturedByWave = false;
	public float previousHeightDiff = 0;

	public PlayerState(int id) {
		this.id = id;
	}		   

	public PlayerState setHouse(HorzDir playerHousePosition) {
		this.playerHousePosition = playerHousePosition;
		return this;
	}

    public PlayerState setTransform(UnityEngine.Transform transform)
    {
        this.transform = transform;
        return this;
    }
    public PlayerState setX(float x)
    {
        this.x = x;
        return this;
    }
    public PlayerState setY(float y)
    {
        this.y = y;
        return this;
    }

    public PlayerState refreshPositionData(float waveHeight)
    {
        this.x = transform.position.x;
        this.y = transform.position.y;
		this.previousHeightDiff = this.y - waveHeight;
        return this;
    }

    public void setValues(PlayerState other)
    {
        this.transform = other.transform;
        this.x = other.x;
        this.y = other.y;
        this.speed = other.speed;
		this.playerHousePosition = other.playerHousePosition;

		this.previousHeightDiff = other.previousHeightDiff;

	}

}