using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIPopup))]
public abstract class BasePopup : MonoBehaviour
{
    protected UIPopup currentUIPopup;

    public virtual void Awake()
    {
        currentUIPopup = this.GetComponent<UIPopup>();
    }

    public virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && UIEnterExitTweens.activeTweensCounter == 0)
        {
            StartCoroutine(PopPopup());
        }
    }

    protected void OnShow(UIPopup popup)
    {
        popup.BroadcastMessage("StartEnterTweens", SendMessageOptions.DontRequireReceiver);
    }

    protected IEnumerator PopPopup()
    {
        this.BroadcastMessage("StartExitTweens", SendMessageOptions.DontRequireReceiver);

        while (UIEnterExitTweens.activeTweensCounter > 0)
        {
            yield return null;
        }

        Manager<UIManager>.Get().PopPopup();
        Manager<UIManager>.Get().ActivePage.BroadcastMessage("StartEnterTweens", SendMessageOptions.DontRequireReceiver);
    }
}
