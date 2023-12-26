using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSet<T> : ScriptableObject
{
	[SerializeField]
	List<T> _items = new List<T>();

	public List<T> Items { get { return _items; } }

	public T this[int index] { get { return _items[index]; } }
}
