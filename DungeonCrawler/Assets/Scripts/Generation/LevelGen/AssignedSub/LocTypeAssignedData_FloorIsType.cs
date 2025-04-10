using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct PairFloorType
{
    public ELocationType type;
    [Min(1)] public int floor;
}

[CreateAssetMenu(fileName = "FloorIsType", menuName = "ScriptableObjects/LevelGen/PreAssigned/FloorIsType")]
public class LocTypeAssignedData_FloorIsType : LocTypeAssignedDataBase
{
    [SerializeField] private List<PairFloorType> toAssign;
    
    public override void Assign(List<List<LocationData>> map)
    {
        foreach (PairFloorType pair in toAssign)
        {
            if (pair.floor - 1 >= map.Count)
                return;

            foreach (LocationData location in map[pair.floor - 1])
                location.locType = pair.type;
        }
    }
}
