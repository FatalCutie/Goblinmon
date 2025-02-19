using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TallGrass : MonoBehaviour
{
    public System.Random rnd = new System.Random();
    public RandomEncounter encounter;

    public void RandomEncounter(){
            int i = rnd.Next(0, 8);
        //Debug.Log(i);
        if (i == 4) encounter.GenerateRandomEncounter();
    }

}
