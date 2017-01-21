using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    MoveFunction[] movement= new MoveFunction[2];
    JumpFunction[] jumping= new JumpFunction[2];
    [SerializeField]
    CharacterController[] controlledCharacter;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 2; i++)
        {
        movement[i] = controlledCharacter[i].Move;
        jumping[i] = controlledCharacter[i].Jump;
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 2; i++)
        {
            movement[i](Input.GetAxis(InputNames.Horizontal.P(i+1)));
         
            if (Input.GetButtonDown(InputNames.Fire1.P(i + 1)))
            {
                jumping[i]();
            }

        }

	}
}
public delegate void MoveFunction(float xSpeed);
public delegate void JumpFunction();