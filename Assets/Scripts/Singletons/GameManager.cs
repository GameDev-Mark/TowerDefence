using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private string currentlySelectedTowerNameFromTowerMenu;
    private List<Object> listOfTowersFromDirectory;
    private int inGameCurrencyAmount;

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
        EventSystemManager.Instance.onTriggerUpdateInGameCurrnecy += UpdateInGameCurrency;
        InitialGameStartStats();
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
    private void InitialGameStartStats()
    {
        inGameCurrencyAmount = 0;
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
    public int GetCurrentCurrencyAmount()
    {
        return inGameCurrencyAmount;
    }
    #endregion

    #region currency
    private void UpdateInGameCurrency(int _currencyAmount, int _updateCurrencyAmountBy)
    {
        inGameCurrencyAmount = _currencyAmount + _updateCurrencyAmountBy;
    }
    #endregion

    #region setup List<> of towers from resource folder .. setting objects
    private void SetResourceFolder(List<Object> _listOfTowersFromResource)
    {
        listOfTowersFromDirectory = _listOfTowersFromResource;
    }
    #endregion
}