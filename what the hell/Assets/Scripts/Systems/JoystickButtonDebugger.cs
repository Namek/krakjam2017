using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoystickButtonDebugger : MonoBehaviour
{
    public bool debug;
    [Range(1, 4)]
    public int playerLimit;

    // Awake is called when the script instance
    // is being loaded.
    void Awake()
    {
    }

    // Update is called every frame, if the
    // MonoBehaviour is enabled.
    void Update()
    {

        if (debug)
        {
            foreach (InputNames item in System.Enum.GetValues(typeof(InputNames)))
            {
                for (int i = 1; i <= playerLimit; i++)
                {

                    if (Mathf.Abs(Input.GetAxis(item.P(i))) > 0.2f)
                        Debug.Log("JoystickButtonDebugger: " + item.P(i));

                }
            }
        }
    }

}