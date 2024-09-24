using UnityEngine;


public class Goblinmon : MonoBehaviour
{
    [SerializeField] public SOGoblinmon goblinData;

    public bool TakeDamage(int dmg, bool weakness)
    {
        if (weakness)
        {
            goblinData.currentHP = goblinData.currentHP - dmg * 2;
        }
        else goblinData.currentHP -= dmg;


        if (goblinData.currentHP <= 0) return true;
        else return false;
    }

    public void Heal(int amount)
    {
        goblinData.currentHP += amount;
        if (goblinData.currentHP > goblinData.maxHP) goblinData.currentHP = goblinData.maxHP;
    }

}
