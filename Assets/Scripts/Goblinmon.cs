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
    public bool TakeDamage(SOMove move, Goblinmon attacker)
    {
        int damageToDeal;

        damageToDeal = ApplyDamageModifiers(move, attacker);
        currentHP -= damageToDeal;

        if (currentHP < 0) currentHP = 0; //clamp damage min
        if (currentHP <= 0) return true;
        else return false;
    }

    //Applies stat changes to damage value
    public int ApplyDamageModifiers(SOMove move, Goblinmon attacker)
    {
        //Creates random modifier to multiply damage with
        float decider = FindObjectOfType<BattleSystem>().rnd.Next(1, 16);
        float randomDamageModifier = 0.84f + decider * 0.01f; //Creates damage range of .85 and 1

        int returnDamage = move.damage;

        //Each attack point = roughly 50% more damage, defense roughly 33% reduction
        if (move.moveModifier == SOMove.MoveModifier.DEFENSE_SCALE)
            returnDamage = returnDamage * ((2 + attacker.defenseModifier) / 2) * ((3 + defenseModifier) / 3);
        else returnDamage = returnDamage * ((2 + attacker.attackModifier) / 2) * ((3 + defenseModifier) / 3);
        returnDamage = (int)(returnDamage * randomDamageModifier);
        return returnDamage * GetWeaknessMultiplier(move);
    }

    public int GetWeaknessMultiplier(SOMove move)
    {
        int toReturn = 0;
        foreach (SOType type in goblinData.types)
        {
            if (type.weakness.Contains(move.moveType)) toReturn += 2;

        }
        if (toReturn < 1) toReturn = 1; //if no weaknesses return 1

        return toReturn;
    }
}
