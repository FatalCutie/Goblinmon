using System;
using System.Collections;
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

    public IEnumerable PlayMoveAnimation(SOMove move)
    {
        yield return new WaitForSeconds(1);
    }

    //Applies stat changes to damage value
    public int ApplyDamageModifiers(SOMove move, Goblinmon attacker)
    {
        //Creates random modifier to multiply damage with
        float decider = FindObjectOfType<BattleSystem>().rnd.Next(1, 16);
        float randomDamageModifier = 0.84f + decider * 0.01f; //Creates damage range of .85 and 1

        float returnDamage = move.damage;

        //Each attack point = roughly 50% more damage, defense roughly 33% reduction
        if (move.moveModifier == SOMove.MoveModifier.DEFENSE_SCALE)
            returnDamage = returnDamage * ((2 + attacker.defenseModifier) / 2) * ((3 + defenseModifier + move.statModifier) / 3);
        else returnDamage = returnDamage * ((2 + attacker.attackModifier) / 2) * ((3 + defenseModifier) / 3);
        returnDamage = (int)(returnDamage * randomDamageModifier);
        return (int) (returnDamage * GetWeaknessMultiplier(move));
    }

    public float GetWeaknessMultiplier(SOMove move)
    {
        float toReturn = 0;
        //calculate weaknesses
        foreach (SOType type in goblinData.types)
        {
            if (type.weakness.Contains(move.moveType)) toReturn += 2;
        }
        if (toReturn < 1) toReturn = 1; //if no damage modifiers return 1
        //calculate resistances
        foreach (SOType type in goblinData.types)
        {
            if (type == move.moveType) toReturn *= .5f;
        }

        return toReturn;
    }
}
