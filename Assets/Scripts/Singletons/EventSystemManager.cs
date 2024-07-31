using System;
using System.Collections.Generic;
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

    public event Action onTriggerTowerTileClick;
    public void TriggerTowerTileClick()
    {
        if (onTriggerTowerTileClick != null)
        {
            onTriggerTowerTileClick();
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