using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EventSystemManager : Singleton<EventSystemManager>
{
    public event Action<string> onTriggerCurrentTower;
    public void TriggerCurrentTower(string currentlySelectedTowerName)
    {
        if(onTriggerCurrentTower != null)
        {
            onTriggerCurrentTower(currentlySelectedTowerName);
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


    public event Action onTriggerSceneGameReset;
    public void TriggerSceneGameReset()
    {
        if(onTriggerSceneGameReset != null)
        {
            onTriggerSceneGameReset();
        }
    }

    public event Action<List<Object>> onTriggerResourceFolderLoaded;
    public void TriggerResourceFolderLoaded(List<Object> listOfTowersFromResource)
    {
        if(onTriggerResourceFolderLoaded != null)
        {
            onTriggerResourceFolderLoaded(listOfTowersFromResource);
        }
    }
}