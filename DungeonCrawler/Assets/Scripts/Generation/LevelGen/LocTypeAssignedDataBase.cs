using System.Collections.Generic;
using UnityEngine;

public abstract class LocTypeAssignedDataBase : ScriptableObject
{
    public abstract void Assign(List<List<LocationData>> map);
}
