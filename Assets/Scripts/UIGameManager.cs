using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    private GameObject towerSprite;
    private GameObject towerGroupContainer;
    private GameObject towerTileTowerInfoGO;
    private GameObject expandOrShrinkTowerMenuButton;
    private GameObject currentlySelectedTowerTile;
    private GameObject closeButtonOnTowerTileUI;
    private GameObject sellButtonOnTowerTileUI;
    private GameObject upgradeButtonOnTowerTileUI;

    private Animator uiTowerMenuAnimator;

    #region Unity Functions
    private void Awake()
    {
        CreateTowerSpriteIcon();
        SetupTowerMenuAnimationInfo();
        SetupTowerPopupMenuInfo();

        towerGroupContainer = uiTowerMenuAnimator.transform.GetChild(1).gameObject;
    }
    private void Start()
    {
        EventSystemManager.Instance.onTriggerCurrentTower += SubscribeToTowerSelection;
        EventSystemManager.Instance.onTriggerTowerTileClick += SubscribeToTowerTileClickedOn;
        LoadUITowerButtons();
    }
    private void Update()
    {
        UpdateTowerImageSpritePosition();
        CancelTowerClick();
    }
    #endregion

    #region Update functionality
    private void UpdateTowerImageSpritePosition()
    {
        if (towerSprite.activeInHierarchy)
        {
            towerSprite.transform.position = Input.mousePosition;
        }
    }
    private void CancelTowerClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EventSystemManager.Instance.TriggerCurrentTower(null);
        }
    }
    #endregion

    #region Tower Icon information
    private void CreateTowerSpriteIcon()
    {
        if (towerSprite == null)
        {
            towerSprite = new GameObject();
            towerSprite.name = "Tower Sprite Mouse Position";
            towerSprite.transform.SetParent(this.gameObject.transform);
            towerSprite.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            towerSprite.AddComponent<RectTransform>();
            towerSprite.AddComponent<Image>();
            towerSprite.GetComponent<Image>().raycastTarget = false;
            towerSprite.SetActive(false);
        }
    }

    private void ActivateTowerSpriteIcon(string _towerName)
    {
        // _towerName will be the specified tower sprite icon 
        if (_towerName != null)
        {
            towerSprite.SetActive(true);
        }
    }

    public void DeactivateTowerSpriteIcon(string _towerName)
    {
        if (_towerName == null)
        {
            towerSprite.SetActive(false);
        }
    }
    #endregion

    #region Tower Buttons creation information
    private void LoadUITowerButtons() // this gets loaded at the start()
    {
        foreach (var resourceTower in GameManager.Instance.ReturnListOfTowersFromResourceFolder())
        {
            GameObject _towerButt = Instantiate(Resources.Load("Prefabs/UI/TowerButtonTemplate") as GameObject);
            _towerButt.name = resourceTower.name;
            _towerButt.GetComponentInChildren<TMP_Text>().text = _towerButt.name;
            _towerButt.transform.SetParent(towerGroupContainer.transform);
            _towerButt.transform.localScale = Vector3.one;

            ApplyUITowerButtonStats(resourceTower, _towerButt);
        }
    }

    private void ApplyUITowerButtonStats(Object _resourceTower, GameObject _towerButton) // applied from the start()
    {
        TowerStatsAndInfo freshTowerStatScript = _towerButton.AddComponent<TowerStatsAndInfo>();
        TowerStatsAndInfo getResourceTowerStatsScript = _resourceTower.GetComponent<TowerStatsAndInfo>();

        freshTowerStatScript.towerName = getResourceTowerStatsScript.TowerName();
        freshTowerStatScript.towerDescription = getResourceTowerStatsScript.TowerDescription();
        freshTowerStatScript.attackDamage = getResourceTowerStatsScript.AttackDamage();
        freshTowerStatScript.attackRange = getResourceTowerStatsScript.AttackRange();
        freshTowerStatScript.attackSpeed = getResourceTowerStatsScript.AttackSpeed();
        freshTowerStatScript.typeOfTower = getResourceTowerStatsScript.TowerType();

        AddOnClickListenerToButton(_towerButton);
    }

    #endregion

    #region UI tower pop up menu 
    private void SetupTowerPopupMenuInfo()
    {
        towerTileTowerInfoGO = transform.Find("TowerTileTowerInfo").gameObject;
        towerTileTowerInfoGO.SetActive(false);

        closeButtonOnTowerTileUI = towerTileTowerInfoGO.transform.Find("CloseWindowButton").gameObject;
        sellButtonOnTowerTileUI = towerTileTowerInfoGO.transform.Find("SellButton").gameObject;
        upgradeButtonOnTowerTileUI = towerTileTowerInfoGO.transform.Find("UpgradeButton").gameObject;

        AddOnClickListenerToButton(closeButtonOnTowerTileUI);
        AddOnClickListenerToButton(sellButtonOnTowerTileUI);
        AddOnClickListenerToButton(upgradeButtonOnTowerTileUI);
    }

    private void ActivateTowerPopupMenu()
    {
        towerTileTowerInfoGO.SetActive(true);
        towerTileTowerInfoGO.transform.position = Input.mousePosition + new Vector3(100, 95);
    }

    private void ShowCorrectTowerStatsOnTowerPopup(GameObject _towerTileClickedOn)
    {
        TowerStatsAndInfo _towerStats = _towerTileClickedOn.GetComponentInChildren<TowerStatsAndInfo>();
        GameObject _towerInfoContainer = towerTileTowerInfoGO.transform.Find("TowerStatsContainer").gameObject;
        int childLoopCount = 0;
        foreach (Transform child in _towerInfoContainer.transform)
        {
            if (child.GetComponentInChildren<TMP_Text>())
            {
                if (childLoopCount == 0)
                    child.GetComponentInChildren<TMP_Text>().text = $"Tower Name\n{_towerStats.TowerName()}";
                if (childLoopCount == 1)
                    child.GetComponentInChildren<TMP_Text>().text = $"Description\n{_towerStats.TowerDescription()}";
                if (childLoopCount == 2)
                    child.GetComponentInChildren<TMP_Text>().text = $"Attack Damage\n{_towerStats.AttackDamage()}";
                if (childLoopCount == 3)
                    child.GetComponentInChildren<TMP_Text>().text = $"Attack Range\n{_towerStats.AttackRange()}";
                if (childLoopCount == 4)
                    child.GetComponentInChildren<TMP_Text>().text = $"Attack Speed\n{_towerStats.AttackSpeed()}";
                if (childLoopCount == 5)
                    child.GetComponentInChildren<TMP_Text>().text = $"Tower Type\n{_towerStats.TowerType()}";
                childLoopCount++;
            }
        }
    }

    private void DeselectTowerTile()
    {
        towerTileTowerInfoGO.SetActive(false);
        currentlySelectedTowerTile = null;
    }

    private void UpgradeTowerButtonFromTowerTilePopup()
    {
        Debug.Log($"Upgrade tower clicked...");
        EventSystemManager.Instance.TriggerTowerTileUpgradeTower(currentlySelectedTowerTile);
        ///todo
        // shoot event
        // upgrade specific stat on the selected tower ( not sure how the upgrades are gonna work yet )
        // UI upgrade stats change - visuals
        // other scripts then subscribe to this event - tileHandler?
    }

    private void SellTowerButtonFromTowerTilePopup()
    {
        Debug.Log($"Sell tower clicked...");
        EventSystemManager.Instance.TriggerTowerTileSellTower(currentlySelectedTowerTile);
    }
    #endregion

    #region TowerMenu animation info
    private void SetupTowerMenuAnimationInfo()
    {
        uiTowerMenuAnimator = GetComponentInChildren<Animator>();
        expandOrShrinkTowerMenuButton = uiTowerMenuAnimator.transform.GetChild(2).GetChild(1).gameObject;
        AddOnClickListenerToButton(expandOrShrinkTowerMenuButton);
    }

    public void UIExpandOrShrinkTowerMenu() // attached the UI expand and shrink tower menu button
    {
        if (uiTowerMenuAnimator.GetBool("isTowerMenuOpen"))
            uiTowerMenuAnimator.SetBool("isTowerMenuOpen", false);
        else
            uiTowerMenuAnimator.SetBool("isTowerMenuOpen", true);
    }
    #endregion

    #region event triggers
    private void SubscribeToTowerSelection(string _towerName)
    {
        ActivateTowerSpriteIcon(_towerName);
        DeactivateTowerSpriteIcon(_towerName);
    }

    private void SubscribeToTowerTileClickedOn(GameObject _towerTileClickedOn)
    {
        ActivateTowerPopupMenu();
        ShowCorrectTowerStatsOnTowerPopup(_towerTileClickedOn);
        currentlySelectedTowerTile = _towerTileClickedOn;
    }
    #endregion

    #region Button events
    private void AddOnClickListenerToButton(GameObject _buttonGO)
    {
        _buttonGO.GetComponent<Button>().onClick.AddListener(delegate { ButtonClicks(_buttonGO.gameObject); });
    }

    private void ButtonClicks(GameObject _buttonClicked)
    {
        if (_buttonClicked.GetComponent<TowerStatsAndInfo>())
        {
            TowerStatsAndInfo towerStats = _buttonClicked.GetComponent<TowerStatsAndInfo>();
            EventSystemManager.Instance.TriggerCurrentTower(towerStats.TowerName());
        }
        if (_buttonClicked.name == expandOrShrinkTowerMenuButton.name)
        {
            UIExpandOrShrinkTowerMenu();
        }
        if (_buttonClicked.name == sellButtonOnTowerTileUI.name)
        {
            SellTowerButtonFromTowerTilePopup();
            DeselectTowerTile();
        }
        if (_buttonClicked.name == upgradeButtonOnTowerTileUI.name)
        {
            UpgradeTowerButtonFromTowerTilePopup();
        }
        if (_buttonClicked.name == closeButtonOnTowerTileUI.name)
        {
            DeselectTowerTile();
        }
    }
    #endregion
}