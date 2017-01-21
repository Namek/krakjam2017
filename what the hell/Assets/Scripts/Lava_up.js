#pragma strict

var startingLevel:float;
var endLevel: float;
var health: float;
var step: float;

function Awake()
{
startingLevel=transform.position.y;
endLevel=100;
health=100;
step=0.50;
}

function Update () {
	if (Input.GetKeyDown("space"))
	TakeDamge(10);
}

function TakeDamge( damage: float)
{

health=health-damage;
transform.position.y=transform.position.y+step;

}

	