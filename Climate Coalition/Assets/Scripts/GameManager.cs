using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public UnitManager unitManager;
    public SelectionManager selectionManager;
    public BuildingManager buildingManager;
    public ResourceManager resourceManager;

    public UnitType meleeType;
    public UnitType rangedType;

    public BuildingType headquartersType;
    public BuildingType barracksType;

    public Team currentTurn = Team.Player;

    private void Start()
    {
        unitManager.SpawnUnit(
            meleeType,
            Team.Enemy,
            gridManager.GetTile(5, 5)
        );

        unitManager.SpawnUnit(
            rangedType,
            Team.Player,
            gridManager.GetTile(10, 10)
        );

        buildingManager.SpawnBuilding(
            headquartersType,
            Team.Player,
            gridManager.GetTile(32, 32)
        );

        buildingManager.SpawnBuilding(
            barracksType,
            Team.Player,
            gridManager.GetTile(12, 12)
        );

        unitManager.RefreshUnits(currentTurn);
    }

    public void EndTurn()
    {
        resourceManager.GenerateResources(
            buildingManager
        );
        
        if (currentTurn == Team.Player)
        {
            currentTurn = Team.Enemy;
        }
        else
        {
            currentTurn = Team.Player;
        }

        unitManager.RefreshUnits(currentTurn);

        if (currentTurn == Team.Enemy)
        {
            selectionManager.ClearMovementPreview();
            RunEnemyTurn();
        }
    }

    private void RunEnemyTurn()
    {
        Debug.Log("Enemy turn started");

        List<UnitData> enemies =
            unitManager.GetUnitsOnTeam(Team.Enemy);
        
        Debug.Log($"Found {enemies.Count} enemies");

        foreach (UnitData enemy in enemies)
        {
            Debug.Log(
                $"Enemy at {enemy.CurrentTile.Position}"
            );

            ITargetable target =
                unitManager.GetBestTarget(enemy);

            if (target == null)
            {
                Debug.Log("No target found");
                continue;
            }

            int distance =
                Mathf.Abs(
                    enemy.CurrentTile.Position.x -
                    target.Tile.Position.x
                )
                +
                Mathf.Abs(
                    enemy.CurrentTile.Position.y -
                    target.Tile.Position.y
                );

            if (distance <= enemy.Type.AttackRange)
            {
                unitManager.Attack(
                    enemy,
                    target
                );

                continue;
            }

            Debug.Log(
                $"Target found at {target.Tile.Position}"
            );

            List<TileData> path =
                gridManager.GetPath(
                    enemy.CurrentTile,
                    target.Tile
                );
            
            if (path == null)
            {
                Debug.Log("No path found");
                continue;
            }

            Debug.Log($"Path length: {path.Count}");

            if (path == null)
            {
                continue;
            }

            MoveEnemyAlongPath(enemy, path);

            if (distance <= enemy.Type.AttackRange)
            {
                unitManager.Attack(
                    enemy,
                    target
                );

                continue;
            }
        }

        EndTurn();
    }

    private void MoveEnemyAlongPath(
        UnitData enemy,
        List<TileData> path)
    {
        Debug.Log(
            $"Enemy AP before moving: {enemy.ActionPoints}"
        );

        int remainingAP = enemy.ActionPoints;

        TileData destination = enemy.CurrentTile;

        foreach (TileData tile in path)
        {
            int cost = tile.MovementCost;

            if (cost > remainingAP)
            {
                break;
            }

            if (
                tile.Occupant != null &&
                tile != enemy.CurrentTile
            )
            {
                break;
            }

            destination = tile;

            remainingAP -= cost;
        }

        enemy.CurrentTile.Occupant = null;

        destination.Occupant = enemy;

        enemy.CurrentTile = destination;

        enemy.ActionPoints = remainingAP;

        enemy.Visual.transform.position =
            new Vector3(
                destination.Position.x,
                0.5f,
                destination.Position.y
            );
        
        Debug.Log(
            $"Moving enemy to {destination.Position}"
        );
    }
}