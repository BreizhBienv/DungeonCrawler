using UnityEngine;

[CreateAssetMenu(fileName = "UniquePath", menuName = "ScriptableObjects/LevelGen/Rule/UniquePath")]
public class LocTypeRuleData_UniquePath : LocTypeRuleDataBase
{
    public override bool IsRuleValid(LocationData location, ELocationType type)
    {
        //for each location this location is coming from
        foreach (LocationData from in location.from)
        {
            //for each destination
            foreach (LocationData to in from.to)
            {
                if (to == location || to.locType == ELocationType.None)
                    continue;

                if (type == to.locType)
                    return false;
            }
        }

        return true;
    }
}
