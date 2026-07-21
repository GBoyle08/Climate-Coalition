using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingData> Buildings =
        new List<BuildingData>();

    public BuildingData SpawnBuilding(
        BuildingType type,
        Team owner,
        TileData tile)
    {
        BuildingData building =
            new BuildingData(
                type,
                owner,
                tile
            );

        GameObject obj = Instantiate(
            type.Prefab,
            new Vector3(
                tile.Position.x,
                0.25f,
                tile.Position.y
            ),
            Quaternion.identity
        );

        BuildingVisual visual =
            obj.GetComponent<BuildingVisual>();

        visual.Initialize(building);

        building.Visual = visual;

        tile.Building = building;

        Buildings.Add(building);

        return building;
    }

    public List<BuildingData> GetBuildingsOnTeam(Team team)
    {
        List<BuildingData> result = new();

        foreach (BuildingData building in Buildings)
        {
            if (building.Owner == team)
            {
                result.Add(building);
            }
        }

        return result;
    }

    public void DestroyBuilding(
    BuildingData building)
    {
        building.Tile.Building = null;

        Buildings.Remove(building);

        Object.Destroy(
            building.Visual.gameObject
        );
    }
}