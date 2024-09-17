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
    //TODO: MoveAction changes move being used to either do damage,
    //Buff a stat, or debuff an enemys stat
    [System.Serializable] public enum MoveAction { ATTACK, BUFF, DEBUFF }
    public MoveAction moveAction; //Should this be private?
    [System.Serializable] public enum StatBuffed { NONE, ATTACK, DEFENSE, DODGE }
    public StatBuffed statBuffed = StatBuffed.NONE; //Defaulting to none
}
