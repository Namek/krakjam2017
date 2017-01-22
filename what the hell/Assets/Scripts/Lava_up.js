#pragma strict

var startingLevel:float;
var endLevel: float;
var health: float;
var step: float;
var mockHeight:float;
var mockScript:PlayerScript;
private var mock : GameObject;
function Awake()
{
mock= GameObject.Find("LavaMesh");
var  mockScript :PlayerScript =mock.GetComponent(PlayerScript);
mockHeight=mockScript.additionalHeight;

waiveHeight=transform.position.y;
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
mockHeight=mockHeight+step;
}

	