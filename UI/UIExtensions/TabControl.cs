using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[Serializable]
public class TabItem
{
    [SerializeField]
    private ToggleEx _header = null;
    public ToggleEx header { get { return _header; } }

    [SerializeField]
    private GameObject _panel = null;
    public GameObject panel { get { return _panel; } }
}

public class TabControl : UIBehaviour
{
    public int defaultTabIndex = 0;

    [SerializeField]
    private List<TabItem> entries = null;

    protected override void Start()
    {
        base.Start();

        foreach (TabItem entry in entries)
        {
            AddButtonListener(entry);
        }

        if (entries.Count > defaultTabIndex)
        {
            SelectTab(entries[defaultTabIndex]);
        }
    }

    public void AddEntry(TabItem entry)
    {
        entries.Add(entry);
    }

    private void AddButtonListener(TabItem entry)
    {
        entry.header.onSelect.AddListener(x => SelectTab(entry));

        //entry.header.onClick.AddListener(() => SelectTab(entry));
    }

    private void SelectTab(TabItem selectedEntry)
    {
        foreach (TabItem entry in entries)
        {
            bool isSelected = entry == selectedEntry;

            if (isSelected)
                entry.header.Select();

            entry.panel.SetActive(isSelected);
        }
    }
}
