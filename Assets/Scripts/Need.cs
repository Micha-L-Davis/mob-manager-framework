using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Need : ScriptableObject
{
	private float _value;
    public float Value 
	{ 
		get 
		{ 
			return _value; 
		} 

		set
		{
			_value += value;
			_value = Mathf.Clamp(_value, 0.0f, 1.0f);
		} 
	}


}
