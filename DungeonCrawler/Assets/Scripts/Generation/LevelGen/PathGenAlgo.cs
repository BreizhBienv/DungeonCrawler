using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ELocationType
{
    None,
    Encounter,
    EliteEncounter,
    Event,
    QuestGiver,
    RestSite,
    Treasure,
    Merchant,
}

public class LocationData
{
    public bool isGenerated = false;
    public GameObject gameObject;
    
    public int floorNumber;
    public int roomNumber;
    
    public List<LocationData> from = new();
    public List<LocationData> to = new();
    public ELocationType locType = ELocationType.None;

    public LocationData(int floorNumber, int roomNumber)
    {
        this.floorNumber = floorNumber;
        this.roomNumber = roomNumber;
    }
}

public static class PathGenAlgo
{
    private static LevelGenerationData pathsAlgoParameters;
    
    public static List<List<LocationData>> GeneratePaths(LevelGenerationData algoParameters)
    {
        pathsAlgoParameters = algoParameters;
        
        LocationData[,] generatedRooms = new LocationData[algoParameters.gridLength, algoParameters.gridWidth];
        
        for (int pathNum = 0; pathNum < algoParameters.numPathToGenerate; ++pathNum)
            GeneratePath(generatedRooms, 0, Random.Range(0, algoParameters.gridWidth), 0);

        List<List<LocationData>> map = new();
        for (int floorNum = 0; floorNum < algoParameters.gridLength; ++floorNum)
        {
            List<LocationData> floor = new();
            for (int roomNum = 0; roomNum < algoParameters.gridWidth; ++roomNum)
            {
                if (generatedRooms[floorNum, roomNum] is not null)
                    floor.Add(generatedRooms[floorNum, roomNum]);
            }
            map.Add(floor);
        }

        AssignLocationTypeToMap(map);
        
        return map;
    }

    private static void GeneratePath(LocationData[,] generatedRooms, int floor, int roomNum, int prevRoom)
    {
        if (floor > pathsAlgoParameters.gridLength - 1)
            return;

        //Generate location
        generatedRooms[floor, roomNum] ??= new LocationData(floor, roomNum);

        if (floor - 1 >= 0)
        {
            LocationData previousLoc = generatedRooms[floor - 1, prevRoom];
            
            //Add new generated location to the coming location data
            if (!previousLoc.to.Contains(generatedRooms[floor, roomNum]))
                previousLoc.to.Add(generatedRooms[floor, roomNum]);
            
            //Add coming from location to new location
            if (!generatedRooms[floor, roomNum].from.Contains(previousLoc))
                generatedRooms[floor, roomNum].from.Add(previousLoc);
        }
        
        //Compute next location to generate
        
        //Check if on bounds or is crossing path with neighbours
        int minNextRoom = NextLeftRoomAvailable(generatedRooms, floor, roomNum) ? roomNum - 1 : roomNum;
        int maxNextRoom = NextRightRoomAvailable(generatedRooms, floor, roomNum) ? roomNum + 1 : roomNum;
        
        //+ 1 because function is exclusive
        int nextFloor = floor + 1;
        int nextRoom = Random.Range(minNextRoom, maxNextRoom + 1);
        
        GeneratePath(generatedRooms, nextFloor, nextRoom, roomNum);
    }

    private static bool NextLeftRoomAvailable(LocationData[,] generatedRooms, int floor, int floorRoom)
    {
        if (floorRoom == 0)
            return false;
        
        if (generatedRooms[floor, floorRoom - 1] == null)
            return true;

        foreach (LocationData room in generatedRooms[floor, floorRoom - 1].to)
        {
            if (room.roomNumber == floorRoom)
                return false;
        }

        return true;
    }

    private static bool NextRightRoomAvailable(LocationData[,] generatedRooms, int floor, int floorRoom)
    {
        if (floorRoom == pathsAlgoParameters.gridWidth - 1)
            return false;
        
        if (generatedRooms[floor, floorRoom + 1] == null)
            return true;

        foreach (LocationData room in generatedRooms[floor, floorRoom + 1].to)
        {
            if (room.roomNumber == floorRoom)
                return false;
        }

        return true;
    }
    
    private static void AssignLocationTypeToMap(List<List<LocationData>> map)
    {
        foreach (LocTypeAssignedDataBase assigned in pathsAlgoParameters.preAssigned)
            assigned.Assign(map);
        
        foreach (List<LocationData> floor in map)
            foreach (LocationData location in floor)
                AssignLocationType(location);
    }

    private static void AssignLocationType(LocationData location)
    {
        if (location.locType != ELocationType.None)
            return;
        
        float accumulated = 0;
        List<ELocationType> assignable = new();
        List<float> odds = new();

        accumulated += ApplyIndividualRule(location, assignable, odds);
        accumulated += ApplyGlobalRule(location, assignable, odds);

        if (assignable.Count == 0)
        {
            location.locType = ELocationType.Encounter;
            return;
        }
        
        for (int i = 0; i < odds.Count; i++)
            odds[i] += accumulated / odds.Count;

        float random = Random.Range(0f, odds.Sum());

        accumulated = 0;
        for (int i = 0; i < odds.Count; i++)
        {
            if (accumulated <= random && random <= accumulated + odds[i])
            {
                location.locType = assignable[i];
                break;
            }

            accumulated += odds[i];
        }
    }

    private static float ApplyIndividualRule(LocationData location, List<ELocationType> assignable, List<float> odds)
    {
        float remainingOdd = 0;
        
        foreach (LocationTypeRules locTypeRules in pathsAlgoParameters.typesRules)
        {
            if (locTypeRules.IsTypeAssignable(location))
            {
                assignable.Add(locTypeRules.type);
                odds.Add(locTypeRules.odd);
            }
            else
                remainingOdd += locTypeRules.odd;
        }

        return remainingOdd;
    }

    private static float ApplyGlobalRule(LocationData location, List<ELocationType> assignable, List<float> odds)
    {
        float oddsRemoved = 0;
        
        List<ELocationType> toRemove = new();
        foreach (LocTypeRuleDataBase globalRule in pathsAlgoParameters.globalRules)
        {
            foreach (ELocationType type in assignable)
            {
                if (globalRule.IsRuleValid(location, type))
                    continue;
                
                if (!toRemove.Contains(type))
                    toRemove.Add(type);
            }
        }
        
        foreach (ELocationType type in toRemove)
        {
            int id = assignable.FindIndex(x => x == type);
            oddsRemoved += odds[id];
            odds.RemoveAt(id);
            assignable.RemoveAt(id);
        }

        return oddsRemoved;
    }
}