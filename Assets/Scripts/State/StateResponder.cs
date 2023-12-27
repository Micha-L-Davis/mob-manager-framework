using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateResponder : MonoBehaviour
{
    [SerializeField]
    internal ResponderSet _responderSet;

    public abstract void Respond(GameObject target);

    internal virtual void Awake()
    {
        _responderSet.Items.Add(this);
    }
}
