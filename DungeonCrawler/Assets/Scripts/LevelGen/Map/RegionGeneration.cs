using System;
using System.Collections.Generic;
using Algo.MapGeneration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGen.Map
{
    [Serializable] public struct PathGenParameters
    {
        public GameObject   roomPrefab;
        public Vector3      origin;
        public float        floorSpace;
        public float        floorPosVariance;
        public float        roomSpace;
        public float        roomPosVariance;
    } 
    
    public class RegionGeneration : MonoBehaviour
    {
        [SerializeField] private int seed;

        [Header("Path")]
        [SerializeField] private PathGenAlgoParameters pathGenAlgoParams;
        [SerializeField] private PathGenParameters pathGenParams;
        
        private List<RoomData> _firstFloor = new();

        private List<GameObject> _rooms = new();

        private void Start()
        {
            GenerateMap();
        }

        public void GenerateMap()
        {
            Random.InitState(seed);
            _firstFloor = PathGenAlgo.GeneratePaths(pathGenAlgoParams);

            if (!pathGenParams.roomPrefab)
                return;

            foreach (RoomData room in _firstFloor)
                GenerateRooms(room);
        }

        private void GenerateRooms(RoomData room)
        {
            if (!room.IsGenerated)
            {
                float xPos = room.FloorRoom * pathGenParams.roomSpace +
                             Random.Range(-pathGenParams.roomPosVariance, pathGenParams.roomPosVariance);
                float yPos = room.Floor * pathGenParams.floorSpace +
                             Random.Range(-pathGenParams.floorPosVariance, pathGenParams.floorPosVariance);

                Vector3 pos = new(xPos, 0, yPos);

                GameObject newRoom = Instantiate(pathGenParams.roomPrefab, pos + pathGenParams.origin, Quaternion.identity);

                room.GameObject = newRoom;
                _rooms.Add(newRoom);
                room.IsGenerated = true;
            }

            foreach (RoomData nextRoom in room.NextRooms)
                GenerateRooms(nextRoom);
        }

        private void OnDrawGizmos()
        {
            if (_firstFloor.Count <= 0)
                return;

            foreach (RoomData room in _firstFloor)
            {
                Gizmos.color = Color.red;
                PropagateGizmo(room);
            }
        }

        private void PropagateGizmo(RoomData room)
        {
            foreach (RoomData nextRoom in room.NextRooms)
            {
                Gizmos.DrawLine(room.GameObject.transform.position, nextRoom.GameObject.transform.position);
                PropagateGizmo(nextRoom);
            }
        }
    }
}
