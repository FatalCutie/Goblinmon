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
            Debug.Log(i);
            if(i == 4) encounter.GenerateRandomEncounter();
    }

    // void OnTriggerEnter(Collider other){
    //     Debug.Log("Trigger Enter!");
    //     if(other.gameObject.CompareTag("Player")){
    //         int i = rnd.Next(0, 8);
    //         Debug.Log(i);
    //         if(i == 4) encounter.GenerateRandomEncounter();
    //     }
    // }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // Check if the player collides with a tile
    //     if(collision.collider.CompareTag("Player")){
    //         int i = rnd.Next(0, 8);
    //         Debug.Log(i);
    //         if(i == 4) encounter.GenerateRandomEncounter();
    //     }
    // }
}
