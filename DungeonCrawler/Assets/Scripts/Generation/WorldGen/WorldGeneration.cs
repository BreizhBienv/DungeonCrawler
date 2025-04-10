using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private int seed;

    [Header("World")]
    [SerializeField] private LevelGenerationData levelGenerationData;
    [SerializeField] private WorldGenerationData worldGenerationData;
    
    private List<List<LocationData>> map = new();

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        Random.InitState(seed);
        map = PathGenAlgo.GeneratePaths(levelGenerationData);

        if (worldGenerationData.prefabs.Count == 0)
            return;

        foreach (List<LocationData> floor in map)
            foreach (LocationData room in floor)
                GenerateRoom(room);
    }

    private void GenerateRoom(LocationData location)
    {
        if (!location.isGenerated)
        {
            float xPos = location.roomNumber * worldGenerationData.roomSpace +
                         Random.Range(-worldGenerationData.roomPosVariance, worldGenerationData.roomPosVariance);
            float yPos = location.floorNumber * worldGenerationData.floorSpace +
                         Random.Range(-worldGenerationData.floorPosVariance, worldGenerationData.floorPosVariance);

            Vector3 pos = new(xPos, 0, yPos);

            GameObject prefab = worldGenerationData.prefabs.Find(x => x.locationType == location.locType).locationPrefab;
            if (prefab is not null)
            {
                GameObject newRoom = Instantiate(prefab, pos + Vector3.zero, Quaternion.identity);
                location.gameObject = newRoom;
            }

            location.isGenerated = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (List<LocationData> floor in map)
        {
            foreach (LocationData loc in floor)
            {
                if (loc.gameObject is null)
                    continue;

                foreach (LocationData nextLoc in loc.to)
                {
                    if (nextLoc.gameObject is null)
                        continue;
                    
                    Gizmos.DrawLine(loc.gameObject.transform.position, nextLoc.gameObject.transform.position);
                }
            }
        }
    }
}
