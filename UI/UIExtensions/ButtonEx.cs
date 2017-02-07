using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEx : Button
{
    public UnityEventWithParameters<Button> onSubmit = new UnityEventWithParameters<Button>();
    public UnityEventWithParameters<Button> onSelect = new UnityEventWithParameters<Button>();
    public UnityEventWithParameters<Button> onDeselect = new UnityEventWithParameters<Button>();

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelect.Invoke(this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        onDeselect.Invoke(this);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        onSubmit.Invoke(this);
    }
}
