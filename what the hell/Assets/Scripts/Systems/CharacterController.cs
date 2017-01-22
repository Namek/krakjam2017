using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Vector2 limits;

    [SerializeField]
    Animator animatorControl;
    [SerializeField]
    ParticleSystem auraParticle;
    [SerializeField]
    string  startJumpTrigger;
    [SerializeField]
    string heightValueName;
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
    bool auraIsActive;
    bool jumpOnce = false;
    bool enableGravity;

    float startingX;
    public void Start()
    {
        startingHeight = transform.position.y;
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameStart, OnGameStart);
        startingX=transform.position.x;
    }
    void OnGameStart(object o)
    {
        verticalSpeed = 0;
        transform.position= new Vector3(startingX, startingHeight,transform.position.z);
    }

    public void StartAccumulatingJumpPower() {
        //start jump
        if (!jumpOnce)
        {
            initJump();
        }
    }
    void initJump() {
        animatorControl.SetTrigger(startJumpTrigger);
        jumpOnce = true;
        jumpStartTime = Time.time;
        verticalSpeed = 0;
        enableGravity = false;
    }
    public bool KeepAccumulatingJumpPower()
    {
        if (!IsJumping())
            initJump();

        showAccumulation();
        if (Time.time - jumpStartTime < jumpMaxPowerupDuration)
            verticalSpeed += jumpAcceleration*Time.deltaTime;
        //Debug.Log("KeepAccumulatingJumpPower");
        return Time.time - jumpStartTime < jumpMaxPowerupDuration;
    }
    void showAccumulation()
    {
        if (Time.time - jumpStartTime > minJumpDuration)
        {
            activateAura();
        }
    }
    public bool IsJumping()
    {
        return (transform.position.y>startingHeight ||verticalSpeed>0);
    }

    public void StopAccumulatingJumpPower() {
        if (Time.time - jumpStartTime > minJumpDuration)
        {
            stopAura();
            enableGravity = true; }
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
            float newHeight = Mathf.Max(startingHeight, transform.position.y + verticalSpeed);
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
            animatorControl.SetFloat(heightValueName, newHeight);
            if (enableGravity)
                verticalSpeed -= jumpGravity * Time.deltaTime;
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
        float nextStep = accelerationCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(currentSpeed))) * movementSpeed * (currentSpeed > 0 ? 1 : -1);
        if (limits.x<transform.position.x+nextStep&&transform.position.x+nextStep<limits.y)
        transform.position += Vector3.right * nextStep;

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
        if(!auraIsActive)
        { 
        auraParticle.Stop();
        auraParticle.Play();
            auraIsActive = true;
        }
    }
    void stopAura() {
        auraParticle.Stop();
        auraIsActive = false;
    }
}
