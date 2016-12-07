using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public Sprite indicatorSprite;

    public Vector2 padding;

    public float scale = 1f;

    public bool isActive = true;

    public Image IndicatorImage
    {
        get
        {
            return indicatorImage;
        }
    }

    #region Private

    private Image indicatorImage;
    private OffScreenIndicatorController controller;

    #endregion

    void Start()
    {
        if (!indicatorSprite)
        {
            Debug.LogError(" Please, specify the indicator sprite.");
        }
        GameObject indicatorImageObject = new GameObject("Indicator");
        indicatorImageObject.AddComponent<Image>();
        controller = OffScreenIndicatorController.Instance;
        if (!controller)
        {
            Destroy(gameObject);
            return;
        }
        indicatorImageObject.transform.SetParent(controller.transform);
        indicatorImage = indicatorImageObject.GetComponent<Image>();
        indicatorImage.sprite = indicatorSprite;
        indicatorImage.rectTransform.pivot = new Vector2(0.5f, 1);
        indicatorImage.rectTransform.localPosition = Vector3.zero;
        indicatorImage.rectTransform.localScale = Vector3.one * scale;
        indicatorImage.rectTransform.sizeDelta = new Vector2(indicatorSprite.rect.width, indicatorSprite.rect.height);
        indicatorImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        controller.CheckIn(this);
    }

    #region Custom methods

    public void Show()
    {
        indicatorImage.gameObject.SetActive(true);
    }

    public void Hide()
    {
        indicatorImage.gameObject.SetActive(false);
    }

    public bool IsVisible()
    {
        return indicatorImage.gameObject.activeSelf;
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public void SetLocalPos(Vector3 pos)
    {
        indicatorImage.rectTransform.localPosition = pos;
    }

    public void SetLocalRot(Quaternion rot)
    {
        indicatorImage.rectTransform.localRotation = rot;
    }

    public void SetOpacity(float opacity)
    {
        indicatorImage.color = new Color(1.0f, 1.0f, 1.0f, opacity);
    }

    #endregion
}
