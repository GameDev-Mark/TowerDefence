using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    string currentlySelectedTowerName;
    private List<Object> listOfTowersFromDirectory;

    #region Unity
    // override awake from singleton .. since Resources folder loads just after scene load from TowerDirectory.cs
    // we can only subscribe to the event in Awake() and not Start()
    public override void Awake()
    {
        EventSystemManager.Instance.onTriggerResourceFolderLoaded += SetResourceFolder; 
        base.Awake();
    }
    private void Start()
    {
        EventSystemManager.Instance.onTriggerCurrentTower += SetCurrentlySelectedTower;
    }
    private void OnDestroy()
    {
        EventSystemManager.Instance.onTriggerResourceFolderLoaded -= SetResourceFolder;
        EventSystemManager.Instance.onTriggerCurrentTower -= SetCurrentlySelectedTower;
    }
    #endregion

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
    public List<Object> ReturnListOfTowersFromResourceFolder()
    {
        return listOfTowersFromDirectory;
    }
    #endregion

    private void SetResourceFolder(List<Object> _listOfTowersFromResource)
    {
        listOfTowersFromDirectory = _listOfTowersFromResource;
    }
}