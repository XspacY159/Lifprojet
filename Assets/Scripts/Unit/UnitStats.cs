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

    private UnitStats(int _health, int _moveSpeed, int _attackDamage, int _attackRange)
    {
        this.SetStats(_health, _moveSpeed, _attackDamage, _attackRange);
    }

    private UnitStats(UnitStats s2)
    {
        this.SetStats(s2.health, s2.moveSpeed, s2.attackDamage, s2.attackRange);
    }

    public void SetStats(int _health, int _moveSpeed, int _attackDamage, int _attackRange)
    {
        health = _health;
        moveSpeed = _moveSpeed;
        attackDamage = _attackDamage;
        attackRange = _attackRange;
    }

    public void SetStats(UnitStats s2)
    {
        this.SetStats(s2.health, s2.moveSpeed, s2.attackDamage, s2.attackRange);
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


