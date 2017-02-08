using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIVerticalScrollerItem : UIBehaviour, ISelectHandler
{
    private UIVerticalScroller uiVerticalScroller;

    protected override void Awake()
    {
        base.Awake();

        uiVerticalScroller = GetComponentInParent<UIVerticalScroller>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        uiVerticalScroller.OnItemSelect(this);
    }
}
