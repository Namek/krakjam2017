using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseParticleManager : MonoBehaviour {
    public HorzDir side;
    public float damageFactor = 50f;
    public ParticleSystem particle;
    public ParticleSystem.MainModule mainmod;
    float timeToStop;
    // Use this for initialization
    void Awake () {
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.baseHitByWave, OnBaseHit);
        mainmod = particle.main;

    }
	void OnBaseHit(object o)
    {
        WaveState w = o as WaveState;
        if (w.horzDir == side)
        {
                particle.Play();
            if (timeToStop <= Time.time)
            {
                timeToStop = Time.time + w.altitude * damageFactor;
                UniqueCoroutine.UCoroutine(this, stopDelayed(), GetInstanceID().ToString());
                mainmod.startLifetime = w.altitude * damageFactor;
            }
            else
            {
                timeToStop += w.altitude * damageFactor;
                mainmod.startLifetime = new ParticleSystem.MinMaxCurve( Mathf.Max(w.altitude * damageFactor, mainmod.startLifetime.constant));
            }
        }
    }

    IEnumerator stopDelayed() {
        while(Time.time<timeToStop)
        {
            yield return new WaitForEndOfFrame();
        }
        particle.Stop();
    }
}
