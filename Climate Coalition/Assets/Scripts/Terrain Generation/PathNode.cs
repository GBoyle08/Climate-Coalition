using UnityEngine;

public class PathNode
{
    public TileData Tile;

    public int Cost;

    public PathNode Previous;

    public PathNode(
        TileData tile,
        int cost,
        PathNode previous = null)
    {
        Tile = tile;
        Cost = cost;
        Previous = previous;
    }
}
