using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Type", order = 1)]
public class SOType : ScriptableObject
{
    //Types give special passives?

    public List<SOType> weakness; //Take more damage from

    //Returns the damage multiplier for weakness

}
