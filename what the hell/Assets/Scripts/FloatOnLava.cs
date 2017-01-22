using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatOnLava : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;
    WaveUpdateSystem waver;

    public float floatingAmplitude = .25f;

    public void Start()
    {
        waver = gameManager.waveUpdateSystem;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, waver.getWaveHeight(transform.position.x) + Mathf.Sin(Time.time) * floatingAmplitude, transform.position.z);
    }
}
