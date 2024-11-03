using System;
using Unity.VisualScripting;
using UnityEngine;


public class Goblinmon : MonoBehaviour
{
    [SerializeField] public SOGoblinmon goblinData;
    public int attackModifier;
    public int defenseModifier;

    void Awake()
    {
        //Set current HP to max HP at start of battle
        //goblinData.InitilizeGoblinmonHP();
    }

    //Unit takes damage
    //TODO:
    //SCRIPTABLE OBJECTS FOR GOBLINMON DATA MEANS THAT THERE CANNOT BE MORE THAN ONE OF ANY UNIT
    //IN A FIGHT AT ANY GIVEN TIME WITHOUT COMPLETELY SCREWING OVER THE ENTIRE BATTLE
    //FIX THAT YESTERDAY
    public bool TakeDamage(int dmg, bool weakness, Goblinmon attacker)
    {
        int damageToDeal = dmg;
        if (weakness)
        {
            damageToDeal = ApplyDamageModifiers(damageToDeal, attacker);
            damageToDeal *= 2;
            goblinData.currentHP = goblinData.currentHP - damageToDeal;
        }
        else
        {
            dmg = ApplyDamageModifiers(dmg, attacker);
            goblinData.currentHP -= dmg;
        }

        if (goblinData.currentHP <= 0) return true;
        else return false;
    }

    //Applies stat changes to damage value
    public int ApplyDamageModifiers(int dmg, Goblinmon attacker)
    {
        int returnDamage = dmg;

        //Compare modifiers to see if they cancel out (+1 attack swinging into +1 defense is neutral damage)
        if (attacker.attackModifier > defenseModifier)
        {
            int atkModTemp = attacker.attackModifier - defenseModifier;
            returnDamage = (int)(returnDamage * (1 + .5 * atkModTemp)); //Each attack point = roughly 50% more damage
        }
        else if (defenseModifier > attacker.attackModifier)
        {
            int defModTemp = defenseModifier - attacker.attackModifier;
            returnDamage = (int)(returnDamage * Math.Pow(.5, defModTemp)); //each defense point = roughly 50% less damage
        }
        else return returnDamage;

        return returnDamage;
    }

}
