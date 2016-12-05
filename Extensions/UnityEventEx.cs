using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class UnityEventWithParameters<T> : UnityEvent<T>
{

}

public class UnityEventWithParameters<T1, T2> : UnityEvent<T1, T2>
{

}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool>
{

}

[System.Serializable]
public class UnityEventInt : UnityEvent<int>
{

}

[System.Serializable]
public class UnityEventFloat : UnityEvent<float>
{

}