using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TowerTileHandler : MonoBehaviour
{
    private bool isTowerTileOccupied = false;
    private Renderer _towerTileChildTowerRenderer;

    #region Mouse clicks
    private void OnMouseDown()
    {
        ClickOnTowerTile();
    }
    private void OnMouseOver()
    {
        TowerVisualWhenHovering();
        //Debug.Log($"On mouse HOVERING.. towerManager.cs");
    }
    private void OnMouseExit()
    {
        TowerVisualWhenHoveringIsDone();
    }
    private void OnMouseEnter()
    {
        //Debug.Log($"On mouse enter.. towerManager.cs");
    }
    #endregion

    #region Interaction
    private void ClickOnTowerTile()
    {
        if (!IsTowerTileOccupied())
        {
            LoopThroughTowerListAndInstaniateCorrectTower();
            //StartCoroutine(TowerVisualWhenClickedOn(Color.white, Color.black, Color.white));
        }
        else
        {
            //StartCoroutine(TowerVisualWhenClickedOn(Color.gray, Color.blue, Color.gray));
        }
    }
    #endregion

    #region Tower creation
    private void LoopThroughTowerListAndInstaniateCorrectTower()
    {
        foreach (var child in GameManager.Instance.ReturnListOfTowersFromResourceFolder())
        {
            if (child.GetComponent<TowerStatsAndInfo>().TowerName() == GameManager.Instance.GetCurrentlySelectedTower())
            {
                GameObject _tower = Instantiate(child) as GameObject;
                CreateGameObjectInfo(_tower, gameObject);
                EventSystemManager.Instance.TriggerCurrentTower(null);
                _towerTileChildTowerRenderer = gameObject.transform.GetChild(0).GetComponent<Renderer>();
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
        _towerTile.GetComponent<TowerTileHandler>().OccupyTile();
    }

    #endregion

    #region Tower visuals
    private IEnumerator TowerVisualWhenClickedOn(Color _initClick, Color _changeTo, Color _initClickColor)
    {
        GetTowerRenderer().material.color = _initClick;
        yield return new WaitForSeconds(0.05f);
        GetTowerRenderer().material.color = _changeTo;
        yield return new WaitForSeconds(0.15f);
        GetTowerRenderer().material.color = _initClickColor;
        yield return null;
    }

    private void TowerVisualWhenHovering()
    {
        GetTowerRenderer().material.color = Color.yellow;

        if (GetChildTowerMaterialInTowerTile() != null)
            GetChildTowerMaterialInTowerTile().material.color = Color.yellow;
    }

    private void TowerVisualWhenHoveringIsDone()
    {
        if (!IsTowerTileOccupied())
        {
            GetTowerRenderer().material.color = Color.white;
            if (GetChildTowerMaterialInTowerTile() != null)
                GetChildTowerMaterialInTowerTile().GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            GetTowerRenderer().material.color = Color.gray;
            if (GetChildTowerMaterialInTowerTile() != null)
                GetChildTowerMaterialInTowerTile().GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    private Renderer GetChildTowerMaterialInTowerTile()
    {
        return _towerTileChildTowerRenderer;
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

    #region Tile checks
    private void OccupyTile()
    {
        if (!isTowerTileOccupied) { isTowerTileOccupied = true; }
    }

    private bool IsTowerTileOccupied()
    {
        return isTowerTileOccupied;
    }
    #endregion
}