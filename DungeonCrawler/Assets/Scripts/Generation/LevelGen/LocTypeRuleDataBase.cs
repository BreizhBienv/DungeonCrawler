using UnityEngine;

public abstract class LocTypeRuleDataBase : ScriptableObject
{
    public abstract bool IsRuleValid(LocationData location, ELocationType type);
}