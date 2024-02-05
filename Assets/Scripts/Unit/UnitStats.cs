using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UnitStats
{
    
    public int health;
    public int moveSpeed;
    public int attackDamage;
    public int attackRange;

    private UnitStats()
    {
        this.SetStats(0, 0, 0, 0);
    }

    private UnitStats(int _Health, int _MoveSpeed, int _AttackDamage, int _AttackRange)
    {
        this.SetStats(_Health, _MoveSpeed, _AttackDamage, _AttackRange);
    }

    private UnitStats(UnitStats s2)
    {
        this.SetStats(s2.health, s2.moveSpeed, s2.attackDamage, s2.attackRange);
    }

    private void SetStats(int _Health, int _MoveSpeed, int _AttackDamage, int _AttackRange)
    {
        health = _Health;
        moveSpeed = _MoveSpeed;
        attackDamage = _AttackDamage;
        attackRange = _AttackRange;
    }

    public static UnitStats operator+(UnitStats s1, UnitStats s2)
    {
        UnitStats sRes = new UnitStats();

        sRes.health = s1.health + s2.health;
        sRes.moveSpeed = s1.moveSpeed + s2.moveSpeed;
        sRes.attackDamage = s1.attackDamage + s2.attackDamage;
        sRes.attackRange = s1.attackRange + s2.attackRange;

        return sRes;
    }
}


