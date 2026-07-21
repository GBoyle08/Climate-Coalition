using UnityEngine;

public class BuildingData : ITargetable
{
    public BuildingType Type;

    public Team Owner { get; set; }

    public TileData Tile { get; set; }

    public int Health { get; set; }

    int ITargetable.Health
    {
        get => Health;
        set => Health = value;
    }

    public BuildingVisual Visual;

    public BuildingData(
        BuildingType type,
        Team owner,
        TileData tile)
    {
        Type = type;

        Owner = owner;

        Tile = tile;

        Health = type.MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }
}