public class TowerStatsAndInfo : TowerManager
{
    public override string TowerName() { return towerName; }

    public override string TowerDescription() { return towerDescription; }

    public override int AttackDamage() { return attackDamage; }

    public override int AttackRange() { return attackRange; }

    public override float AttackSpeed() { return attackSpeed; }

    public override TypeOfTower TowerType() { return typeOfTower; }
}