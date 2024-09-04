using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Type", order = 1)]
public class SOType : ScriptableObject
{
    //Types give special passives?

    public List<SOType> strength; //Deal more damage to

    //This list is redundent
    //public List<SOType> weakness;

    public void compareTypes(SOType other)
    {
        if (strength.Contains(other))
        {
            //Is Stronger
        }
    }
}
