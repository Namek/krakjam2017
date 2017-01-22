using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    MoveFunction[] movement= new MoveFunction[2];
    CanKeepAccumulating[] jumpAccumulationChecker = new CanKeepAccumulating[2];
    StartAccumulatePowerFunction[] startAccumulating = new StartAccumulatePowerFunction[2];
    StopAccumulatePowerFunction[] stopAccumulating = new StopAccumulatePowerFunction[2];
    IsJumping[] isJumping = new IsJumping[2];

    // Use this for initialization
    public void Initialize (Transform[] controlledCharacter) {
        CharacterController[] characters = new CharacterController[2] {
            controlledCharacter[0].GetComponent<CharacterController>() ,
            controlledCharacter[1].GetComponent<CharacterController>()
        };

        for (int i = 0; i < 2; i++)
        {
            movement[i] = characters[i].Move;
            jumpAccumulationChecker[i] = characters[i].KeepAccumulatingJumpPower;
            startAccumulating[i] = characters[i].StartAccumulatingJumpPower;
            stopAccumulating[i] = characters[i].StopAccumulatingJumpPower;
            isJumping[i]= characters[i].IsJumping;
        }
	}

    public void waitForStart(GameManager manager)
    {
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetButtonDown(InputNames.Cancel.P(i + 1)))
                manager.GamePhase = GameManager.gameState.game;
        }
    }
    public void waitForRestart(GameManager manager)
    {
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetButtonDown(InputNames.Cancel.P(i + 1)))
                manager.GamePhase = GameManager.gameState.mainMenu;
        }
    }

    // Update is called once per frame
    public void UpdateCharacterMovements () {
        /*
        if (Input.GetButtonDown(InputNames.Fire1.P(1)))
        {
            eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openMainMenu, null);
        }
        if (Input.GetButtonDown(InputNames.Fire2.P(1)))
        {
            eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.closeMainMenu, null);
            eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openGameUI, null);
        }

        if (Input.GetButtonDown(InputNames.Fire3.P(1)))
        {
            eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.closeGameUI, null);
            eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.openEndMenu, null);
        }
        if (Input.GetButtonDown(InputNames.Jump.P(1)))
        {
            eventHandlerManager.globalBroadcast(this, eventChannels.menu, (int)menuChannelEvents.closeEndMenu, null);
        }*/

        for (int i = 0; i < 2; i++)
        {
            movement[i](Input.GetAxis(InputNames.Horizontal.P(i+1)));

            if (Input.GetButtonDown(InputNames.Fire1.P(i + 1)))
            {
                startAccumulating[i]();
            }
            if (Input.GetButton(InputNames.Fire1.P(i + 1)))
            {
                if(!jumpAccumulationChecker[i]())
                {
                        stopAccumulating[i]();
                }
            }
            if (Input.GetButtonUp(InputNames.Fire1.P(i + 1)))
            {
                stopAccumulating[i]();
            }

        }

	}
}
public delegate void MoveFunction(float xSpeed);
public delegate void StartAccumulatePowerFunction();
public delegate void StopAccumulatePowerFunction();
public delegate bool CanKeepAccumulating();
public delegate bool IsJumping();