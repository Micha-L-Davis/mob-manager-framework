using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Needs))]
public class Workstation : Stationary
{
    [SerializeField]
    Need _workstationType;
    [SerializeField]
    float _defaultValue = 0.5f;

    private void Start()
    {
        Needs needs = GetComponent<Needs>();
        needs.AddNeed(_workstationType, _defaultValue);
    }
}
