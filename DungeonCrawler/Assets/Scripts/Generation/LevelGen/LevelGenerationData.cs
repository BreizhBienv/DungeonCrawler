using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public class LocationTypeRules
{
    public ELocationType type = ELocationType.None;
    [Range(0, 100)] public int odd;
    [SerializeField] private List<LocTypeRuleDataBase> rules = new();

    public bool IsTypeAssignable(LocationData location) => rules.All(rule => rule.IsRuleValid(location, type));
}

[CreateAssetMenu(fileName = "LevelGenerationData", menuName = "ScriptableObjects/LevelGen/LevelGenerationData")]
public class LevelGenerationData : ScriptableObject
{
    [Min(1)] public int gridWidth;
    [Min(1)] public int gridLength;
    [Min(1)] public int numPathToGenerate;

    public List<LocTypeAssignedDataBase> preAssigned = new();
    public List<LocTypeRuleDataBase> globalRules = new();
    public List<LocationTypeRules> typesRules = new();

    private void OnValidate()
    {
        List<ELocationType> duplicate = new();
        int totalOdds = 0;
        foreach (LocationTypeRules rule in typesRules)
        {
            if (duplicate.Contains(rule.type))
            {
                Debug.LogError("Two rule of same type detected");
                return;
            }
            
            duplicate.Add(rule.type);
            totalOdds += rule.odd;
        }
        
        if (totalOdds != 100)
            Debug.LogError("All odds does not add to a hundred");
    }
}