﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    MoveFunction[] movement= new MoveFunction[2];
    JumpFunction[] jumpingExecutor = new JumpFunction[2];
    CanKeepAccumulating[] jumpAccumulationChecker = new CanKeepAccumulating[2];
    StartAccumulatePowerFunction[] startAccumulating = new StartAccumulatePowerFunction[2];
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
            jumpingExecutor[i] = characters[i].Jump;
            jumpAccumulationChecker[i] = characters[i].KeepAccumulatingJumpPower;
            startAccumulating[i] = characters[i].StartAccumulatingJumpPower;
            isJumping[i]= characters[i].IsJumping;
        }
	}

    public void waitForStart(GameManager manager)
    {
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetButtonDown(InputNames.Submit.P(i + 1)))
                manager.GamePhase = GameManager.gameState.game;
        }
    }
    public void waitForRestart(GameManager manager)
    {
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetButtonDown(InputNames.Submit.P(i + 1)))
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
                    if (!isJumping[i]())
                        jumpingExecutor[i]();
                }
            }
            if (Input.GetButtonUp(InputNames.Fire1.P(i + 1)))
            {
                if(!isJumping[i]())
                    jumpingExecutor[i]();
            }

        }

	}
}
public delegate void MoveFunction(float xSpeed);
public delegate void StartAccumulatePowerFunction();
public delegate void JumpFunction();
public delegate bool CanKeepAccumulating();
public delegate bool IsJumping();