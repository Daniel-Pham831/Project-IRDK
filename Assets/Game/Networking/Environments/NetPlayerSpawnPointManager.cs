
using System;
using System.Collections.Generic;
using Game.Enums;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Game.Networking.Environments
{
    [Serializable]
    public struct DirectionSpawnPoint
    {
        public Direction Direction;
        public List<Transform> SpawnPositions;
    }
    
    public class NetPlayerSpawnPointManager : MonoLocator<NetPlayerSpawnPointManager>
    {
        [SerializeField] private List<DirectionSpawnPoint> _directionSpawnPoints;
        
        [SerializeField] private List<Transform> _spawnPoints;
        
        public Vector3 GetRandomSpawnPointAtDirection(Direction direction)
        {
            DirectionSpawnPoint directionSpawnPoint = _directionSpawnPoints.Find(x => x.Direction == direction);
            return directionSpawnPoint.SpawnPositions.Count == 0 ? transform.position : directionSpawnPoint.SpawnPositions.TakeRandom().position;
        }

        public Vector3 GetRandomSpawnPoint()
        {
            return _spawnPoints.Count == 0 ? transform.position : _spawnPoints.TakeRandom().position;
        }
    }
}