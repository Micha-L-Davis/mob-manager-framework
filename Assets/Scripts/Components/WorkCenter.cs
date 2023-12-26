using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkCenter : MonoBehaviour
{
    [SerializeField]
    List<WorkSlot> _workSlots = new List<WorkSlot>();
    [SerializeField]
    WorkSlotSet _workSlotSet;

    private void Awake()
    {
        foreach(WorkSlot slot in _workSlots)
        {
            _workSlotSet.Items.Add(slot);
        }
    }
}
