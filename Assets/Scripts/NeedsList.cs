using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Asset/Needs/Needs List")]
public class NeedsList : ScriptableObject
{
    public List<Need> Items = new List<Need>();
}
