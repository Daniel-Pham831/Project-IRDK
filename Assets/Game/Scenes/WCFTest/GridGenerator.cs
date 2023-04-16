using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Scenes.WCFTest
{
    public class GridGenerator : MonoBehaviour
    {
        public int Columns = 1;
        public int Rows = 1;
        public float CellSize = 1f;

        private Vector2 Offset;
        private Vector2 _mousePosition;
        public GameObject tilesPrefab;
        public List<GameObject> tiles = new List<GameObject>();

        private void OnValidate()
        {
            Rows = Mathf.Clamp(Rows, 0, Rows);
            Columns = Mathf.Clamp(Columns, 0, Columns);
            CellSize = Mathf.Clamp(CellSize, 0f, CellSize);

            Offset.y = (float)((Rows / 2f - 0.5) * -1f);
            Offset.x = (float)((Columns / 2f - 0.5) * -1f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    Gizmos.DrawWireCube(new Vector3(column + Offset.x, row + Offset.y),
                        new Vector3(CellSize, CellSize, 0));
                }
            }
        }

        private void Start()
        {
            GenerateAllTiles();
        }

        private void GenerateAllTiles()
        {
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    var newTiles = Instantiate(tilesPrefab,new Vector3(column + Offset.x, row + Offset.y),Quaternion.identity);
                    newTiles.name = $"Tiles {row} - {column}";
                    tiles.Add(newTiles);
                }
            }
        }
    }
}