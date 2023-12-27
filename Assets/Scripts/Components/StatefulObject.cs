using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulObject : MonoBehaviour
{
    [SerializeField]
    List<State> _states = new List<State>();



    private void Update()
    {
        // for each state,
        //  if state can restore:
        //      restore state
        //      return
        //  if state can degrade:
        //      degrade state
        //      return
    }


}
