using UnityEngine;

public class TileVisual : MonoBehaviour
{
    public TileData Data;

    public Material Grass;
    public Material Forest;
    public Material Hill;
    public Material Mountain;
    public Material River;
    public Material Water;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void Initialize(TileData data)
    {
        Data = data;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        switch (Data.Type)
        {
            case TileType.Grass:
                rend.material = Grass;
                break;

            case TileType.Forest:
                rend.material = Forest;
                break;

            case TileType.Hill:
                rend.material = Hill;
                break;

            case TileType.Mountain:
                rend.material = Mountain;
                break;

            case TileType.River:
                rend.material = River;
                break;

            case TileType.Water:
                rend.material = Water;
                break;
        }
    }
}