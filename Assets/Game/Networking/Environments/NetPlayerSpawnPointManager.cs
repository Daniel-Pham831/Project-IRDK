using System;
using System.Collections.Generic;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;

namespace Game.Networking.Environments
{
    public class NetPlayerSpawnPointManager : MonoBehaviour
    {
        private List<Vector3> _spawnPoints;

        private void Awake()
        {
            Locator<NetPlayerSpawnPointManager>.Set(this,true);
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

        public Vector3 GetRandomSpawnPoint()
        {
            return _spawnPoints.Count == 0 ? transform.position : _spawnPoints.TakeRandom();
        }
    }
}