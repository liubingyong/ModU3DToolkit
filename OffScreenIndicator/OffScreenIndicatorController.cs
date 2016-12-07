using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class OffScreenIndicatorController : MonoBehaviour
{
    #region Singleton
    public static OffScreenIndicatorController Instance
    {
        get
        {
            if (!_instance)
            {
                OffScreenIndicatorController[] controllers = GameObject.FindObjectsOfType<OffScreenIndicatorController>();

                if (controllers.Length != 0)
                {
                    if (controllers.Length == 1)
                    {
                        _instance = controllers[0];
                    }
                    else
                    {
                        Debug.LogError("You have more than one OffScreenIndicatorController in the scene.");
                    }
                }
                else
                {
                    Debug.LogError("You should add OffScreenIndicatorController prefab to your canvas");
                }


            }


            return _instance;
        }
    }

    private static OffScreenIndicatorController _instance;
    #endregion

    #region Public

    public Vector2 padding = Vector2.zero;

    #endregion


    #region Private

    private RectTransform mapRect;

    private float screenSlopAngel;

    #endregion

    #region Unity Methods

    void Awake()
    {
        mapRect = GetComponent<RectTransform>();
        screenSlopAngel = Mathf.Atan2(Screen.height, Screen.width);
    }

    void Update()
    {

    }

    #endregion

    #region Custom methods

    public void CheckIn(OffScreenIndicator indicator)
    {
        if (indicator.isActive)
        {
            var screenPos = Camera.main.WorldToScreenPoint(indicator.transform.position);

            if (screenPos.x > 0 && screenPos.x < Screen.width
                && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                // ON SCREEN

                if (indicator.IsVisible())
                    indicator.Hide();
            }
            else
            {
                // OFF SCREEN
                indicator.Show();

                var screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2f;

                screenPos -= screenCenter;

                var slope = screenPos.y / screenPos.x;
                var angel = Mathf.Atan2(screenPos.y, screenPos.x);

                if (angel < 0)
                {
                    angel += 2 * Mathf.PI;
                }

                if (angel >= 2 * Mathf.PI - screenSlopAngel || angel <= screenSlopAngel)
                {
                    screenPos.x = Screen.width / 2;
                    screenPos.y = slope * screenPos.x;
                }
                else if (angel >= screenSlopAngel && angel <= Mathf.PI - screenSlopAngel)
                {
                    screenPos.y = Screen.height / 2;
                    screenPos.x = screenPos.y / slope;
                }
                else if (angel >= Mathf.PI - screenSlopAngel && angel <= Mathf.PI + screenSlopAngel)
                {
                    screenPos.x = -Screen.width / 2;
                    screenPos.y = slope * screenPos.x;
                }
                else if (angel >= Mathf.PI + screenSlopAngel && angel <= 2 * Mathf.PI - screenSlopAngel)
                {
                    screenPos.y = -Screen.height / 2;
                    screenPos.x = screenPos.y / slope;
                }

                indicator.SetLocalPos(new Vector3(screenPos.x - padding.x - indicator.padding.x, screenPos.y - padding.y - indicator.padding.y, 0));
                indicator.SetLocalRot(Quaternion.Euler(0, 0, (angel - Mathf.PI / 2) * Mathf.Rad2Deg));
            }
        }
        else
        {
            if (indicator.IsVisible())
                indicator.Hide();
        }
    }

    #endregion
}
