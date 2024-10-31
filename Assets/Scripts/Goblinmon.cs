using System;
using Unity.VisualScripting;
using UnityEngine;


public class Goblinmon : MonoBehaviour
{
    [SerializeField] public SOGoblinmon goblinData;
    public int currentHP;
    public int attackModifier;
    public int defenseModifier;

    void Awake()
    {
        try
        {
            currentHP = goblinData.maxHP; //This will need to be changed LOL!
        }
        //I don't remember why this warning exists but I remember that it's not really important LOL!
        catch (NullReferenceException) { Debug.LogWarning("Goblinmon Unit did not have a data SO at creation! Please disregard if intended"); }

    }


    public bool TakeDamage(int dmg, bool weakness)
    {
        if (weakness)
        {
            dmg *= 2;
            dmg = ApplyDamageModifiers(dmg);
            currentHP -= dmg;
        }
        else
        {
            dmg = ApplyDamageModifiers(dmg);
            currentHP -= dmg;
        }


        if (currentHP <= 0) return true;
        else return false;
    }

    //Applies stat changes to damage value
    private int ApplyDamageModifiers(int dmg)
    {
        int returnDamage = dmg;
        if (attackModifier > defenseModifier)
        {
            int atkModTemp = attackModifier - defenseModifier;
            returnDamage = (int)(returnDamage * (1 + .5 * atkModTemp)); //Each attack point = roughly 50% more damage
            Debug.Log($"Attack Modifier: {atkModTemp}");
        }
        else if (defenseModifier > attackModifier)
        {
            int defModTemp = defenseModifier - attackModifier;
            returnDamage = (int)(returnDamage * Math.Pow(.5, defModTemp)); //each defense point = roughly 50% less damage
            Debug.Log($"Defense Modifier: {defModTemp}");
        }
        else return returnDamage;

        return returnDamage;
    }
}
