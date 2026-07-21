using UnityEngine;

[CreateAssetMenu(menuName = "Building/Building Type")]
public class BuildingType : ScriptableObject
{
    [Header("Basic Info")]
    public string BuildingName;

    public GameObject Prefab;

    [Header("Health")]
    public int MaxHealth = 20;

    [Header("Ownership")]
    public bool CanBeCaptured = true;

    [Header("Economy")]
    public List<ResourceAmount> ResourcesProduced;

    public List<ResourceAmount> Cost;

    [Header("Production")]
    public bool CanProduceUnits = false;

    public UnitType[] UnitsProduced;
}