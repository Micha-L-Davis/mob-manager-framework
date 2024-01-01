using UnityEngine;

[System.Serializable]
public struct State
{
    [SerializeField]
    public StateTaskEvent OnStateConditionChange;

    private int _priority;
    [SerializeField]
    public int Priority
    {
        get => _priority;
        set => _priority = value;
    }

    [SerializeField]
    StateVariable _stateVariable;
    public StateVariable Variable => _stateVariable;

    [SerializeField]
    private StateCondition _condition;
    public StateCondition Condition { get { return _condition; } set { _condition = value; } }

    [SerializeField]
    private float _cooldownInterval;
    [SerializeField]
    private float _decayCooldown;

    public void ProcessDecay()
    {
        if (Time.time > _decayCooldown)
        {
            _decayCooldown = Time.time + _cooldownInterval;
            Variable.Decay();
        }
    }

    public StateCondition CalculateCondition()
    {
        float value = Mathf.Floor(_stateVariable.Value / 0.25f + 0.5f) * 0.25f;
        value = value * 100;
        if (_stateVariable.Value < 0) value *= -1;

        return (StateCondition)value;
    }

}
