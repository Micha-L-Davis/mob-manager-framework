using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Asset/Needs List")]
public class NeedsList : ScriptableObject
{
    public List<CrewNeed> Items = new List<CrewNeed>();
}
