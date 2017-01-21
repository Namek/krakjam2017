using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    Animator animatorControl;
    [SerializeField]
    ParticleSystem auraParticle;
    [SerializeField]
    string  startChargingTrigger;
    [SerializeField]
    string unleashTrigger;
    [SerializeField]
    string jumpCompletionValueName;
    [SerializeField]
    string horizontalSpeedValueName;
    
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
    float jumpMaxPowerupDuration;
    float jumpStartTime;
    float verticalSpeed;
    bool isDecelerating;
    [SerializeField]
    [Range(0.1f, 1)]
    float jumpAcceleration;
    [SerializeField]
    float jumpGravity;
    [SerializeField]
    float minJumpDuration;

    bool jumpOnce = false;
    bool enableGravity;

    public void Start()
    {
        startingHeight = transform.position.y;
    }

    public void StartAccumulatingJumpPower() {
        //start jump
        Debug.Log("StartAccumulatingJumpPower "+ IsJumping());
        if (!jumpOnce)
        {
            initJump();
        }
    }
    void initJump() {
        jumpOnce = true;
        jumpStartTime = Time.time;
        verticalSpeed = 0;
        enableGravity = false;
    }
    public bool KeepAccumulatingJumpPower()
    {
        //power up jump

        //showAccumulation();
        if (Time.time - jumpStartTime < jumpMaxPowerupDuration)
            verticalSpeed += jumpAcceleration*Time.deltaTime;
        //Debug.Log("KeepAccumulatingJumpPower");
        return Time.time - jumpStartTime < jumpMaxPowerupDuration;
    }
    //void showAccumulation() {
    //}
    public bool IsJumping()
    {
        return (transform.position.y>startingHeight ||verticalSpeed>0);
    }

    public void StopAccumulatingJumpPower() {
        if (Time.time - jumpStartTime > minJumpDuration)
            enableGravity = true;
        else
            UniqueCoroutine.UCoroutine(this, doMinimumJump(), "mammt");
    }
    IEnumerator doMinimumJump() {
        while (Time.time - jumpStartTime < minJumpDuration)
        { yield return new WaitForEndOfFrame();
            verticalSpeed += jumpAcceleration * Time.deltaTime;
        }
        enableGravity = true;
    }
    void updateYposition() {
        if (IsJumping())
        {
            transform.position = new Vector3(transform.position.x,
                Mathf.Max(startingHeight, transform.position.y + verticalSpeed)
                , transform.position.z);
            if (enableGravity)
                verticalSpeed -= jumpGravity * Time.deltaTime;
            //animatorControl.SetFloat(jumpCompletionValueName, jumpCurvePercent);
        }
        else
        {
            initJump();
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
        animatorControl.SetFloat(horizontalSpeedValueName, currentSpeed);
        //update pos
        transform.position += Vector3.right * accelerationCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(currentSpeed))) * movementSpeed * (currentSpeed > 0 ? 1 : -1);

        //dampen current speed
        if (isDecelerating)
            currentSpeed = decelerationCurve.Evaluate(Time.time - lastAccelerationTime) *(currentSpeed>0?1:-1);
    }
    public void Struggle() { }

    void Update() { 
        updateXposition();
        updateYposition();
    }
    void activateAura() {
        auraParticle.Stop();
        auraParticle.Play();
    }
    void stopAura() {
        auraParticle.Stop();
    }
}
