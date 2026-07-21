using UnityEngine;

public class UnitData : ITargetable
{
    public Team Team;

    public TileData CurrentTile;

    public UnitType Type;

    public UnitVisual Visual;

    public int Health;

    public int ActionPoints;

    public Team Owner => Team;

    public TileData Tile => CurrentTile;

    int ITargetable.Health
    {
        get => Health;
        set => Health = value;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}