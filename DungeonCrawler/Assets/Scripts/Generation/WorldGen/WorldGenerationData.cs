using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct LocationPrefab
{
    public ELocationType locationType;
    public GameObject locationPrefab;
}

[CreateAssetMenu(fileName = "WorldGenerationData", menuName = "ScriptableObjects/MapGen/WorldGenerationData")]
public class WorldGenerationData : ScriptableObject
{
    public List<LocationPrefab> prefabs = new();
    public float floorSpace;
    public float floorPosVariance;
    public float roomSpace;
    public float roomPosVariance;
}
