using UnityEngine;
using UnityEngine.UI;

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
        AddOnClickEventToTowerButtons();
        EventSystemManager.Instance.onTriggerCurrentTower += SubscribeToTowerSelection;
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
    private void AddOnClickEventToTowerButtons()
    {
        foreach(Transform child in towerGroupContainer.transform)
        {
            child.GetComponent<Button>().onClick.AddListener(delegate { OnPointerDown(child.gameObject); });
        }
    }

    public void OnPointerDown(GameObject _towerButtonClicked)
    {
        UITowerInfo towerInfo = _towerButtonClicked.GetComponent<UITowerInfo>();
        EventSystemManager.Instance.TriggerCurrentTower(towerInfo.TowerName());
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