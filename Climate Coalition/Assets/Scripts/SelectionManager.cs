using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    public Camera mainCamera;

    public UnitData selectedUnit;

    private UnitVisual selectedVisual;
    private UnitVisual hoveredEnemy;

    public Team currentPlayerTeam = Team.Player;

    public GameManager gameManager;

    public UnitManager unitManager;

    private Dictionary<TileData, PathNode>
        reachableTiles =
            new Dictionary<TileData, PathNode>();

    public GridManager gridManager;

    private HashSet<TileData> previewedTiles = new HashSet<TileData>();

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SelectObject();
        }

        HandleHoverPreview();
    }

    private void SelectUnit()
    {
        Debug.Log("Clicked");

        ClearMovementPreview();

        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Hit {hit.collider.gameObject.name}");

            UnitVisual unit = hit.collider.GetComponent<UnitVisual>();

            if (unit != null)
            {
                Debug.Log($"Selected {unit.Data.Type.UnitName}");

                if (selectedVisual != null)
                {
                    selectedVisual.SetSelected(false);
                }

                selectedUnit = unit.Data;
                selectedVisual = unit;

                selectedVisual.SetSelected(true);

                ShowMovementPreview();
            }
            else
            {
                Debug.Log("No UnitVisual found.");
                selectedVisual.SetSelected(false);

                ClearMovementPreview();
            }
        }
        else
        {
            Debug.Log("Raycast hit nothing.");
        }
    }

    private void SelectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        UnitVisual unit = hit.collider.GetComponentInParent<UnitVisual>();

        if (unit != null)
        {
            if (selectedUnit != null &&
                unit.Data.Team != selectedUnit.Team)
            {
                TryAttack(unit.Data);

                return;
            }

            if (unit.Data.Team == gameManager.currentTurn)
            {
                SelectUnit();

                return;
            }
        }

        TileVisual tile = hit.collider.GetComponentInParent<TileVisual>();

        if (tile != null)
        {
            SelectTile(tile);
        }
    }

    public void ShowMovementPreview()
    {
        ClearMovementPreview();

        reachableTiles =
            gridManager.GetReachableTiles(
                selectedUnit.CurrentTile,
                selectedUnit.ActionPoints
            );

        foreach (TileData tile in reachableTiles.Keys)
        {
            TileVisual visual =
                gridManager.TileVisuals[
                    tile.Position.x,
                    tile.Position.y
                ];

            PreviewTile(tile, PreviewType.Attack);

            previewedTiles.Add(tile);
        }

        Debug.Log($"Showing {previewedTiles.Count} preview tiles");
    }

    public void ClearMovementPreview()
    {
        foreach (TileData tile in previewedTiles)
        {
            gridManager.TileVisuals[
                tile.Position.x,
                tile.Position.y
            ].SetPreview(PreviewType.None);
        }

        previewedTiles.Clear();
    }

    private void SelectTile(TileVisual tileVisual)
    {
        if (selectedUnit == null)
        {
            return;
        }

        TileData tile = tileVisual.Data;

        if (!reachableTiles.ContainsKey(tile))
        {
            return;
        }

        MoveSelectedUnit(tile);
    }

    private void MoveSelectedUnit(TileData destination)
    {
        int cost = reachableTiles[destination].Cost;

        if (selectedUnit.ActionPoints < cost)
        {
            return;
        }

        // Clear the old preview while reachableTiles
        // still contains the old positions.
        ClearMovementPreview();

        selectedUnit.CurrentTile.Occupant = null;

        destination.Occupant = selectedUnit;

        selectedUnit.CurrentTile = destination;

        PathNode path = reachableTiles[destination];

        selectedUnit.ActionPoints -= cost;

        selectedUnit.Visual.transform.position =
            new Vector3(
                destination.Position.x,
                0.5f,
                destination.Position.y
            );

        Debug.Log($"Selected unit is now at {selectedUnit.CurrentTile.Position}");
        ShowMovementPreview();
    }

    private void TryAttack(UnitData target)
    {
        unitManager.Attack(selectedUnit, target);

        ClearMovementPreview();

        if (hoveredEnemy != null)
        {
            ShowAttackPreview(hoveredEnemy.Data);
        }
        else
        {
            ShowMovementPreview();
        }
    }

    private void HandleHoverPreview()
    {
        if (selectedUnit == null)
        {
            return;
        }

        Ray ray =
            mainCamera.ScreenPointToRay(
                Mouse.current.position.ReadValue()
            );

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            RestoreMovementPreview();

            return;
        }

        UnitVisual unit =
            hit.collider.GetComponentInParent<UnitVisual>();

        if (
            unit != null &&
            unit.Data.Team != selectedUnit.Team
        )
        {
            if (hoveredEnemy != unit)
            {
                hoveredEnemy = unit;

                ShowAttackPreview(unit.Data);
            }
        }
        else
        {
            RestoreMovementPreview();

            hoveredEnemy = null;
        }
    }

    private void ShowAttackPreview(UnitData target)
    {
        ClearMovementPreview();

        TileData center = selectedUnit.CurrentTile;

        for (int x = 0; x < GridManager.Width; x++)
        {
            for (int y = 0; y < GridManager.Height; y++)
            {
                TileData tile = gridManager.GetTile(x, y);

                int dx = Mathf.Abs(
                    tile.Position.x -
                    center.Position.x
                );

                int dy = Mathf.Abs(
                    tile.Position.y -
                    center.Position.y
                );

                int distance = dx + dy;

                if (distance <= selectedUnit.Type.AttackRange)
                {
                    PreviewTile(tile, PreviewType.Attack);

                    previewedTiles.Add(tile);
                }
            }
        }
    }

    private void RestoreMovementPreview()
    {
        ClearMovementPreview();

        reachableTiles =
            gridManager.GetReachableTiles(
                selectedUnit.CurrentTile,
                selectedUnit.ActionPoints
            );

        foreach (TileData tile in reachableTiles.Keys)
        {
            PreviewTile(
                tile,
                PreviewType.Movement
            );
        }
    }

    private void PreviewTile(
    TileData tile,
    PreviewType previewType)
    {
        gridManager.TileVisuals[
            tile.Position.x,
            tile.Position.y
        ].SetPreview(previewType);

        previewedTiles.Add(tile);
    }
}