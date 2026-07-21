using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    public UnitData Data;

    public GameObject selectionRing;

    public void Initialize(UnitData data)
    {
        Data = data;

        selectionRing.SetActive(false);

        gameObject.name =
            $"{data.Team} {data.Type}";
    }

    public void MoveTo(TileData tile)
    {
        transform.position =
            new Vector3(
                tile.Position.x,
                0.5f,
                tile.Position.y
            );
    }

    public void SetSelected(bool selected)
    {
        selectionRing.SetActive(selected);
    }
}