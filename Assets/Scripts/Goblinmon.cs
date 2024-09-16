using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Goblinmon : MonoBehaviour
{
    public string gName;
    public int gLevel;

    public int damage;

    public Sprite sprite;

    public int currentHP;
    public int maxHP;


    public SOType type;
    public List<SOMove> moveset;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0) return true;
        else return false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }
}
