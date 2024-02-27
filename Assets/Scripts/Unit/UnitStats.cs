using System;

[Serializable]
public class UnitStats
{
    public int maxHealth;
    public int moveSpeed;
    public int attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float maxRessources;

    private UnitStats()
    {
        this.SetStats(0, 0, 0, 0, 0, 0);
    }

    private UnitStats(int _health, int _moveSpeed, int _attackDamage, float _attackRange, float _maxRessources, float _attackCooldown)
    {
        this.SetStats(_health, _moveSpeed, _attackDamage, _attackRange, _maxRessources, _attackCooldown);
    }

    private UnitStats(UnitStats s2)
    {
        this.SetStats(s2.maxHealth, s2.moveSpeed, s2.attackDamage, s2.attackRange, s2.maxRessources, s2.attackCooldown);
    }

    public void SetStats(int _health, int _moveSpeed, int _attackDamage, float _attackRange, float _maxRessources, float _attackCooldown)
    {
        maxHealth = _health;
        moveSpeed = _moveSpeed;
        attackDamage = _attackDamage;
        attackRange = _attackRange;
        maxRessources = _maxRessources;
        attackCooldown = _attackCooldown;
    }

    public void SetStats(UnitStats s2)
    {
        this.SetStats(s2.maxHealth, s2.moveSpeed, s2.attackDamage, s2.attackRange, s2.maxRessources, s2.attackCooldown);
    }

    public static UnitStats operator+(UnitStats s1, UnitStats s2)
    {
        UnitStats sRes = new UnitStats();

        sRes.maxHealth = s1.maxHealth + s2.maxHealth;
        sRes.moveSpeed = s1.moveSpeed + s2.moveSpeed;
        sRes.attackDamage = s1.attackDamage + s2.attackDamage;
        sRes.attackRange = s1.attackRange + s2.attackRange;

        return sRes;
    }
}


