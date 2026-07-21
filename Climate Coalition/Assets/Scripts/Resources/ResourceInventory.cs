using UnityEngine;
using System.Collections.Generic;

public class ResourceInventory
{
    private Dictionary<ResourceType, int> resources =
        new Dictionary<ResourceType, int>();

    public ResourceInventory()
    {
        foreach (ResourceType type in
                 System.Enum.GetValues(
                     typeof(ResourceType)))
        {
            resources[type] = 0;
        }
    }

    public int Get(ResourceType type)
    {
        return resources[type];
    }

    public void Add(
        ResourceType type,
        int amount)
    {
        resources[type] += amount;
    }

    public bool CanAfford(
        List<ResourceAmount> cost)
    {
        foreach (ResourceAmount entry in cost)
        {
            if (Get(entry.Type) < entry.Amount)
            {
                return false;
            }
        }

        return true;
    }

    public void Spend(
        List<ResourceAmount> cost)
    {
        foreach (ResourceAmount entry in cost)
        {
            resources[entry.Type] -= entry.Amount;
        }
    }
}