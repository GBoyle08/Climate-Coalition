using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<Team, ResourceInventory>
        inventories =
            new Dictionary<Team, ResourceInventory>();

    private void Awake()
    {
        inventories[Team.Player] =
            new ResourceInventory();

        inventories[Team.Enemy] =
            new ResourceInventory();
    }

    public ResourceInventory GetInventory(
        Team team)
    {
        return inventories[team];
    }

    public void GenerateResources(
    BuildingManager buildingManager)
    {
        foreach (BuildingData building
                in buildingManager.Buildings)
        {
            ResourceInventory inventory =
                inventories[building.Owner];

            foreach (ResourceAmount amount
                    in building.Type.ResourcesProduced)
            {
                inventory.Add(
                    amount.Type,
                    amount.Amount
                );
            }
        }
    }
}