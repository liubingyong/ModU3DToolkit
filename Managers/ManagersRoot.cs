using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ModU3DToolkit/Managers/ManagersRoot")]
public class ManagersRoot : MonoBehaviour
{
    #region Unity callbacks
    void Awake()
    {
        foreach (Transform child in transform)
            GameObject.DontDestroyOnLoad(child.gameObject);

        GameObject.DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Messages
    void OnLoadNewLevel()
    {
        transform.parent = null;
    }
    #endregion
}
