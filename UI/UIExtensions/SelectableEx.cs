using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableEx : Selectable, IPointerClickHandler
{
    public override void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        base.OnPointerUp(eventData);
    }
}
