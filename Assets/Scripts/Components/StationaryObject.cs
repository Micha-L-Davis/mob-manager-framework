using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObject : MonoBehaviour, IStateRespondable
{
    public void Respond(GameObject target)
    {
        MobileObject mob = target.GetComponent<MobileObject>();
        if (mob != null)
        {
            mob.SetDestination(transform.position);
        }
    }
}
