using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveFillManager : MonoBehaviour {

    float actualValue;
    Vector3 actualZ= new Vector3();
    float updateDuration = .3f;
    float lastUpdateTime;
    public HorzDir side;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    MockLava shower;
    float value;
    [SerializeField]
    Vector3 starterPos;//1.92 0.77 -2.42
    [SerializeField]
    Vector3 arrivalPos;//1.27 0.78 -0.45
    Vector3[] topZval; //= new Vector2(-2.87f, -1.27f);
    [SerializeField]
    Vector2 additionalVal = new Vector2(-1, 1);
    // Use this for initialization
    void Start()
    {
       topZval = new Vector3[2];
        topZval[0] = starterPos;
        topZval[1] = arrivalPos;
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
        value = 0;
        actualValue = additionalVal.x;
        actualZ = topZval[0];
    }
    void OnBaseHit(object o)
    {
        HorzDir hitPlayer = (o as WaveState).horzDir;
        if (side == hitPlayer)
        {
            lastUpdateTime = Time.time;
            value = 1 - (gameManager.getHealth(hitPlayer) / (float)gameManager.startingPlayerBaseHealth[(int)hitPlayer]);
            actualValue = (1 - value) * additionalVal.x + value * additionalVal.y;
            actualZ = Vector3.Lerp( topZval[0] , topZval[1], value );
        }
    }
    void Update()
    {
        float interpolator = Mathf.Clamp01((Time.time - lastUpdateTime )/ updateDuration);
        shower.additionalHeight = Mathf.Lerp(shower.additionalHeight, actualValue, interpolator);
        transform.localPosition= Vector3.Lerp(transform.localPosition, actualZ, interpolator) ;
    }
}
