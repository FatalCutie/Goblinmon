using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartyStorage : MonoBehaviour
{
    public EnemyPartyStorage instance;
    public List<Goblinmon> goblinmon;
    public int money;
    public TriggerBattleOverworld.BattleMusic battleMusic = TriggerBattleOverworld.BattleMusic.BM_TRAINER;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            //Destroy(gameObject);
            //Override previous instance, because older session should have more up to date info
            Destroy(instance);
            instance = this;
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PopulateEnemyParty(List<Goblinmon> newUnits, int playerReward)
    {
        foreach (Goblinmon u in newUnits){
            goblinmon.Add(u);
        }
        money = playerReward;
    }

    public void PopulateEnemy(Goblinmon unit)
    {
        goblinmon.Add(unit);
    }
}
