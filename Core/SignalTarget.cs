using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModU3DToolkit.Core
{
    [Serializable]
    public class SignalTarget
    {
        public GameObject gameObject;
        public string method;

        public SignalTarget()
        { }

        public SignalTarget(GameObject _gameObject, string _method)
        {
            gameObject = _gameObject;
            method = _method;
        }
    }
}
