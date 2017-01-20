using System;
using System.Collections.Generic;
using UnityEngine;
/*
This eventHandlerManager supports registering/unregistering listeners for specific events and it's usable 
even in Awake functions since its actual initialization happens in Reset time through the InitializeDictionary static function
This class is usable both as a static reference and as a specific instance.
eventHandlerManager has also a debug feature that can be used to check on wether events 
are happening and where (if any) crashes are happening in a series of callbacks for the same event.
To debug the static instance through the inspector an instance of the script needs to be added in the scene but it's not needed for usage.
It's the script's responsibility to remove listeners when the script is destroyed, otherwise there will be an error in the callback chain that will
block the execution of the successive callbacks.
*/
public class eventHandlerManager : MonoBehaviour
{
    //platform-conditional compilation helps to avoid the debug overhead in the final build.
#if UNITY_EDITOR
    //each flag controls a different debug message
    public bool debug;//print debug when any event is broadcasted, unless debugSpecificEvent is true, in which case only that one event is debugged
    public bool debugAdds; //print debug when any listener is added
    public bool debugRemovals;//print debug when any listener is removed
    public bool debugSpecificEvent;//print debug when a specific event is broadcasted, after each callback, 
    public EventPicker picker; //component that gives the UI element to select the event to debug with dropdowns
    //invoked after a debugAll check
    bool shouldDebugEvent(int ev)
    {
        return (!debugSpecificEvent) || (ev == picker.Selected);
    }
    public static eventHandlerManager globalDebugController;
    public bool debugGlobally;//if true activates the debugging for the global eventHandlerManager
    /*
     in reaction to setting the debugGlobally variable, set the instance as 
    globalDebugController so that its debug fields are used for the static
    functions to control debugging, it also ensures that only one debugGlobally 
    variable can be set as true in all the instances of the eventHandlerManager
    */
    void OnValidate()
    {
        if (debugSpecificEvent)
            debug = true;
        if (debugGlobally)
        {
            eventHandlerManager temp = globalDebugController;
            globalDebugController = this;
            if (temp != null)
                if (temp.GetInstanceID() != this.GetInstanceID())
                    temp.debugGlobally = false;
        }
        else
        {
            if (globalDebugController != null)
                if (globalDebugController.GetInstanceID() == this.GetInstanceID())
                    globalDebugController = null;
        }
    }
#endif
    //these dictionaries host the callback delegates, they are initialized by the initializeDicts functions, so that they are already initialized when the first Awake is called.
    static Dictionary<int, Dictionary<int, GarbagelessList>> globalListenerFunctions = initializeDicts();
    Dictionary<int, Dictionary<int, GarbagelessList>> ListenerFunctions = initializeDicts();
    #region broadcast
    public static void globalBroadcast(MonoBehaviour source, eventChannels evType, int ev, object e)
    {
#if UNITY_EDITOR
        if (globalDebugController == null)
            executeBroadcast(false, false, source, evType, ev, e, globalListenerFunctions);
        else
#endif
            executeBroadcast(
#if UNITY_EDITOR
                globalDebugController.debug, globalDebugController.shouldDebugEvent(ev),
#endif
                 source, evType, ev, e, globalListenerFunctions);
    }
    public void Broadcast(MonoBehaviour source, eventChannels evType, int ev, object e)
    {
        executeBroadcast(
#if UNITY_EDITOR
                debug, shouldDebugEvent(ev),
#endif
                source, evType, ev, e, ListenerFunctions);
    }
    static void executeBroadcast(
#if UNITY_EDITOR
        bool debug, bool specific,
#endif
        MonoBehaviour source, eventChannels evType, int ev, object e, Dictionary<int, Dictionary<int, GarbagelessList>> target)
    {
#if UNITY_EDITOR
        //if the flags are true prints a list of what is going to be invoked before execution

        if (debug && specific)
        {
            Debug.Log("EventHandleManager Broadcast" + evType + " - " + ev + " calling " + target[(int)evType][ev].Count + " functions:");
            int i = 0;
            target[(int)evType][ev].Reset();
            while (target[(int)evType][ev].MoveNext())
            {
                Debug.Log(evType + " - " + ev + " [" + i + "](" + target[(int)evType][ev].Current.Method.DeclaringType.ToString() + ">" + target[(int)evType][ev].Current.ToString() + ")");
                ++i;
            }
            Debug.Log("EventHandleManager Broadcast - START");
        }

#endif
        //invoke event delegates
        target[(int)evType][ev].Reset();
        while (target[(int)evType][ev].MoveNext())
        {
            target[(int)evType][ev].Current(e);
        }
#if UNITY_EDITOR
        if (debug && specific)
        {
            Debug.Log("EventHandleManager Broadcast - OVER");
        }
#endif
    }
    #endregion
    #region AddListener
    public static void globalAddListener(eventChannels evType, int ev, gameEventHandler eventListener)
    {
#if UNITY_EDITOR
        if (globalDebugController == null)
            executeAddListener(false, false, false, evType, ev, eventListener, globalListenerFunctions);
        else
#endif
            executeAddListener(
#if UNITY_EDITOR
                globalDebugController.debug, globalDebugController.shouldDebugEvent(ev), globalDebugController.debugAdds,
#endif
                 evType, ev, eventListener, globalListenerFunctions);
    }
    public void AddListener(eventChannels evType, int ev, gameEventHandler eventListener)
    {
        executeAddListener(
#if UNITY_EDITOR
                debug, shouldDebugEvent(ev), debugAdds,
#endif
         evType, ev, eventListener, ListenerFunctions);
    }
    static void executeAddListener(
#if UNITY_EDITOR
                bool debug, bool specific, bool debugAdds,
#endif
         eventChannels evType, int ev, gameEventHandler eventListener, Dictionary<int, Dictionary<int, GarbagelessList>> target)
    {
        target[(int)evType][ev].Add(eventListener);
#if UNITY_EDITOR
        //if this event's execution should be logged, add a debugging delegate before the method
        if (debug && specific)
        {
            target[(int)evType][ev].Add(new gameEventHandler(delegate (object e)
             {
                 Debug.Log("EventHandleManager execution" + evType + " - " + ev +
                     " finished " + eventListener.Method.DeclaringType.ToString() + " >" + eventListener.Method.ToString());
             }));
        }
        if (debugAdds)
            Debug.Log("EventHandleManager Added event " + evType + " - " + ev +
                        " by " + eventListener.Method.DeclaringType.ToString() + " >" + eventListener.Method.ToString());
#endif
    }
    #endregion
    #region RemoveListener
    public static void globalRemoveListener(eventChannels evType, int ev, gameEventHandler eventListener)
    {
#if UNITY_EDITOR
        if (globalDebugController == null)
            executeRemoveListener(false, evType, ev, eventListener, globalListenerFunctions);
        else
#endif
            executeRemoveListener(
#if UNITY_EDITOR
                globalDebugController.debugRemovals,
#endif
                 evType, ev, eventListener, globalListenerFunctions);
    }
    public void RemoveListener(eventChannels evType, int ev, gameEventHandler eventListener)
    {
        executeRemoveListener(
#if UNITY_EDITOR
                debugRemovals,
#endif
                evType, ev, eventListener, ListenerFunctions);
    }
    static void executeRemoveListener(
#if UNITY_EDITOR
                bool debugRemovals,
#endif
        eventChannels evType, int ev, gameEventHandler eventListener, Dictionary<int, Dictionary<int, GarbagelessList>> target)
    {
#if UNITY_EDITOR
        if (debugRemovals)
            Debug.Log("EventHandleManager Removed event " + evType + " - " + ev +
                        " by " + eventListener.Method.DeclaringType.ToString() + " >" + eventListener.Method.ToString());
#endif
        target[(int)evType][ev].Remove(eventListener);
    }
    #endregion

    public void OnDestroy()
    {
        ListenerFunctions = initializeDicts();
    }

    /*
    this method initializes the ListenerFunctions dictionary by doubly indexing first by eventChannel and then by specific event. The initialization consists in an empity gameEventHandler to which new listeners will eventually be added with the AddListener functions
    */
    static Dictionary<int, Dictionary<int, GarbagelessList>> initializeDicts()
    {
        //gets the information on the structure of channels from ChannelEnums
        Dictionary<eventChannels, Array> enumChannelEventList = ChannelEnums.getChannelEnumList();
        Dictionary<int, Dictionary<int, GarbagelessList>> result = new Dictionary<int, Dictionary<int, GarbagelessList>>();
        foreach (var val in (eventChannels[])Enum.GetValues(typeof(eventChannels)))
        {
            result.Add((int)val, new Dictionary<int, GarbagelessList>());
            foreach (var ev in enumChannelEventList[val])
            {
                //adds an empity gameEventHandler for each event
                result[(int)val].Add((int)ev, new GarbagelessList());
            }
        }
        return result;
    }
}
//delegate signature for the callback functions
public delegate void gameEventHandler(object e);

