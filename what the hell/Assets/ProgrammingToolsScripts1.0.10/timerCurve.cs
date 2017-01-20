using UnityEngine;

[System.Serializable]
public class timerCurve
{
    /*
    timer "tascabile" con curva annessa. La curva deve essere normalizzata da 0 a 1, il tempo di durata massimo viene gestito con timeToMax
    necessita di un updateTime() ogni frame e di un Reset() prima della partenza
    poi basta usare .CurrentValue ogni volta che serve il valore
    */
    public timerCurve(float timeToMax, float currentTimeSinceStart, float StartTime, AnimationCurve controlledFactor, bool debug, string debugmessage)
    {
        this.timeToMax = timeToMax;
        this.currentTimeSinceStart = currentTimeSinceStart;
        this.StartTime = StartTime;
        this.controlledFactor = controlledFactor;
        this.debug = debug;
        this.debugmessage = debugmessage;
    }
    //tempo massimo
    [SerializeField]
    protected float timeToMax;
    //tempo attuale dallo start
    protected float currentTimeSinceStart = 0f;
    //tempo allo start
    protected float StartTime = 0f;
    public virtual float CurrentTime { get { return currentTimeSinceStart; } }
    //curva da utilizzare
    public AnimationCurve controlledFactor;
    [SerializeField]
    protected bool debug;
    [SerializeField]
    protected string debugmessage;
    /// <summary>
    /// incrementa tempo DEPRECATA, usare refreshTime() invece!
    /// </summary>
    public virtual void updateTime()
    {
        currentTimeSinceStart += Time.deltaTime;
    }

    /// <summary>
    /// aggiorna il tempo corrente in base alla differenza tra tempo attuale e startTime
    /// </summary>
    public virtual void refreshTime()
    {
        currentTimeSinceStart = Time.time - StartTime;
    }
    /// <summary>
    /// ottiene il valore attuale della curva
    /// </summary>
    public virtual float CurrentValue
    {
        get
        {
            return controlledFactor.Evaluate(Mathf.Min(controlledFactor.duration(), currentTimeSinceStart / timeToMax));
        }
    }
    public void setOver() { currentTimeSinceStart = timeToMax; }
    public virtual bool isOver { get { return (currentTimeSinceStart / timeToMax) >= controlledFactor.duration(); } }
    /// <summary>
    /// resetta il timer
    /// </summary>
    public virtual void Reset()
    {
        currentTimeSinceStart = 0;
        StartTime = Time.time;
        if (timeToMax == 0f)
        {
            Debug.LogError("timerCurve.Reset: ATTENZIONE TIMETOMAX==0!!!");
        }
    }

    public virtual void printDebug()
    {
        if (debug)
            Debug.Log(debugmessage + " currentTimeSinceStart>" + currentTimeSinceStart + " timeToMax>" + timeToMax + " controlledFactor.duration()>" + controlledFactor.duration() + " normalized time>" + (currentTimeSinceStart / timeToMax));
    }

    public virtual timerCurve clone()
    {
        return new timerCurve(timeToMax, currentTimeSinceStart, StartTime, controlledFactor, debug, debugmessage);
    }

}
