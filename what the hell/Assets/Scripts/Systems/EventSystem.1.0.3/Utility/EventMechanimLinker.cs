using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EventMechanimLinker : EventUtility
{

    public List<eventData> eventMechanimList = new List<eventData>();


    [System.Serializable]
    public class eventData
    {
        public string triggerName;
        public EventPicker ActualEvent;
        public gameEventHandler listener;
    }
    public Animator animatorController;

    public void Reset()
    {
        animatorController = GetComponent<Animator>();
    }
    // Use this for initialization
    void Awake()
    {
        //Debug.Log("" + eventMechanimList.Count);
        foreach (var item in eventMechanimList)
        {
            string triggerName = item.triggerName;
            item.listener = new gameEventHandler(delegate (object e)
            {
                animatorController.SetTrigger(triggerName);
            });
            AddListener(item.ActualEvent.channel, item.ActualEvent.Selected, item.listener);
        }

    }
    void OnDestroy()
    {
        foreach (var item in eventMechanimList)
        {
            RemoveListener(item.ActualEvent.channel, item.ActualEvent.Selected, item.listener);
        }
    }
}
