
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
        
        private List<Vector3> _spawnPoints;
        
        public override void Awake()
        {
            base.Awake();
            _spawnPoints = GetSpawnPoints();

        } 

        private List<Vector3> GetSpawnPoints()
        {
            List<Vector3> spawnPoints = new List<Vector3>();
            Transform parentTransform = gameObject.transform;

            for (int i = 0; i < parentTransform.childCount; i++)
            {
                // Get the child transform at the current index
                Transform childTransform = parentTransform.GetChild(i);

                if (childTransform != null)
                {
                    spawnPoints.Add(childTransform.position);
                }
            }

            return spawnPoints;
        }

        public Vector3 GetRandomSpawnPointAtDirection(Direction direction)
        {
            DirectionSpawnPoint directionSpawnPoint = _directionSpawnPoints.Find(x => x.Direction == direction);
            return directionSpawnPoint.SpawnPositions.Count == 0 ? transform.position : directionSpawnPoint.SpawnPositions.TakeRandom().position;
        }

        public Vector3 GetRandomSpawnPoint()
        {
            return _spawnPoints.Count == 0 ? transform.position : _spawnPoints.TakeRandom();
        }
    }
}