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
    [System.Serializable] public enum MoveAction { ATTACK, BUFF, DEBUFF }
    public MoveAction moveAction; //Should this be private?
    [System.Serializable] public enum StatModified { NONE, ATTACK, DEFENSE }
    public StatModified statModified = StatModified.NONE; //Defaulting to none
    public int statModifier; //How many points it buffs stat
}
