using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class UnityEventWithParameters<T> : UnityEvent<T>
{

}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool>
{

}