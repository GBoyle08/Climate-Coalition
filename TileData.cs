using UnityEngine;

public class TileData
{
    public Vector2Int Position;
    public TileType Type;
    public int MovementCost;
    public UnitData Occupant;
    public float Height;

    public TileData(Vector2Int position)
    {
        Position = position;
        Type = TileType.Grass;
        MovementCost = 1;
    }
}
