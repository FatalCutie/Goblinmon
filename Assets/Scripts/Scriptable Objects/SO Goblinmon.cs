using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Goblinmon", order = 1)]
public class SOGoblinmon : ScriptableObject
{
    public string gName;
    public int gLevel;

    public Sprite sprite;
    public int maxHP;


    public SOType type;
    public List<SOMove> moveset;

    public List<SOType> weakness; //Take more damage from

}
