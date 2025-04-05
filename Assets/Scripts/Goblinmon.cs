using System;
using UnityEngine;


public class Goblinmon : MonoBehaviourID
{
    [SerializeField] public SOGoblinmon goblinData;
    public int currentHP;
    public int attackModifier;
    public int defenseModifier;
    public enum FriendOrFoe { FRIEND, FOE }; //Affects which direction the sprite faces
    public FriendOrFoe friendOrFoe = FriendOrFoe.FOE; //Defaults to Foe, flipped on capture

    //This is really inefficient LOL!

    void Awake()
    {
        //Set current HP to max HP at start of battle
        //goblinData.InitializeGoblinmonHP();
    }

    //Unit takes damage
    public bool TakeDamage(int dmg, bool weakness, Goblinmon attacker)
    {
        int damageToDeal = dmg;
        if (weakness)
        {
            damageToDeal = ApplyDamageModifiers(damageToDeal, attacker);
            damageToDeal *= 2;
            currentHP -= damageToDeal;
            if (currentHP < 0) currentHP = 0; //clamp damage min
        }
        else
        {
            dmg = ApplyDamageModifiers(dmg, attacker);
            currentHP -= dmg;
            if (currentHP < 0) currentHP = 0; //clamp damage min
        }

        if (currentHP <= 0) return true;
        else return false;
    }

    //Applies stat changes to damage value
    public int ApplyDamageModifiers(int dmg, Goblinmon attacker)
    {
        //Creates random modifier to multiply damage with
        float decider = FindObjectOfType<BattleSystem>().rnd.Next(1, 16);
        float randomDamageModifier = 0.84f + decider * 0.01f; //Creates damage range of .85 and 1

        int returnDamage = dmg;

        //Compare modifiers to see if they cancel out (+1 attack swinging into +1 defense is neutral damage)
        if (attacker.attackModifier > defenseModifier)
        {
            int atkModTemp = attacker.attackModifier - defenseModifier;
            returnDamage = (int)(returnDamage * ((2+atkModTemp)/2)); //Each attack point = roughly 50% more damage
            returnDamage = (int)(returnDamage * randomDamageModifier);
        }
        else if (defenseModifier > attacker.attackModifier)
        {
            int defModTemp = defenseModifier - attacker.attackModifier;
            //3+def/3
            returnDamage = (int)(returnDamage * ((3+defModTemp)/3)); //each defense point = roughly 33% less damage
            returnDamage = (int)(returnDamage * randomDamageModifier);
        }
        else return (int)(returnDamage * randomDamageModifier);

        return returnDamage;
    }

}
