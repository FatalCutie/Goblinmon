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
    public bool TakeDamage(int dmg, bool weakness, Goblinmon attacker)
    {
        if (weakness)
        {
            dmg *= 2;
            dmg = ApplyDamageModifiers(dmg, attacker);
            goblinData.currentHP -= dmg;
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
        //TODO: Take enemy defense into consideration when attacking, not own
        int returnDamage = dmg;

        //Compare modifiers to see if they cancel out (one attack swinging into one defense is neutral damage)
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
