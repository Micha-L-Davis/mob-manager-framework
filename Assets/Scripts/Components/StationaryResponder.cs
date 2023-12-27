using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryResponder : StateResponder
{
    public override void Respond(GameObject target)
    {
        MobileResponder mob = target.GetComponent<MobileResponder>();
        if (mob != null)
        {
            mob.SetDestination(transform.position);
        }
    }
}
