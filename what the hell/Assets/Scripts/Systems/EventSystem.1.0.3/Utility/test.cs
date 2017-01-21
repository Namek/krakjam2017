
#if UNITY_EDITOR
using UnityEngine;

public class test : MonoBehaviour
{
    public bool broadcast;
    public EventPicker pick;

    void Start()
    {
        Debug.Log("add" + eventChannels.inGame + "pick.Selected" + (int)inGameChannelEvents.gameStart);
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameStart, testFunction);
        Debug.Log("add" + eventChannels.inGame + "pick.Selected" + (int)inGameChannelEvents.gameOver);
        eventHandlerManager.globalAddListener(eventChannels.inGame, (int)inGameChannelEvents.gameOver, testFunction);
    }

    public void Update()
    {
        if (broadcast)
        {
            Debug.Log("call " + pick.channel + " pick.Selected " + pick.Selected);
            eventHandlerManager.globalBroadcast(this, pick.channel, pick.Selected, null);

            broadcast = false;
        }
    }

    // Update is called once per frame
    void testFunction(object e)
    {
        Debug.Log("broadcastExecuted");
    }
}

#endif