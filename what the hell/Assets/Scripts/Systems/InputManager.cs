using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    MoveFunction[] movement= new MoveFunction[2];
    JumpFunction[] jumping= new JumpFunction[2];

    // Use this for initialization
    public void Initialize (Transform[] controlledCharacter) {
        CharacterController[] characters = new CharacterController[2] {
            controlledCharacter[0].GetComponent<CharacterController>() ,
            controlledCharacter[1].GetComponent<CharacterController>()
        };

        for (int i = 0; i < 2; i++)
        {
            movement[i] = characters[i].Move;
            jumping[i] = characters[i].Jump;
        }
	}
	
	// Update is called once per frame
	public void UpdateCharacterMovements () {
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