using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIListItem : MonoBehaviour
{
    [field: SerializeField]
    public Text NeedText { get; private set; }
    [field: SerializeField]
    public Text ValueText { get; private set; }
}
