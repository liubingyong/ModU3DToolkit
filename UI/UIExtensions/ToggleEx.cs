using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleEx : Toggle
{
    public UnityEventWithParameters<Toggle> onSubmit = new UnityEventWithParameters<Toggle>();
    public UnityEventWithParameters<Toggle> onSelect = new UnityEventWithParameters<Toggle>();
    public UnityEventWithParameters<Toggle> onDeselect = new UnityEventWithParameters<Toggle>();

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelect.Invoke(this);

        isOn = true;
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

        isOn = true;
    }
}
