using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
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
