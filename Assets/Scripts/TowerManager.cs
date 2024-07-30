using UnityEngine;

public abstract class TowerManager : MonoBehaviour
{
    [Header("Tower Info")]
    public string towerName;
    [TextArea] public string towerDescription;

    [Header("Attack stats"), Space(10)]
    public int attackDamage;
    public int attackRange;
    public float attackSpeed;
    public TypeOfTower typeOfTower;

    [HideInInspector] public enum TypeOfTower { NONE, SAUCE, CHEESY, PEPPORNI, PINEAPPLE }

    public abstract string TowerName();
    public abstract string TowerDescription();
    public abstract int AttackDamage();
    public abstract int AttackRange();
    public abstract float AttackSpeed();
    public abstract TypeOfTower TowerType();
}