using UnityEngine;

[CreateAssetMenu(fileName = "NotConsecutive", menuName = "ScriptableObjects/LevelGen/Rule/NotConsecutive")]
public class LocTypeRuleData_NotConsecutive : LocTypeRuleDataBase
{
    public override bool IsRuleValid(LocationData location, ELocationType type)
    {
        if (type == ELocationType.None)
            return false;
        
        foreach (LocationData from in location.from)
        {
            if (from.locType != ELocationType.None && from.locType == type)
                return false;
        }
        
        foreach (LocationData to in location.to)
        {
            if (to.locType != ELocationType.None && to.locType == type)
                return false;
        }

        return true;
    }
}