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

    public Material movementMaterial;

    public Material attackMaterial;

    private Renderer rend;

    private Renderer tileRenderer;

    public GameObject highlightPlane;

    private Color originalColor;

    private MeshRenderer highlightRenderer;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        tileRenderer = GetComponent<Renderer>();

        originalColor = tileRenderer.material.color;

        highlightRenderer = highlightPlane.GetComponent<MeshRenderer>();

        highlightPlane.SetActive(false);
    }

    public void Initialize(TileData data)
    {
        Data = data;
        UpdateVisual();
    }

    public void SetHighlighted(bool highlighted)
    {
        highlightPlane.SetActive(highlighted);
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

    public void SetPreview(PreviewType type)
    {
        switch (type)
        {
            case PreviewType.None:

                highlightPlane.SetActive(false);
                break;

            case PreviewType.Movement:

                highlightPlane.SetActive(true);

                highlightRenderer.material =
                    movementMaterial;

                break;

            case PreviewType.Attack:

                highlightPlane.SetActive(true);

                highlightRenderer.material =
                    attackMaterial;

                break;
        }
    }   
}