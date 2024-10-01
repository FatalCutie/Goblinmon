using System;
using Unity.VisualScripting;
using UnityEngine;


public class Goblinmon : MonoBehaviour
{
    [SerializeField] public SOGoblinmon goblinData;
    public int currentHP;

    void Awake()
    {
        try
        {
            currentHP = goblinData.maxHP;
        }
        catch (NullReferenceException) { Debug.LogWarning("Goblinmon Unit did not have a data SO at creation! Please disregard if intended"); }

    }


    public bool TakeDamage(int dmg, bool weakness)
    {
        if (weakness)
        {
            dmg *= 2;
            currentHP -= dmg;
            Debug.Log(dmg);
        }
        else currentHP -= dmg;


        if (currentHP <= 0) return true;
        else return false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > goblinData.maxHP) currentHP = goblinData.maxHP;
    }

}
