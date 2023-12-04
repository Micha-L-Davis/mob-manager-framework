using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedResolver
{
    public List<Need> ResolvableNeeds { get; }

    public Transform Transform { get; }

    public bool IsAvailable { get; set; }

    public bool ResolveNeed(Needs needyObject);
}
