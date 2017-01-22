using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour {
    [SerializeField]
    Text winAnnouncer;
    string baseText;
    string left="LEFT";
    string right="RIGHT";
    // Use this for initialization
    void Start ()
    {
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameOver, OnGameOver);
        baseText = winAnnouncer.text;
    }
    
    void OnGameOver(object o)
    {
        float[] scores= o as float[];
        winAnnouncer.text = (scores[0]>scores[1]?left:right)+ baseText;
    }
}
