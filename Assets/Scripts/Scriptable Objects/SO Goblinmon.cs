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
    // public int currentHP;

    public SOType type;
    public List<SOMove> moveset;

    //Reset current unit HP to max health, called during initilization in SwitchingManager
    // public void InitilizeGoblinmonHP()
    // {
    //     currentHP = maxHP;
    // }

}
