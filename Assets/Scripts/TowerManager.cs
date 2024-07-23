using UnityEngine;

public abstract class TowerManager : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log($"@OnMouseDown {gameObject.name}");
        CreateTowerGameObject(gameObject);
    }

    private void CreateTowerGameObject(GameObject _towerTile)
    {
        if (!IsTowerTileOccupied())
        {
            GameObject _tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _tower.name = "Tower";
            _tower.transform.SetParent(_towerTile.transform);
            _tower.transform.position = new Vector3(_towerTile.transform.position.x, 1.5f, _towerTile.transform.position.z);
            _tower.transform.rotation = Quaternion.identity;
            _tower.transform.localScale = Vector3.one / 2f;
            _towerTile.GetComponent<TowerPlacement>().OccupyTile();
        }
    }

    public abstract bool IsTowerTileOccupied();

    public abstract void OccupyTile();

    //private IEnumerator OnMouseOver()
    //{
    //    Debug.Log($"@Hovering over ");
    //    yield return new WaitForSeconds(0.5f);
    //}
}