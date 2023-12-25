using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stationary : NeedyObject
{
    [SerializeField]
    Need _primaryNeed;
    [SerializeField]
    internal List<Need> _workerNeeds = new List<Need>();
    [SerializeField]
    internal ResolverSet _stationarySet;
    [SerializeField]
    float _defaultValue = 0.5f;

    public override bool ProvideResolution(Needs needyObject)
    {
        if (Vector3.Distance(this.transform.position, needyObject.transform.position) < 0.5f)
        {
            foreach (Need need in ResolvableNeeds)
            {
                needyObject.ReplenishNeed(need);
            }
            return true;
        }
        return false;
    }

    internal override void OnEnable()
    {
        base.OnEnable();

        _stationarySet.Items.Add(this);
    }


    internal override void Start()
    {
        base.Start();
        _needs.AddNeed(_primaryNeed, _defaultValue);
    }

    internal override void OnDisable()
    {
        base.OnDisable();

        _stationarySet.Items.Remove(this);
    }

    public override void SeekResolution(INeedResolver resolver)
    {
        Debug.Log($"{gameObject.name} needs to give the work need to {resolver.Transform.gameObject.name}!");
        // add the work need to resolver
        Needs resolverNeeds = resolver.Transform.gameObject.GetComponent<Needs>();
        if (resolverNeeds != null)
        {
            foreach (Need need in _workerNeeds)
            {
                resolverNeeds.AddNeed(need, 0.0f);
            }
        }
        // or maybe this does nothing here?
        // or perhaps here we check for a chain of needs--like, 
        // Notify a tool it needs to be carried here by the worker?
        //
    }


}
