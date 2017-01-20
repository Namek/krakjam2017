using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    MoveFunction movement;
    JumpFunction jumping;
    [SerializeField]
    CharacterController controlledCharacter;

    // Use this for initialization
    void Start () {
        movement = controlledCharacter.Move;
        jumping = controlledCharacter.Jump;
	}
	
	// Update is called once per frame
	void Update () {

        movement(Input.GetAxis("Horizontal"));
         
        if (Input.GetButtonDown("Fire1"))
        {
            jumping();
        }
	}
}
public delegate void MoveFunction(float xSpeed);
public delegate void JumpFunction();