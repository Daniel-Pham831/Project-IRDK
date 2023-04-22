using System.Collections;
using System.Collections.Generic;
using Maniac.Utils.Extension;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{

    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private TileBase _tileBase;
    // Start is called before the first frame update
    // void Start()
    // {
    //     var a = 10;
    //     a++;
    //     a--;
    //     var allTiles = _tilemap.GetTiles<TileBase>();
    //     a--;
    //
    //     foreach (var tile in allTiles)
    //     {
    //     }
    // }

    public List<Vector3> availablePlaces;
 
    void Start () {
        availablePlaces = new List<Vector3>();
 
        for (int n = _tilemap.cellBounds.xMin; n < _tilemap.cellBounds.xMax; n++)
        {
            for (int p = _tilemap.cellBounds.yMin; p < _tilemap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)_tilemap.transform.position.y));
                Vector3 place = _tilemap.CellToWorld(localPlace);
                if (_tilemap.HasTile(localPlace))
                {
                    //Tile at "place"
                    availablePlaces.Add(place);
                }
                else
                {
                    //No tile at "place"
                }
            }
        }

        _tilemap.SetTile(availablePlaces[4].ToVector3Int(),_tileBase);
    }
}
