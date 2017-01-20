using UnityEngine;

public class EventReactionScript : EventUtility
{
    [SerializeField]
    EventPicker reactTo;
    [SerializeField]
    bool debug;

    public void Awake()
    {
        AddListener(reactTo.channel, reactTo.Selected, reaction);

    }
    public virtual void reaction(object e)
    {
        if (debug)
        {
            Debug.Log("EventReactionScript reaction to " + reactTo.channel + "/" + reactTo.Selected);
        }
    }

    public void OnDestroy()
    {
        RemoveListener(reactTo.channel, reactTo.Selected, reaction);
    }
}
