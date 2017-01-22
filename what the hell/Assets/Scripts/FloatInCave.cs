using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatInCave : MonoBehaviour
{
    [SerializeField]
    MockLava lava;
    float StartingY;
    public void Awake()
    {
        StartingY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 
            StartingY+ lava.additionalHeight + Mathf.Sin(Time.time) * .25f, transform.localPosition.z);
    }
}
