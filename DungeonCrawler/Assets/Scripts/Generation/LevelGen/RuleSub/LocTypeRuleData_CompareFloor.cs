using System;
using UnityEngine;

enum EInequality
{
    Below,
    Equal,
    NotEqual,
    Above,
}

[CreateAssetMenu(fileName = "CompareFloor", menuName = "ScriptableObjects/LevelGen/Rule/CompareFloor")]
public class LocTypeRuleData_CompareFloor : LocTypeRuleDataBase
{
    [SerializeField] private EInequality comparison;
    [SerializeField, Min(1)] private int floor;
    
    public override bool IsRuleValid(LocationData location, ELocationType type)
    {
        switch (comparison)
        {
            case EInequality.Below:
                return location.floorNumber < floor - 1;
            case EInequality.Equal:
                return location.floorNumber == floor - 1;
            case EInequality.NotEqual:
                return location.floorNumber != floor - 1;
            case EInequality.Above:
                return location.floorNumber > floor - 1;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
