using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIVerticalScroller : MonoBehaviour
{
    private ScrollRect scrollRect;

    private float currentScrollbarValue = 1;
    private float targetScrollbarValue = 1;
    private float scrollDelta = 0;

    private float duration = 0.2f;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollDelta < 1)
        {
            scrollRect.verticalScrollbar.value = Mathf.MoveTowards(currentScrollbarValue, targetScrollbarValue, scrollDelta);
            scrollDelta += Time.deltaTime / duration;
        }
    }

    internal void OnItemSelect(UIVerticalScrollerItem item)
    {
        var totalHeight = scrollRect.content.rect.size.y;
        var viewHeight = scrollRect.content.rect.size.y * scrollRect.verticalScrollbar.size;

        var minValue = 0 + (1 - scrollRect.verticalScrollbar.value) * (totalHeight - viewHeight);
        var maxValue = viewHeight + (1 - scrollRect.verticalScrollbar.value) * (totalHeight - viewHeight);

        if (maxValue > totalHeight)
        {
            maxValue = totalHeight;
            minValue = totalHeight - viewHeight;
        }

        var itemRectTransform = item.GetComponent<RectTransform>();
        var topY = Mathf.Abs(itemRectTransform.anchoredPosition.y) - itemRectTransform.sizeDelta.y / 2;
        var bottomY = Mathf.Abs(itemRectTransform.anchoredPosition.y) + itemRectTransform.sizeDelta.y / 2;

        if ((topY > minValue && topY < maxValue) && (bottomY > minValue && bottomY < maxValue))
        {
            // ON Screen
        }
        else
        {
            // OFF Screen

            if (bottomY < viewHeight)
            {
                targetScrollbarValue = 1;
            }
            else if (topY > totalHeight - viewHeight)
            {
                targetScrollbarValue = 0;
            }
            else
            {
                var currentTopY = (totalHeight - viewHeight) * (1 - scrollRect.verticalScrollbar.value) + scrollRect.content.GetComponent<GridLayoutGroup>().padding.top;

                if (topY > currentTopY)
                {
                    targetScrollbarValue = 1 - ((topY - scrollRect.content.GetComponent<GridLayoutGroup>().padding.top) / (totalHeight - viewHeight));
                }
                else
                {
                    targetScrollbarValue = 1 - ((bottomY + scrollRect.content.GetComponent<GridLayoutGroup>().padding.bottom - viewHeight) / (totalHeight - viewHeight));
                }
            }

            scrollDelta = 0;
            currentScrollbarValue = scrollRect.verticalScrollbar.value;
        }
    }
}
