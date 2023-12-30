using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkSlot
{
    public bool isAvailable;
    public float readiness;
    public float laborCooldown;

    public WorkSlot(float laborCooldown)
    {
        isAvailable = true;
        readiness = -1.0f;
        this.laborCooldown = laborCooldown;
    }

    public void ApplyLabor(StateVariable state, float value)
    {
        if (readiness < Time.time)
        {
            readiness = Time.time + laborCooldown;

            state.ApplyChange(value);
        }
    }


}
