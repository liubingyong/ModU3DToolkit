using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourEx : MonoBehaviour
{
    protected Coroutine Invoke (UnityAction action, float time)
    {
        return StartCoroutine (InvokeActionCore (action, time));
    }

    private IEnumerator InvokeActionCore (UnityAction action, float time)
    {
        yield return new WaitForSeconds (time);
        action.Invoke ();
    }

    protected Coroutine Invoke (UnityAction action, Func<bool> predicate)
    {
        return StartCoroutine (InvokeUntilCore (action, predicate));
    }

    private IEnumerator InvokeUntilCore (UnityAction action, Func<bool> predicate)
    {
        yield return new WaitUntil (predicate);
        action.Invoke ();
    }
}
