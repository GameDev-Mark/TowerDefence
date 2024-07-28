using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    string currentlySelectedTowerName;

    private void Start()
    {
        EventSystemManager.Instance.onTriggerCurrentTower += SetCurrentlySelectedTower;
    }

    #region Set some info
    private void SetCurrentlySelectedTower(string _currentlySelectedTowerName)
    {
        currentlySelectedTowerName = _currentlySelectedTowerName;
    }
    #endregion

    #region public functions 
    public string GetCurrentlySelectedTower()
    {
        return currentlySelectedTowerName;
    }
    #endregion
}