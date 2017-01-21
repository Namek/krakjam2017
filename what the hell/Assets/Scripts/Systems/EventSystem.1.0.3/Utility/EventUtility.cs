using UnityEngine;

/// <summary>
/// holds the basic eventmanager functions and a eventHandlerManager reference.
/// Inherit to get a pre-implemented global\local switch
/// </summary>
public class EventUtility : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Leave to none to use the global event manager")]
    protected eventHandlerManager refManager;

    protected virtual void Broadcast(MonoBehaviour source, eventChannels channel, int ev, object e)
    {
        if (refManager == null)
            eventHandlerManager.globalBroadcast(source, channel, ev, e);
        else
            refManager.Broadcast(source, channel, ev, e);
    }
    protected virtual void AddListener(eventChannels channel, int ev, gameEventHandler function)
    {
        if (refManager == null)
            eventHandlerManager.globalAddListener(channel, ev, function);
        else
            refManager.AddListener(channel, ev, function);
    }
    protected virtual void RemoveListener(eventChannels channel, int ev, gameEventHandler function)
    {
        if (refManager == null)
            eventHandlerManager.globalRemoveListener(channel, ev, function);
        else
            refManager.RemoveListener(channel, ev, function);
    }
}
