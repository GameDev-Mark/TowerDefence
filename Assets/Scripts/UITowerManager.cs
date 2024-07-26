using UnityEngine;
using UnityEngine.UI;

public class UITowerManager : MonoBehaviour
{
    #region Private variables
    private GameObject towerSprite;
    private Animator towerMenuAnimator;
    private string currentlyClickedTower = "";
    #endregion

    #region Public variables

    #endregion

    #region Unity Functions
    private void Awake()
    {
        towerMenuAnimator = GetComponent<Animator>();
        TowerSpriteIcon();
    }

    private void Update()
    {
        UpdateTowerImageSpritePosition();
        CancelTowerClick();
    }
    #endregion

    #region Button events
    public void UIExpandOrShrinkTowerMenu() // attached the UI expand and shrink tower menu button
    {
        if (towerMenuAnimator.GetBool("isTowerMenuOpen"))
            towerMenuAnimator.SetBool("isTowerMenuOpen", false);
        else
            towerMenuAnimator.SetBool("isTowerMenuOpen", true);
    }

    public void UIChooseATower(string _towerName) // attached to the UI tower(s) list button(s)
    {
        currentlyClickedTower = _towerName;
        towerSprite.SetActive(true);
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
            ResetTowerInfo();
        }
    }
    #endregion

    #region Tower information
    private void TowerSpriteIcon()
    {
        towerSprite = new GameObject();
        towerSprite.name = "Tower Sprite";
        towerSprite.transform.SetParent(this.gameObject.transform.parent);
        towerSprite.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        towerSprite.AddComponent<RectTransform>();
        towerSprite.AddComponent<Image>();
        towerSprite.GetComponent<Image>().raycastTarget = false;
        towerSprite.SetActive(false);
    }
    #endregion

    #region Public information
    public string TowerName()
    {
        return currentlyClickedTower;
    }
    public void ResetTowerInfo()
    {
        towerSprite.SetActive(false);
        currentlyClickedTower = "";
    }
    #endregion
}