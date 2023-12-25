using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NeedsUI : MonoBehaviour
{
    [SerializeField]
    NeedyObject _needyObject;
    Needs _needsComponent;

    [SerializeField]
    Text _nameText;
    [SerializeField]
    Text _rankText;
    [SerializeField]
    Text _occupationText;

    [SerializeField]
    GameObject _needsListUI;
    [SerializeField]
    GameObject _listItemPrefab;

    [SerializeField]
    Canvas _canvas;

    private void Awake()
    {
        SetBioText($"Name: {_needyObject.name}", "...", "...");
        _needsComponent = _needyObject.GetComponent<Needs>();
    }

    private void Update()
    {
        SetNeeds();
    }

    public void ToggleUI()
    {
        _canvas.enabled = !_canvas.enabled;
    }

    void SetBioText(string lineA, string lineB, string lineC)
    {
        _nameText.text = lineA;
        //_rankText.text = _mobile.Rank;
        _rankText.text = lineB;
        //_occupationText.text = _mobile.Occupation
        _occupationText.text = lineC;
    }

    void SetNeeds()
    {
        foreach (Transform item in _needsListUI.transform)
        {
            Destroy(item.gameObject);
        }

        // Write priority need to the list first, if it exists
        if (_needsComponent.PriorityNeed != null)
        {
            GameObject listItem = GameObject.Instantiate(_listItemPrefab, _needsListUI.transform);
            UIListItem uiListItem = listItem.GetComponent<UIListItem>();
            if (uiListItem != null)
            {
                uiListItem.NeedText.text = _needsComponent.PriorityNeed.name;
                uiListItem.ValueText.text = _needsComponent.NeedsDictionary[_needsComponent.PriorityNeed].ToString();
            }
        }

        foreach (KeyValuePair<Need, float> need in _needsComponent.NeedsDictionary.OrderBy(n => n.Key.Priority))
        {
            if (need.Key == _needsComponent.PriorityNeed) continue;

            GameObject listItem = GameObject.Instantiate(_listItemPrefab, _needsListUI.transform);
            UIListItem uiListItem = listItem.GetComponent<UIListItem>();
            if (uiListItem != null)
            {
                uiListItem.NeedText.text = need.Key.name;
                uiListItem.ValueText.text = need.Value.ToString();
            }
        }
    }
}
