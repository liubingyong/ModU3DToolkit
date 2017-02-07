using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using ModU3DToolkit.Core;
using UnityEngine.SceneManagement;

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

    public void PushPopup(string name, bool deactivatePrevious = true)
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
        
    }

    void OnDisable()
    {
        
    }

    new void Awake()
    {
        base.Awake();

        SceneManager.sceneLoaded += SceneLoaded;

        if (dontDestroyOnLoad)
            GameObject.DontDestroyOnLoad(gameObject);
    }

    new void Start()
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
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {

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

    //void OnLevelWasLoaded(int level)
    //{
    //    if (disableUnityMouseEvents)
    //    {
    //        foreach (Camera cam in Camera.allCameras)
    //            cam.eventMask = 0;
    //    }
    //    else
    //        uiCamera.eventMask = 0;
    //}

    void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (disableUnityMouseEvents)
        {
            foreach (Camera cam in Camera.allCameras)
                cam.eventMask = 0;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

    }
#endif
    #endregion
}
