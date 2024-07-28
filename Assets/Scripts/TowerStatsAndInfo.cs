public class TowerPlacement : TowerManager
{
    private bool isTowerTileOccupied = false;

    protected override void OccupyTile()
    {
        if (!isTowerTileOccupied) { isTowerTileOccupied = true; }
    }

    protected override bool IsTowerTileOccupied()
    {
        return isTowerTileOccupied;
    }
}