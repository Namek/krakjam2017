using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    timerCurve jumpHeightCurve;
    [SerializeField]
    AnimationCurve accelerationCurve;
    [SerializeField]
    AnimationCurve decelerationCurve;

    float startingHeight;
    [SerializeField]
    float movementSpeed;
    float currentSpeed;
    float lastAccelerationTime;
    [SerializeField]
    float jumpMaxHeight;
    bool isDecelerating;
    
    public void Start()
    {
        jumpHeightCurve.setOver();
        startingHeight = transform.position.y;
    }

    public void Jump()
    {
        if (jumpHeightCurve.isOver) { 
            jumpHeightCurve.Reset();
            UniqueCoroutine.doUntil(this, delegate ()
            {
                jumpHeightCurve.refreshTime();
                transform.position = new Vector3(transform.position.x, jumpHeightCurve.CurrentValue*jumpMaxHeight, transform.position.z);
            }, delegate () { return !jumpHeightCurve.isOver; });
        }
    }


    public void Move(float xAxisSpeed) {
        currentSpeed = Mathf.Clamp(currentSpeed+ xAxisSpeed *Time.deltaTime,-1,1);
        //if current speed sign not equal to axis speed sign their product is less then 0 and therefore is decelerating
        isDecelerating = currentSpeed * xAxisSpeed < 0|| xAxisSpeed==0;
        if(!isDecelerating)
        { lastAccelerationTime = Time.time; }
    }

    void updateXposition() {

        //update pos
        transform.position += Vector3.right * accelerationCurve.Evaluate(Mathf.Clamp01(Mathf.Abs( currentSpeed))) * movementSpeed * (currentSpeed > 0 ? 1 : -1);

        //dampen current speed
        if (isDecelerating)
            currentSpeed = decelerationCurve.Evaluate(Time.time - lastAccelerationTime) *(currentSpeed>0?1:-1);
    }
    public void Struggle() { }

    void Update() { 
        updateXposition();
    }

}
