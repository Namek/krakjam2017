using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeBarManager : MonoBehaviour
{
    public HorzDir side;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    Slider shower;
    // Use this for initialization
    void Start ()
    {
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.baseHitByWave, OnBaseHit);
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameStart, OnGameStart);

    }
    void OnGameStart(object o    )
    {
        shower.value = 1;
    }

    void OnBaseHit(object o)
    {
        HorzDir hitPlayer = (o as WaveState).horzDir;
        if (side == hitPlayer)
            shower.value = gameManager.getHealth(hitPlayer) / (float)gameManager.startingPlayerBaseHealth[(int)hitPlayer];
    }
}
