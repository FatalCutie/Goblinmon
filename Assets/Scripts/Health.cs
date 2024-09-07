using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int maxHealth; //this may need to be public later
    public int health;

    public void dealDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            //TODO: Come back to this
            Debug.Log("Object is dead!");
        }
    }
}
