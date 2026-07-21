using UnityEngine;

public class BuildingVisual : MonoBehaviour
{
    public BuildingData Data;

    public void Initialize(
        BuildingData building)
    {
        Data = building;
    }
}