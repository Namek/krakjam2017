using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObject: MonoBehaviour
{
	public Transform myTransform;
	public Rigidbody rb;
	public float waterLevel = 4;
	public float bounceDamp = 0.5f;
	public float waterHeight = 2;
	public Vector3 centerOffset;

	private float randomFloatForce;
	private float forceFactor;
	private Vector3 actionPoint;
	private Vector3 upLift;

	void Awake()
	{
		myTransform = GetComponent<Transform>();
		rb=GetComponent<Rigidbody>();
		InvokeRepeating("RandomFloatForce",Random.Range(0,1),Random.Range(2,3));
	}

	void FixedUpdate()
	{
		actionPoint = myTransform.position + myTransform.TransformDirection(centerOffset);
		forceFactor = 1 + randomFloatForce -(actionPoint.y-(waterLevel/waterHeight));

		if(forceFactor>0f)
		{
			upLift= -Physics.gravity * (forceFactor - rb.velocity.y * bounceDamp);
			rb.AddForceAtPosition(upLift*rb.mass,actionPoint);


		}

	}

	public void RandomFloatForce()
	{
		if 
		(randomFloatForce == 0)
		{
			randomFloatForce = Random.Range(.025f, .125f);
		}
		else
		{
			randomFloatForce = 0;
		}
	}
}