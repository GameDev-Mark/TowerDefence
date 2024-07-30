using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    private GameObject towerSprite;
    private GameObject towerGroupContainer;

    public Animator uiTowerMenuAnimator;

    #region Unity Functions
    private void Awake()
    {
        CreateTowerSpriteIcon();
        uiTowerMenuAnimator = GetComponentInChildren<Animator>();
        towerGroupContainer = uiTowerMenuAnimator.transform.GetChild(1).gameObject;
    }
    private void Start()
    {
        EventSystemManager.Instance.onTriggerCurrentTower += SubscribeToTowerSelection;
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

    #region Tower information
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

    #region Tower Button Info
    private void LoadUITowerButtons()
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

    private void ApplyUITowerButtonStats(Object _resourceTower, GameObject _towerButton)
    {
        TowerStatsAndInfo freshTowerStatScript = _towerButton.AddComponent<TowerStatsAndInfo>();
        TowerStatsAndInfo getResourceTowerStatsScript = _resourceTower.GetComponent<TowerStatsAndInfo>();

        freshTowerStatScript.towerName = getResourceTowerStatsScript.TowerName();
        freshTowerStatScript.towerDescription = getResourceTowerStatsScript.TowerDescription();
        freshTowerStatScript.attackDamage = getResourceTowerStatsScript.AttackDamage();
        freshTowerStatScript.attackRange = getResourceTowerStatsScript.AttackRange();
        freshTowerStatScript.attackSpeed = getResourceTowerStatsScript.AttackSpeed();
        freshTowerStatScript.typeOfTower = getResourceTowerStatsScript.TowerType();

        AddOnClickEventToTowerButtons(_towerButton);
    }

    private void AddOnClickEventToTowerButtons(GameObject _towerButton)
    {
        _towerButton.GetComponent<Button>().onClick.AddListener(delegate { OnPointerDown(_towerButton.gameObject); });
    }

    private void OnPointerDown(GameObject _towerButtonClicked)
    {
        TowerStatsAndInfo towerStats = _towerButtonClicked.GetComponent<TowerStatsAndInfo>();
        EventSystemManager.Instance.TriggerCurrentTower(towerStats.TowerName());
    }
    #endregion

    #region event triggers
    private void SubscribeToTowerSelection(string _towerName)
    {
        ActivateTowerSpriteIcon(_towerName);
        DeactivateTowerSpriteIcon(_towerName);
    }
    #endregion

    #region Button events
    public void UIExpandOrShrinkTowerMenu() // attached the UI expand and shrink tower menu button
    {
        if (uiTowerMenuAnimator.GetBool("isTowerMenuOpen"))
            uiTowerMenuAnimator.SetBool("isTowerMenuOpen", false);
        else
            uiTowerMenuAnimator.SetBool("isTowerMenuOpen", true);
    }
    #endregion
}