using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UITowerManager : MonoBehaviour
{
    //#region IPointerEvents
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (eventData.pointerClick.GetComponent<UITowerInfo>() != null)
    //    {
    //        UITowerInfo towerInfo = eventData.pointerClick.GetComponent<UITowerInfo>();
    //        Debug.Log($"TowerInfo.TowerName(): {towerInfo.TowerName()}...");
    //        EventSystemManager.Instance.TriggerCurrentTower(towerInfo.TowerName());
    //    }
    //}
    //#endregion

    #region Protected Tower stats / information
    public abstract string TowerName();
    #endregion
}