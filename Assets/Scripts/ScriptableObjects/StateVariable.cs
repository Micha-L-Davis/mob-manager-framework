using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Variable/StateVariable")]
public class StateVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    [Range(-1f, 1f)]
    public float Value = 0;

    [SerializeField]
    private float _baseDecay;
    [SerializeField]
    private float _baseRestore;


    public void SetValue(float value)
    {
        Value = Mathf.Clamp(value, -1.0f, 1.0f);
    }

    public void ApplyChange(float amount)
    {
        float v = Value;
        v += amount;
        Value = Mathf.Clamp(v, -1.0f, 1.0f);
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
