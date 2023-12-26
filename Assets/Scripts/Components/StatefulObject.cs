using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulObject : MonoBehaviour
{
    [SerializeField]
    List<State> _states = new List<State>();

}
