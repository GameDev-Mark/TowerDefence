using UnityEngine;

public class TowerPlacement : TowerManager
{
    private bool isTowerTileOccupied = false;

    public override void OccupyTile()
    {
        if(!isTowerTileOccupied) { isTowerTileOccupied = true; }
    }

    public override bool IsTowerTileOccupied()
    {
        Debug.Log($"TowerPlacement: isTowerTileOccupied: {isTowerTileOccupied}.. ");
        return isTowerTileOccupied;
    }
}