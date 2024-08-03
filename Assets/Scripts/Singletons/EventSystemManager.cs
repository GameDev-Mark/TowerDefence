using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EventSystemManager : Singleton<EventSystemManager>
{
    #region tower interactions
    public event Action<string> onTriggerCurrentlySelectedTowerFromTowerMenu;
    public void TriggerCurrentlySelectedTowerFromTowerMenu(string currentlySelectedTowerName)
    {
        if(onTriggerCurrentlySelectedTowerFromTowerMenu != null)
        {
            onTriggerCurrentlySelectedTowerFromTowerMenu(currentlySelectedTowerName);
        }
    }

    public event Action<GameObject> onTriggerTowerTileClick;
    public void TriggerTowerTileClick(GameObject _towerTileClickedOn)
    {
        if (onTriggerTowerTileClick != null)
        {
            onTriggerTowerTileClick(_towerTileClickedOn);
        }
    }

    public event Action<GameObject> onTriggerTowerTileSellTower;
    public void TriggerTowerTileSellTower(GameObject _currentlySelectedTowerTile)
    {
        if (onTriggerTowerTileSellTower != null)
        {
            onTriggerTowerTileSellTower(_currentlySelectedTowerTile);
        }
    }

    public event Action<GameObject> onTriggerTowerTileUpgradeTower;
    public void TriggerTowerTileUpgradeTower(GameObject _currentlySelectedTowerTile)
    {
        if (onTriggerTowerTileUpgradeTower != null)
        {
            onTriggerTowerTileUpgradeTower(_currentlySelectedTowerTile);
        }
    }

    public event Action<GameObject> onTriggerTowerTileClosePopup;
    public void TriggerTowerTileClosePopup(GameObject _currentlySelectedTowerTile)
    {
        if (onTriggerTowerTileClosePopup != null)
        {
            onTriggerTowerTileClosePopup(_currentlySelectedTowerTile);
        }
    }
    #endregion

    #region Scenes
    public event Action onTriggerSceneGameReset;
    public void TriggerSceneGameReset()
    {
        if(onTriggerSceneGameReset != null)
        {
            onTriggerSceneGameReset();
        }
    }
    #endregion

    #region Resources
    public event Action<List<Object>> onTriggerResourceFolderLoaded;
    public void TriggerResourceFolderLoaded(List<Object> listOfTowersFromResource)
    {
        if(onTriggerResourceFolderLoaded != null)
        {
            onTriggerResourceFolderLoaded(listOfTowersFromResource);
        }
    }
    #endregion
}