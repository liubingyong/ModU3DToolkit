using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIPage))]
public abstract class BasePage : MonoBehaviour {
	protected UIPage currentUIPage;

	public virtual void Awake() {
		currentUIPage = this.GetComponent<UIPage> ();
	}

	protected IEnumerator GoToNextPage(string nextPage)
	{
		Manager<UIManager>.Get().ActivePage.BroadcastMessage("StartExitTweens", SendMessageOptions.DontRequireReceiver);

		while (UIEnterExitTweens.activeTweensCounter > 0)
		{
			yield return null;
		}

		if (nextPage.EndsWith("Popup")) {
			Manager<UIManager>.Get().PushPopup(nextPage);
		} else
		{
			Manager<UIManager>.Get().GoToPage(nextPage);
		}
	}
}
