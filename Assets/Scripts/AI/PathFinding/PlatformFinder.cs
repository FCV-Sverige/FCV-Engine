using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlatformFinder : MonoBehaviour
{
    public static PlatformFinder Instance { get; private set; }

    [SerializeField] private Tilemap tileMap;


    private List<Vector3Int> walkablePositions;

    public Vector3 CellHalfSize => tileMap ? tileMap.cellSize / 2f : Vector3.zero;
    public Tilemap TileMap => tileMap;
    private Vector3Int MaxV3Int => new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    
    private void Awake()
    {
        SingletonCheck();
    }

    public Vector3Int GetPlatformBelow(Vector3 position)
    {
        int i = 0;
        while (!tileMap.HasTile(tileMap.WorldToCell(position)))
        {
            position += Vector3.down;
            
            if (i == 9)
            {
                print("Null");
                return MaxV3Int;
            }

            i++;
        }
        
        return tileMap.WorldToCell(position);
    }
    
    public List<Vector3Int> GetClosestPlatform(Vector3 position)
    {
        List<Vector3Int> platformPositions = new();
        
        Vector3Int tilePosition = GetPlatformBelow(position);

        if (tilePosition == MaxV3Int) return null;

        platformPositions.Add(tilePosition);

        platformPositions.AddRange(CheckPlatformInDirection(tilePosition, Vector2Int.left));
        platformPositions.AddRange(CheckPlatformInDirection(tilePosition, Vector2Int.right));
        return platformPositions.Select(x => x + Vector3Int.up).ToList();
    }

    private List<Vector3Int> CheckPlatformInDirection(Vector3Int start, Vector2Int direction)
    {
        List<Vector3Int> platforms = new();
        Vector3Int nextTile = start;
        direction.Clamp(-Vector2Int.one, Vector2Int.one);
        
        while (tileMap.HasTile(nextTile) && !tileMap.HasTile(nextTile + Vector3Int.up))
        {
            if (nextTile != start) platforms.Add(nextTile);
            
            nextTile += (Vector3Int)direction;
        }

        return platforms;
    }

    private void OnDrawGizmos()
    {
        if (walkablePositions == null || walkablePositions.Count <= 0) return;

        foreach (var position in walkablePositions)
        {
            Gizmos.DrawWireSphere(position + CellHalfSize, .2f);
        }
    }

    #region Singleton
    private void SingletonCheck()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    #endregion
}
