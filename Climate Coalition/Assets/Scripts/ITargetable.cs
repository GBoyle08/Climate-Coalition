using UnityEngine;

public interface ITargetable
{
    Team Owner { get; }

    TileData Tile { get; }

    int Health { get; set; }

    void TakeDamage(int damage);

    public void DestroyTarget(
    ITargetable target,
    UnitManager unitManager,
    BuildingManager buildingManager)
    {
        if (target is UnitData unit)
        {
            unitManager.KillUnit(unit);
        }

        else if (target is BuildingData building)
        {
            buildingManager.DestroyBuilding(building);
        }
    }
}
