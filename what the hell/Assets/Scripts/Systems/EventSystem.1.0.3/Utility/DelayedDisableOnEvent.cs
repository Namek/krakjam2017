using System.Collections;
using UnityEngine;

public class DelayedDisableOnEvent : EventReactionScript
{
    [SerializeField]
    float delay;
    public override void reaction(object e)
    {
        base.reaction(e);
        StartCoroutine(afterTime());
    }

    IEnumerator afterTime()
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
