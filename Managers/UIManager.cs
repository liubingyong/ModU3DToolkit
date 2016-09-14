using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using ModU3DToolkit.Core;
using ModU3DToolkit.Pool;

[AddComponentMenu("UI/UIManager")]
public class UIManager : Manager<UIManager>
{
    #region Singleton instance
    public static UIManager Instance
    {
        get
        {
            return Manager<UIManager>.Get();
        }
    }
    #endregion

    #region Public members
    public LayerMask uiLayers;
    //public int inputLayer = 0;

    public int baseScreenWidth = 1920;
    public int baseScreenHeight = 1080;
    public float pixelsPerUnit = 100.0f;

    [Range(1.0f, 100.0f)]
    public float maxDepth = 1.0f;

    public bool dontDestroyOnLoad = false;
    public bool disableUnityMouseEvents = true;
    public bool disableInputs = false;
    #endregion

    #region Protected members
    protected IntrusiveList<UIPage> pages = new IntrusiveList<UIPage>();
    protected IntrusiveList<UIPopup> popups = new IntrusiveList<UIPopup>();

    protected Stack<UIPopup> popupsStack = new Stack<UIPopup>();
/*
    protected uint nextFlyerID = 0;
    protected Dictionary<string, uint> flyerIDs = new Dictionary<string,uint>();
    protected Dictionary<uint, Pool<UIFlyer>> flyerPools = new Dictionary<uint,Pool<UIFlyer>>();
*/
    protected PoolCollection<UIFlyer> flyers;

    protected Camera uiCamera = null;
    protected TimeSource uiTimeSource = null;
    protected UIPage prevPage = null;
    protected UIPage activePage = null;
    protected Stack<UIPage> pagesStack = new Stack<UIPage>();

    protected Collider2D[] buttonsBuffer = new Collider2D[16];

    protected float prevScreenWidth = 0.0f;
    protected float prevScreenHeight = 0.0f;
    protected bool shouldDoLayout = false;
    #endregion

    #region Pages
    public UIPage ActivePage
    {
        get
        {
            return activePage;
        }
    }

    public UIPage PreviousPage
    {
        get
        {
            return prevPage;
        }
    }

    public string ActivePageName
    {
        get
        {
            return activePage != null ? activePage.name : string.Empty;
        }
    }

    public string PreviousPageName
    {
        get
        {
            return prevPage != null ? prevPage.name : string.Empty;
        }
    }
    
    public void AddPage(UIPage page)
    {
        pages.Add(page);
    }

    public void RemovePage(UIPage page)
    {
        pages.Remove(page);
    }

    public UIPage GetPage(string name)
    {
        foreach (UIPage page in pages)
        {
            if (page.name.Equals(name))
                return page;
        }
        return null;
    }

#if UNITY_EDITOR
    protected bool gotoPageGuard = false;
#endif
    public void GoToPage(string name)
    {
#if UNITY_EDITOR
        if (gotoPageGuard)
        {
            Debug.LogError("GoToPage called inside another GoToPage");
            return;
        }

        gotoPageGuard = true;
#endif
        prevPage = activePage;

        if (activePage != null)
        {
            activePage.onExit.Invoke(activePage);
            activePage.gameObject.SetActive(false);
            activePage = null;
        }
        foreach (UIPage page in pages)
        {
            if (page.name.Equals(name))
            {
                activePage = page;
                page.gameObject.SetActive(true);
                break;
            }
        }
#if UNITY_EDITOR
        gotoPageGuard = false;
#endif
    }

    public void GoToPage(UIPage page)
    {
#if UNITY_EDITOR
        if (gotoPageGuard)
        {
            Debug.LogError("GoToPage called inside another GoToPage");
            return;
        }

        gotoPageGuard = true;
#endif
        if (activePage != null)
        {
            activePage.onExit.Invoke(activePage);
            activePage.gameObject.SetActive(false);
            activePage = null;
        }
        foreach (UIPage otherPage in pages)
        {
            if (otherPage == page)
            {
                activePage = otherPage;
                otherPage.gameObject.SetActive(true);
                break;
            }
        }
#if UNITY_EDITOR
        gotoPageGuard = false;
#endif
    }
    #endregion

    #region Popups
    public int PopupsInStack
    {
        get
        {
            return popupsStack.Count;
        }
    }

    public UIPopup FrontPopup
    {
        get
        {
            if (popupsStack.Count > 0)
                return popupsStack.Peek();
            else
                return null;
        }
    }

    public void AddPopup(UIPopup popup)
    {
        popups.Add(popup);
    }

    public void RemovePopup(UIPopup popup)
    {
        this.RemovePopupFromStack(popup);

        popups.Remove(popup);
    }

    public bool IsPopupInStack(UIPopup popup)
    {
        return popupsStack.Contains(popup);
    }

    public bool IsPopupInStack(string name)
    {
        foreach (UIPopup popup in popups)
        {
            if (popup.name.Equals(name))
                return this.IsPopupInStack(popup);
        }
        return false;
    }

    public void PushPopup(UIPopup popup, bool deactivatePrevious = true)
    {
        if (popupsStack.Count > 0 && deactivatePrevious)
            popupsStack.Peek().gameObject.SetActive(false);

        popupsStack.Push(popup);

       if (popup.pausesGame)
            TimeManager.Instance.MasterSource.Pause();

        popup.gameObject.SetActive(true);
    }

    public void PushPopup(string name, bool deactivatePrevious=true)
    {
        foreach (UIPopup popup in popups)
        {
            if (popup.name.Equals(name))
            {
                this.PushPopup(popup, deactivatePrevious);
                break;
            }
        }
    }

    public void EnqueuePopup(UIPopup popup)
    {
        if (0 == popupsStack.Count)
        {
            this.PushPopup(popup);
            return;
        }

        UIPopup[] stack = popupsStack.ToArray();

        popupsStack.Clear();
        popupsStack.Push(popup);

        for (int i = stack.Length - 1; i >= 0; --i)
            popupsStack.Push(stack[i]);
    }

    public void EnqueuePopup(string name)
    {
        foreach (UIPopup popup in popups)
        {
            if (popup.name.Equals(name))
            {
                this.EnqueuePopup(popup);
                break;
            }
        }
    }

    public bool PopPopup()
    {
        if (popupsStack.Count > 0)
        {
            UIPopup popup = popupsStack.Peek();

            popup.gameObject.SetActive(false);
            popupsStack.Pop();

           if (popup.pausesGame)
                TimeManager.Instance.MasterSource.Resume();
            popup.onRemoveFromStack.Invoke(popup);

            if (popupsStack.Count > 0)
                popupsStack.Peek().gameObject.SetActive(true);

            return true;
        }
        return false;
    }

    public bool RemovePopupFromStack(UIPopup popup)
    {
        UIPopup[] stack = popupsStack.ToArray();
        int index = Array.IndexOf<UIPopup>(stack, popup);
        if (index >= 0)
        {
            popupsStack.Clear();
            for (int i = stack.Length - 1; i >= 0; --i)
            {
                if (i == index)
                    continue;

                popupsStack.Push(stack[i]);
            }

            popup.gameObject.SetActive(false);
            if (popup.pausesGame)
                TimeManager.Instance.MasterSource.Resume();
            popup.onRemoveFromStack.Invoke(popup);

            if (0 == index && popupsStack.Count > 0)
                popupsStack.Peek().gameObject.SetActive(true);

            return true;
        }
        return false;
    }

    public bool RemovePopupFromStack(string name)
    {
        foreach (UIPopup popup in popups)
        {
            if (popup.name.Equals(name))
                return this.RemovePopupFromStack(popup);
        }
        return false;
    }

    public bool BringPopupToFront(UIPopup popup)
    {
        UIPopup[] stack = popupsStack.ToArray();
        int index = Array.IndexOf<UIPopup>(stack, popup);
        if (index >= 0)
        {
            if (0 == index)
                return true;

            popupsStack.Peek().gameObject.SetActive(false);

            popupsStack.Clear();
            for (int i = stack.Length - 1; i >= 0; --i)
            {
                if (i == index)
                    continue;

                popupsStack.Push(stack[i]);
            }

            popupsStack.Push(popup);
            popup.gameObject.SetActive(true);

            return true;
        }
        return false;
    }

    public bool BringPopupToFront(string name)
    {
        foreach (UIPopup popup in popups)
        {
            if (popup.name.Equals(name))
                return this.BringPopupToFront(popup);
        }
        return false;
    }

    public void ClearPopups()
    {
        while (this.PopPopup()) ;
    }
    #endregion

    #region Flyers
    public void RegisterFlyerPrefab(UIFlyer flyerPrefab, int poolSize)
    {/*
#if UNITY_EDITOR
        if (PrefabUtility.GetPrefabType(flyerPrefab) != PrefabType.Prefab)
        {
            Debug.LogError("Flyer \"" + flyerPrefab.name + "\" is not a Prefab!", flyerPrefab);
            return;
        }

        if (flyerIDs.ContainsKey(flyerPrefab.name))
        {
            Debug.LogError("Flyer \"" + flyerPrefab.name + "\" already exist!", flyerPrefab);
            return;
        }
#endif
        flyerIDs.Add(flyerPrefab.name, nextFlyerID);
        flyerPools.Add(nextFlyerID, new Pool<UIFlyer>(flyerPrefab, true, poolSize));

        ++nextFlyerID;*/
        flyers.RegisterPrefab(flyerPrefab, poolSize);
    }

    public void RegisterFlyerPrefab(UIFlyer flyerPrefab)
    {
        //this.RegisterFlyerPrefab(flyerPrefab, 0);
        flyers.RegisterPrefab(flyerPrefab);
    }

    public void RegisterFlyer(UIFlyer flyer, int poolSize)
    {/*
#if UNITY_EDITOR
        if (PrefabType.Prefab == PrefabUtility.GetPrefabType(flyer))
        {
            Debug.LogError("Flyer \"" + flyer.name + "\" is not an instance!", flyer);
            return;
        }

        if (flyerIDs.ContainsKey(flyer.name))
        {
            Debug.LogError("Flyer \"" + flyer.name + "\" already exist!", flyer);
            return;
        }
#endif
        flyerIDs.Add(flyer.name, nextFlyerID);
        flyerPools.Add(nextFlyerID, new Pool<UIFlyer>(flyer, false, poolSize));

        ++nextFlyerID;*/
        flyers.RegisterInstance(flyer, poolSize);
    }

    public void RegisterFlyer(UIFlyer flyer)
    {
        //this.RegisterFlyer(flyer, 0);
        flyers.RegisterInstance(flyer);
    }

    public uint GetFlyerID(string flyerName)
    {/*
        uint hash;
        if (flyerIDs.TryGetValue(flyerName, out hash))
            return hash;
        return uint.MaxValue;*/
        return flyers.GetOriginalID(flyerName);
    }

    public uint GetFlyerID(UIFlyer flyer)
    {
        //return this.GetFlyerID(flyer.name);
        return flyers.GetOriginalID(flyer);
    }

    public IntrusiveList<UIFlyer> GetActiveFlyers(uint flyerID)
    {/*
#if UNITY_EDITOR
        if (!flyerPools.ContainsKey(flyerID))
        {
            Debug.LogError("Flyer ID" + flyerID + " doesn't exist!");
            return null;
        }
#endif
        return flyerPools[flyerID].UsedList;*/
        return flyers.GetActiveList(flyerID);
    }

    public IntrusiveList<UIFlyer> GetActiveFlyers(string flyerName)
    {
        //return this.GetActiveFlyers(this.GetFlyerID(flyerName));
        return flyers.GetActiveList(flyerName);
    }

    public IntrusiveList<UIFlyer> GetActiveFlyers(UIFlyer flyer)
    {
        //return this.GetActiveFlyers(this.GetFlyerID(flyer));
        return flyers.GetActiveList(flyer);
    }

    public UIFlyer PlayFlyer(uint flyerID, Vector3 position, Quaternion rotation)
    {/*
#if UNITY_EDITOR
        if (!flyerPools.ContainsKey(flyerID))
        {
            Debug.LogError("Flyer ID" + flyerID + " doesn't exist!");
            return null;
        }
#endif
        UIFlyer flyer = flyerPools[flyerID].Get();
        flyer.transform.position = transform.TransformPoint(position);
        flyer.transform.rotation = transform.rotation * rotation;
        flyer.Play();

        return flyer;*/
        UIFlyer flyer = flyers.Instantiate(flyerID);
        flyer.transform.position = transform.TransformPoint(position);
        flyer.transform.rotation = transform.rotation * rotation;
        flyer.Play();
        return flyer;
    }

    public UIFlyer PlayFlyer(string flyerName, Vector3 position, Quaternion rotation)
    {
        return this.PlayFlyer(this.GetFlyerID(flyerName), position, rotation);
    }

    public UIFlyer PlayFlyer(UIFlyer flyer, Vector3 position, Quaternion rotation)
    {
        return this.PlayFlyer(this.GetFlyerID(flyer), position, rotation);
    }

    protected void UpdateFlyers()
    {
#if SBS_PROFILER
        Profiler.BeginSample("UpdateFlyers");
#endif
        /*foreach (KeyValuePair<uint, Pool<UIFlyer>> item in flyerPools)
        {
            UIFlyer node = item.Value.UsedList.Head;
            while (node != null)
            {
                node.UpdateFlyer();

                node = node.next;
            }
        }*/
        foreach (UIFlyer flyer in flyers)
            flyer.UpdateFlyer();
#if SBS_PROFILER
        Profiler.EndSample();
#endif
    }
    #endregion

    #region Camera
    public Camera UICamera
    {
        get
        {
            return uiCamera;
        }
    }

    protected void CreateCamera()
    {
        GameObject camGO = GameObject.Find("uiCamera_" + name);
        if (camGO != null)
            uiCamera = camGO.GetComponent<Camera>();
        if (null == uiCamera)
        {
            GameObject go = new GameObject("uiCamera_" + name);
            go.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy | HideFlags.DontSave;
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;

            if (dontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(go);

            uiCamera = /*gameObject*/go.AddComponent<Camera>();

            uiCamera.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy | HideFlags.DontSave;
            uiCamera.useOcclusionCulling = false;
            uiCamera.hdr = false;
            uiCamera.orthographic = true;
            uiCamera.clearFlags = CameraClearFlags.Depth;
            uiCamera.depth = float.MaxValue;
            uiCamera.nearClipPlane = 0.0f;
            //uiCamera.farClipPlane = 1.0f;

            if (disableUnityMouseEvents)
            {
                foreach (Camera cam in Camera.allCameras)
                    cam.eventMask = 0;
            }
            else
                uiCamera.eventMask = 0;
        }

        uiCamera.cullingMask = uiLayers;
        uiCamera.orthographicSize = baseScreenHeight * 0.5f / pixelsPerUnit;
        uiCamera.farClipPlane = maxDepth;
    }

    protected void DestroyCamera()
    {
        if (uiCamera != null)
        {
            Camera.DestroyImmediate(uiCamera.gameObject);
            uiCamera = null;
        }
    }

    protected Vector3 ScreenToWorld(Vector3 screenPos)
    {
        return uiCamera.ScreenToWorldPoint(screenPos);/*
        Vector3 clipPos = new Vector3((screenPos.x * 2.0f / Screen.width ) - 1.0f,
                                      (screenPos.y * 2.0f / Screen.height) - 1.0f,
                                      uiCamera.nearClipPlane);
        clipPos.y *= uiCamera.orthographicSize;
        clipPos.x *= (uiCamera.orthographicSize * uiCamera.aspect);
        return uiCamera.cameraToWorldMatrix * clipPos;*/
    }

    protected void SetupCamera()
    {
#if UNITY_EDITOR
        uiCamera.cullingMask = uiLayers;
        uiCamera.orthographicSize = baseScreenHeight * 0.5f;
        uiCamera.farClipPlane = maxDepth;

        if (Application.isPlaying)
            uiCamera.ResetAspect();
        else
            uiCamera.aspect = baseScreenWidth / (float)baseScreenHeight;
#else
        uiCamera.farClipPlane = maxDepth;

        uiCamera.ResetAspect();
#endif
    }
    #endregion

    #region TimeSource
    public TimeSource UITimeSource
    {
        get
        {
            return uiTimeSource;
        }
    }
    #endregion

    #region Unity callbacks
    void OnEnable()
    {
        this.CreateCamera();
    }

    void OnDisable()
    {
        this.DestroyCamera();
    }

    new void Awake()
    {
        base.Awake();
        
        flyers.Initialize();

        if (dontDestroyOnLoad)
            GameObject.DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
#if UNITY_EDITOR
        gotoPageGuard = false;
#endif

        uiTimeSource = new TimeSource();
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            TimeManager.Instance.AddSource(uiTimeSource);
        }
    }

    void Update()
    {
        this.SetupCamera();

#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            if (uiCamera.pixelWidth != prevScreenWidth || uiCamera.pixelHeight != prevScreenHeight)
            {
                prevScreenWidth = uiCamera.pixelWidth;
                prevScreenHeight = uiCamera.pixelHeight;
                shouldDoLayout = true;
            }
        }
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
           
        }
    }

    void OnLevelWasLoaded(int level)
    {
        if (disableUnityMouseEvents)
        {
            foreach (Camera cam in Camera.allCameras)
                cam.eventMask = 0;
        }
        else
            uiCamera.eventMask = 0;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (uiCamera != null)
        {
            Gizmos.DrawWireCube(
                transform.TransformPoint(Vector3.forward * (uiCamera.nearClipPlane + uiCamera.farClipPlane) * 0.5f),
                new Vector3(2.0f * uiCamera.orthographicSize * uiCamera.aspect, 2.0f * uiCamera.orthographicSize, uiCamera.farClipPlane - uiCamera.nearClipPlane));

            //Gizmos.DrawSphere(this.ScreenToWorld(Input.mousePosition), 0.1f);
        }
    }
#endif
    #endregion
}
