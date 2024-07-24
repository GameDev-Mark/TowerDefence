using UnityEngine;

public class UITowerManager : MonoBehaviour
{
    private Animator towerMenuAnimator;

    private void Awake()
    {
        towerMenuAnimator = GetComponent<Animator>();
    }

    public void UIExpandOrShrinkTowerMenu()
    {
        if (towerMenuAnimator.GetBool("isTowerMenuOpen"))
            towerMenuAnimator.SetBool("isTowerMenuOpen", false);
        else
            towerMenuAnimator.SetBool("isTowerMenuOpen", true);
    }
}