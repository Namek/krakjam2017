#pragma strict
var waterLevel : float = 4;
var floatHeight :float = 2;
var bounceDamp :float = 0.05;
var CentreOffset: Vector3;
var mycollider: Collider;
var LavaPos:GameObject;
	private var forceFactor:float;
	private var actionPoint :Vector3;
	private var upLift: Vector3;
	private var randomFloatForce: float;
	private var isFloating = false;

function Awake()
{
isFloating = true;
mycollider= GetComponent(BoxCollider);
InvokeRepeating("RandomFloatForce",Random.Range(0,1),Random.Range(2,3));
}
function Update()
{



		actionPoint = transform.position + transform.TransformDirection(CentreOffset);
		forceFactor = 1f +randomFloatForce-((actionPoint.y-waterLevel)/floatHeight);

		if(forceFactor > 0f && isFloating==true)
		{
		if(transform.position.y<=LavaPos.transform.position.y)
		{
		Debug.Log("Floating!");
			upLift= -Physics.gravity * (forceFactor - GetComponent.<Rigidbody>().velocity.y * bounceDamp);
			GetComponent.<Rigidbody>().AddForceAtPosition(upLift,actionPoint);

			}
			else{Debug.Log("Is below!");

			}
		}
		else
		{}

}

function OnCollisionEnter( col: Collision)

{
Debug.Log("Kolizja!");
if(col.gameObject.tag=="ground")
{
Debug.Log("It is ground!");

isFloating = false;
}
if(col.gameObject.tag=="lava")
{
Debug.Log("It is lava!");
mycollider.isTrigger=true;
isFloating = true;
}



}
function RandomFloatForce()
	{
		if 
		(randomFloatForce == 0)
		{
			randomFloatForce = Random.Range(.325f, .525f);
		}
		else
		{
			randomFloatForce = 0;
		}
	}


	