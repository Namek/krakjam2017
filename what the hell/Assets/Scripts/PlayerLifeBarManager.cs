using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeBarManager : MonoBehaviour
{
    float actualValue;
    float updateDuration = .3f;
    float lastUpdateTime;
    public HorzDir side;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    Slider shower;
    // Use this for initialization
    void Start()
    {
        Init();
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.baseHitByWave, OnBaseHit);
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameStart, OnGameStart);

    }
    void OnGameStart(object o)
    {
        Init();
    }
    void Init()
    {
        lastUpdateTime = -1000;
        shower.value = 1;
        actualValue = 1;
    }
    void OnBaseHit(object o)
    {
        HorzDir hitPlayer = (o as WaveState).horzDir;
        if (side == hitPlayer)
        {
            actualValue = gameManager.getHealth(hitPlayer) / (float)gameManager.startingPlayerBaseHealth[(int)hitPlayer];
            lastUpdateTime = Time.time;
        }
    }

    void Update()
    {
        shower.value = Mathf.Lerp(shower.value, actualValue, Mathf.Clamp01((Time.time - lastUpdateTime) / updateDuration));
    }
}
