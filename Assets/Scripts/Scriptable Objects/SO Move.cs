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
    public enum MoveAction { Attack, Buff, Debuff }
    public enum StatBuffed { Attack, Defense, Dodge }
}
