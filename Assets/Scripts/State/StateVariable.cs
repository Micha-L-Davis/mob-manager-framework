using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Variable/StateVariable")]
public class StateVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string StateDescription = "";
#endif
    [Range(-1.0f, 1.0f), SerializeField]
    private float _value = 0;
    public float Value
    {
        get => _value;

        private set
        {
            _value = Mathf.Clamp(value, -1.0f, 1.0f);
        }
    }

    [SerializeField]
    private float _baseDecay;
    [SerializeField]
    private float _baseRestore;


    public void SetValue(float value)
    {
        Value = value;
    }

    public void ApplyChange(float amount)
    {
        Value += amount;
    }

    public void Decay(float additionalDecay = 0, float decayMultiplier = 1)
    {
        float amount = (_baseDecay + additionalDecay) * decayMultiplier;
        ApplyChange(-amount);
    }

    public void Restore(float additionalRestore = 0, float restoreMultiplier = 1)
    {
        float amount = (_baseRestore + additionalRestore) * restoreMultiplier;
        ApplyChange(amount);
    }
}