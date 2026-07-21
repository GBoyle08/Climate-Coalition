using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject unitPrefab;

    private List<UnitData> units =
        new List<UnitData>();

    public List<UnitData> Units = new List<UnitData>();

    public GridManager gridManager;
    public BuildingManager buildingManager;
    public UnitManager unitManager;
    public SelectionManager selectionManager;

    public UnitData SpawnUnit(
        UnitType type,
        Team team,
        TileData tile)
    {
        UnitData unit = new UnitData();

        unit.Type = type;

        unit.Team = team;

        unit.CurrentTile = tile;

        unit.Health = type.MaxHealth;

        unit.ActionPoints = type.MaxActionPoints;

        tile.Occupant = unit;

        GameObject obj = Instantiate(
            type.Prefab,
            new Vector3(
                tile.Position.x,
                1f,
                tile.Position.y
            ),
            Quaternion.identity
        );

        UnitVisual visual = obj.GetComponent<UnitVisual>();

        visual.Initialize(unit);

        unit.Visual = visual;

        Units.Add(unit);

        return unit;
    }

    public bool MoveUnit(UnitData unit, TileData destination)
    {
        int cost = destination.MovementCost;

        if (unit.ActionPoints < cost)
        {
            return false;
        }

        if (destination.Occupant != null)
        {
        return false;
        }

        selectionManager.ClearMovementPreview();

        unit.CurrentTile.Occupant = null;

        destination.Occupant = unit;

        unit.CurrentTile = destination;

        unit.ActionPoints -= cost;
        
        selectionManager.ShowMovementPreview();

        return true;
    }

    public void StartTurn(UnitData unit)
    {
        unit.ActionPoints = unit.Type.MaxActionPoints;
    }

    public void RefreshUnits(Team team)
    {
        foreach (UnitData unit in Units)
        {
            if (unit.Team == team)
            {
                unit.ActionPoints =
                    unit.Type.MaxActionPoints;
            }
        }
    }

    public List<UnitData> GetUnitsOnTeam(Team team)
    {
        List<UnitData> result = new List<UnitData>();

        foreach (UnitData unit in Units)
        {
            if (unit.Team == team)
            {
                result.Add(unit);
            }
        }

        return result;
    }

    public ITargetable GetBestTarget(UnitData enemy)
    {
        UnitData closestUnit = null;

        BuildingData closestBuilding = null;

        int unitCost = int.MaxValue;

        int buildingCost = int.MaxValue;

        foreach (UnitData unit in GetUnitsOnTeam(Team.Player))
        {
            List<TileData> path =
                gridManager.GetPath(
                    enemy.CurrentTile,
                    unit.CurrentTile
                );

            if (path == null)
            {
                continue;
            }

            int cost = gridManager.GetPathCost(path);

            if (cost < unitCost)
            {
                unitCost = cost;
                closestUnit = unit;
            }
        }

        foreach (BuildingData building in
                buildingManager.GetBuildingsOnTeam(Team.Player))
        {
            List<TileData> path =
                gridManager.GetPath(
                    enemy.CurrentTile,
                    building.Tile
                );

            if (path == null)
            {
                continue;
            }

            int cost = gridManager.GetPathCost(path);

            if (cost < buildingCost)
            {
                buildingCost = cost;
                closestBuilding = building;
            }
        }

        if (closestBuilding != null &&
            buildingCost <= unitCost * 1.5f)
        {
            return closestBuilding;
        }

        return closestUnit;
    }

    public void KillUnit(UnitData unit)
    {
        unit.CurrentTile.Occupant = null;

        Units.Remove(unit);

        Destroy(unit.Visual.gameObject);

        Debug.Log(
            $"{unit.Type.UnitName} died."
        );
    }

    public void Attack(UnitData attacker, ITargetable defender)
    {
        if (attacker.Team == defender.Owner)
        {
            return;
        }

        int distance =
            Mathf.Abs(
                attacker.CurrentTile.Position.x -
                defender.Tile.Position.x
            )
            +
            Mathf.Abs(
                attacker.CurrentTile.Position.y -
                defender.Tile.Position.y
            );

        if (distance > attacker.Type.AttackRange)
        {
            return;
        }

        if (attacker.ActionPoints < attacker.Type.AttackCost)
        {
            return;
        }

        attacker.ActionPoints -= attacker.Type.AttackCost;

        defender.TakeDamage(
            attacker.Type.Damage
        );

        if (defender.Health <= 0)
        {
            defender.DestroyTarget(defender, unitManager, buildingManager);
        }
    }
}