using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Move", order = 1)]
public class SOMove : ScriptableObject
{
    public string moveName;
    public string moveDescription;
    public SOType moveType;
    public int damage;

    //Buff a stat, or debuff an enemys stat
    [System.Serializable] public enum MoveAction { ATTACK, BUFF, DEBUFF, HEAL };
    public MoveAction moveAction; //Should this be private?
    [System.Serializable]
    public enum MoveModifier
    {
        NONE, ATTACK, DEFENSE, ATTACK_DEFENSE, RAMP, MULTI_HIT,
        TWO_TURN, RANDOM_TYPE, DEFENSE_SCALE, RECOIL, SQUID, OVER_TIME, RIGGED_MULTI_HIT
    };
    public MoveModifier moveModifier = MoveModifier.NONE; //Defaulting to none
    public int statModifier; //How many points it buffs stat
}
