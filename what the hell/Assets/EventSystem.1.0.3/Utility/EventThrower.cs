using UnityEngine;

public class EventThrower : EventUtility
{
    [SerializeField]
    EventPicker pickEvent;
    public void doBroadcast()
    {
        //Debug.Log("-----" + pickEvent.Selected.ToString());
        Broadcast(this, pickEvent.channel, pickEvent.Selected, null);
    }
}
