using System;

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

    public event Action onTriggerSceneGameReset;
    public void TriggerSceneGameReset()
    {
        if(onTriggerSceneGameReset != null)
        {
            onTriggerSceneGameReset();
        }
    }
}