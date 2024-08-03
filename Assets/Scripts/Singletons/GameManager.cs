using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    string currentlySelectedTowerNameFromTowerMenu;
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
        EventSystemManager.Instance.onTriggerCurrentlySelectedTowerFromTowerMenu += SetCurrentlySelectedTowerFromTowerMenu;
    }
    private void OnDestroy()
    {
        EventSystemManager.Instance.onTriggerResourceFolderLoaded -= SetResourceFolder;
        EventSystemManager.Instance.onTriggerCurrentlySelectedTowerFromTowerMenu -= SetCurrentlySelectedTowerFromTowerMenu;
    }
    #endregion

    #region Set some info
    private void SetCurrentlySelectedTowerFromTowerMenu(string _currentlySelectedTowerNameFromTowerMenu)
    {
        currentlySelectedTowerNameFromTowerMenu = _currentlySelectedTowerNameFromTowerMenu;
    }
    #endregion

    #region public functions 
    public string GetCurrentlySelectedTowerFromTowerMenu()
    {
        return currentlySelectedTowerNameFromTowerMenu;
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