using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ConcreteUnityEvent<T> : UnityEvent<T>
{

}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool>
{

}