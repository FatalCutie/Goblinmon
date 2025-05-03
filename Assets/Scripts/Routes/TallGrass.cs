using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TallGrass : MonoBehaviour
{
    public System.Random rnd = new System.Random();
    public RandomEncounter encounter;
    public int encounterChance = 8;

    public void RandomEncounter(){
        int i = rnd.Next(0, encounterChance);
        // Debug.Log(i);
        if (i == 1 && FindObjectOfType<PlayerTileMovement>().canWildEncounter) encounter.GenerateRandomEncounter();
    }

    private void ForceRandomEncounter()
    {
        encounter.GenerateRandomEncounter();
    }

}
