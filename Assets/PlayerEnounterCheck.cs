using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnounterCheck : MonoBehaviour
{
    public bool inGrass;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tall Grass"))
        {
            Debug.Log("Movepoint Entering tall grass!");
            inGrass = true;

        }
    }

        private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tall Grass"))
        {
            Debug.Log("Movepoint Exiting tall grass!");
            inGrass = false;

        }
    }
}
