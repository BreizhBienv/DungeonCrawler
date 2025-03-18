using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Algo.MapGeneration
{
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
    
    public class RoomData
    {
        public bool IsGenerated = false;
        public GameObject GameObject;
        
        public int Floor;
        public int FloorRoom;
        
        public List<RoomData> NextRooms = new();
        public ELocationType RoomType;

        public RoomData(int floor, int floorRoom)
        {
            Floor = floor;
            FloorRoom = floorRoom;
        }
    }

    [Serializable] public struct PathGenAlgoParameters
    {
        [Min(1)] public int gridWidth;
        [Min(1)] public int gridLength;
        [Min(1)] public int numToGenerate;
    } 

    public static class PathGenAlgo
    {
        private static PathGenAlgoParameters _pathsAlgoParameters;
        
        public static List<RoomData> GeneratePaths(PathGenAlgoParameters algoParameters)
        {
            _pathsAlgoParameters = algoParameters;
            
            RoomData[,] paths = new RoomData[algoParameters.gridLength, algoParameters.gridWidth];
            
            for (int pathNum = 0; pathNum < algoParameters.numToGenerate; ++pathNum)
            {
                int floor = 0;
                int roomNum = Random.Range(0, algoParameters.gridWidth);

                paths[floor, roomNum] ??= new RoomData(floor, roomNum);
                
                GeneratePath(paths, floor, roomNum);
            }

            List<RoomData> firstFloor = new();
            for (int i = 0; i < algoParameters.gridWidth; ++i)
            {
                if (paths[0, i] is not null && paths[0, i].NextRooms.Count > 0)
                    firstFloor.Add(paths[0, i]);
            }

            return firstFloor;
        }
        
        private static void GeneratePath(RoomData[,] paths, int floor, int roomNum)
        {
            if (floor >= _pathsAlgoParameters.gridLength - 1)
                return;

            RoomData loc = paths[floor, roomNum];

            //Check if on bounds or is crossing path with neighbours
            int minNextRoom = NextLeftRoomAvailable(paths, floor, roomNum) ? roomNum - 1 : roomNum;
            int maxNextRoom = NextRightRoomAvailable(paths, floor, roomNum) ? roomNum + 1 : roomNum;
            
            //+ 1 because function is exclusive
            int nextFloor = floor + 1;
            int nextRoom = Random.Range(minNextRoom, maxNextRoom + 1);

            paths[nextFloor, nextRoom] ??= new RoomData(nextFloor, nextRoom);
            
            if (!loc.NextRooms.Contains(paths[nextFloor, nextRoom]))
                loc.NextRooms.Add(paths[nextFloor, nextRoom]);
            
            GeneratePath(paths, nextFloor, nextRoom);
        }

        private static bool NextLeftRoomAvailable(RoomData[,] paths, int floor, int floorRoom)
        {
            if (floorRoom == 0)
                return false;
            
            if (paths[floor, floorRoom - 1] == null)
                return true;

            foreach (RoomData room in paths[floor, floorRoom - 1].NextRooms)
            {
                if (room.FloorRoom == floorRoom)
                    return false;
            }

            return true;
        }

        private static bool NextRightRoomAvailable(RoomData[,] paths, int floor, int floorRoom)
        {
            if (floorRoom == _pathsAlgoParameters.gridWidth - 1)
                return false;
            
            if (paths[floor, floorRoom + 1] == null)
                return true;

            foreach (RoomData room in paths[floor, floorRoom + 1].NextRooms)
            {
                if (room.FloorRoom == floorRoom)
                    return false;
            }

            return true;
        }
    }
}