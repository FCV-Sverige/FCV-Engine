using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlatformFinder : MonoBehaviour
{
    public static PlatformFinder Instance { get; private set; }

    [SerializeField] private Tilemap tileMap;
    [FormerlySerializedAs("enemyCantWalkThrough")] [SerializeField] private LayerMask layerCantWalkThrough;

    private List<Vector3Int> walkablePositions;

    public Vector3 CellHalfSize => tileMap ? tileMap.cellSize / 2f : Vector3.zero;
    public Tilemap TileMap => tileMap;
    private Vector3Int MaxV3Int => new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    
    private void Awake()
    {
        SingletonCheck();
    }
    
    /// <summary>
    /// Gets the tile that's closest below a position
    /// </summary>
    /// <param name="position">position to check from</param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Get the closest platform (tiles next to each other without a gap) below a certain position
    /// </summary>
    /// <param name="position">position to check from</param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Gets all tiles in a certain direction that has no tiles above it
    /// </summary>
    /// <param name="start">start tile position</param>
    /// <param name="direction">direction to check for other platforms</param>
    /// <returns></returns>
    private List<Vector3Int> CheckPlatformInDirection(Vector3Int start, Vector2Int direction)
    {
        List<Vector3Int> platforms = new();
        Vector3Int nextTile = start;
        direction.Clamp(-Vector2Int.one, Vector2Int.one);
        
        while (tileMap.HasTile(nextTile) && !tileMap.HasTile(nextTile + Vector3Int.up))
        {
            if (nextTile != start) platforms.Add(nextTile);
            
            
            if (Physics2D.OverlapBox((Vector2Int)nextTile + Vector2Int.up * 2, Vector2.one * .9f, 0, layerCantWalkThrough))
                return platforms;
            
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
