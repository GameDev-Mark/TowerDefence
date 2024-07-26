using System.Collections;
using UnityEngine;

public abstract class TowerManager : MonoBehaviour
{
    #region Mouse clicks
    private void OnMouseDown()
    {
        LoopThroughTowerListAndInstaniateCorrectTower();
    }
    private void OnMouseOver()
    {
        TowerVisualWhenHovering();
    }
    private void OnMouseExit()
    {
        TowerVisualWhenHoveringIsDone();
    }
    private void OnMouseEnter()
    {
        
    }
    #endregion

    #region Tower creation
    private void LoopThroughTowerListAndInstaniateCorrectTower()
    {
        if (!IsTowerTileOccupied())
        {
            foreach (var child in TowerDirectory.ReturnListOfTowersFromDirectory())
            {
                if (child.name == FindObjectOfType<UITowerManager>().TowerName())
                {
                    GameObject _tower = Instantiate(child) as GameObject;
                    CreateGameObjectInfo(_tower, gameObject);
                    FindObjectOfType<UITowerManager>().ResetTowerInfo();
                    StartCoroutine(TowerVisualWhenClickedOn());
                }
            }
        }
    }

    private void CreateGameObjectInfo(GameObject _tower, GameObject _towerTile)
    {
        _tower.name = $"Tower_{_tower.name}";
        _tower.transform.SetParent(_towerTile.transform);
        _tower.transform.position = new Vector3(_towerTile.transform.position.x, 1.5f, _towerTile.transform.position.z);
        _tower.transform.rotation = Quaternion.identity;
        _tower.transform.localScale = Vector3.one / 2f;
        GenerateTowerMaterial(_tower, "BridgeTile_CobbleStone");
        _towerTile.GetComponent<TowerPlacement>().OccupyTile();
    }

    private void GenerateTowerMaterial(GameObject _towerGO, string resourceName)
    {
        Renderer _rend = _towerGO.GetComponent<Renderer>();
        _rend.material = Resources.Load($"Materials/{resourceName}") as Material;
        GetTowerRenderer().material = _rend.material;
    }

    private Renderer GetTowerRenderer()
    {
        return gameObject.GetComponent<Renderer>();
    }
    #endregion

    #region Tower visuals
    private IEnumerator TowerVisualWhenClickedOn()
    {
        GetTowerRenderer().material.color = Color.gray;
        yield return new WaitForSeconds(0.05f);
        GetTowerRenderer().material.color = Color.black;
        yield return new WaitForSeconds(0.15f);
        GetTowerRenderer().material.color = Color.gray;
        yield return null;
    }

    private void TowerVisualWhenHovering()
    {
        GetTowerRenderer().material.color = Color.yellow;
    }

    private void TowerVisualWhenHoveringIsDone()
    {
        if (!IsTowerTileOccupied())
            GetTowerRenderer().material.color = Color.white;
        else
            GetTowerRenderer().material.color = Color.gray;
            
    }
    #endregion

    #region Public individual tower functionality
    public abstract bool IsTowerTileOccupied();

    public abstract void OccupyTile();
    #endregion
}