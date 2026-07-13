using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public const int Width = 64;
    public const int Height = 64;

    public TileData[,] Tiles;

    public GameObject tilePrefab;
    public float tileSize = 1f;

    public float scale = 0.05f;

    private float seedX;
    private float seedY;

    private float forestSeedX;
    private float forestSeedY;

    public TileVisual[,] TileVisuals;

    private void Awake()
    {
        seedX = Random.Range(0f, 10000f);
        seedY = Random.Range(0f, 10000f);

        forestSeedX = Random.Range(0f, 10000f);
        forestSeedY = Random.Range(0f, 10000f);

        TileVisuals = new TileVisual[Width, Height];

        GenerateGrid();
    }

    void GenerateGrid() 
    {
    Tiles = new TileData[Width, Height];

    for (int x = 0; x < Width; x++)
    {
        for (int y = 0; y < Height; y++)
        {
            float height =
                Mathf.PerlinNoise(
                    (x + seedX) * 0.03f,
                    (y + seedY) * 0.03f
                ) * 0.7f
                +
                Mathf.PerlinNoise(
                    (x + seedX) * 0.08f,
                    (y + seedY) * 0.08f
                ) * 0.3f;
            
            TileData tile = new TileData(new Vector2Int(x, y));

            tile.Height = height;

            if (height < 0.25f)
            {
                tile.Type = TileType.Water;
            }
            else if (height < 0.55f)
            {
                tile.Type = TileType.Grass;
            }
            else if (height < 0.70f)
            {
                tile.Type = TileType.Hill;
            }
            else
            {
                tile.Type = TileType.Mountain;
            }

            Tiles[x, y] = tile;

            float forestNoise = Mathf.PerlinNoise(
            (x + forestSeedX) * 0.08f,
            (y + forestSeedY) * 0.08f
            );

            if (
                Tiles[x,y].Type == TileType.Grass &&
                forestNoise > 0.6f
            )
            {
                Tiles[x,y].Type = TileType.Forest;
            }

            GameObject obj = Instantiate(
                tilePrefab,
                new Vector3(x * tileSize, 0, y * tileSize),
                tilePrefab.transform.rotation,
                transform
            );

            TileVisual visual = obj.GetComponent<TileVisual>();
            visual.Initialize(Tiles[x, y]);

            TileVisuals[x, y] = visual;
        }
    }

    Debug.Log($"Starting river generation");
    List<TileData> riverSources = new List<TileData>();

    for (int i = 0; i < 3; i++)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                TileData tile = Tiles[x, y];

                if(tile == null)
                {
                    Debug.LogError($"Tile ({x}, {y}) is null");
                }

                if (
                    tile.Type == TileType.Mountain &&
                    HasLowerNeighbor(tile)
                )
                {
                    riverSources.Add(tile);
                }
            }
        }

        if (riverSources.Count < 3)
        {
            Debug.LogWarning($"Only found {riverSources.Count} valid river sources.");

            return;
        }

            int index = Random.Range(0, riverSources.Count);
            GenerateRiver(riverSources[index]);
            riverSources.RemoveAt(index);
            Debug.Log($"River #" + (i + 1) + " generated");
    }

    for (int updX = 0; updX < Width; updX++)
    {
        for (int updY = 0; updY < Height; updY++)
        {
            TileVisuals[updX, updY].UpdateVisual();
        }
    }
}

    public TileData GetTile(int x, int y)
    {
        return Tiles[x, y];
    }

    private List<TileData> GetNeighbors(TileData tile)
    {
        List<TileData> neighbors = new List<TileData>();

        int x = tile.Position.x;
        int y = tile.Position.y;

        int[,] directions =
        {
            { 1, 0 },
            { -1, 0 },
            { 0, 1 },
            { 0, -1 }
        };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + directions[i, 0];
            int ny = y + directions[i, 1];

            if (nx >= 0 && nx < Width &&
                ny >= 0 && ny < Height)
            {
                neighbors.Add(Tiles[nx, ny]);
            }
        }

        return neighbors;
    }

    private TileData GetLowestNeighbor(TileData tile)
    {
        TileData lowest = null;
        float lowestHeight = tile.Height;

        foreach (TileData neighbor in GetNeighbors(tile))
        {
            if (neighbor.Height <= lowestHeight + 0.02f)
            {
                lowest = neighbor;
                lowestHeight = neighbor.Height;
            }
        }

        return lowest ?? tile;
    }

    private void GenerateRiver(TileData source)
    {
        TileData current = source;

        int maxLength = 100;

        while (maxLength-- > 0)
        {
            if (current.Type == TileType.Water)
            {
                Debug.Log($"River Hit Water");
                return;
            }

            if (current.Type != TileType.Mountain)
            {
                current.Type = TileType.River;
            }

            TileData next = GetLowestNeighbor(current);

            if (next == current)
            {
                Debug.Log($"River Stopped Flowing");
                return;
            }

            current = next;
        }
    }

    private bool HasLowerNeighbor(TileData tile)
    {
        foreach (TileData neighbor in GetNeighbors(tile))
        {
            if (neighbor.Height < tile.Height)
            {
                return true;
            }
        }

        return false;
    }
}