using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPartyStorage : MonoBehaviour
{
    public EnemyPartyStorage instance;
    public List<Goblinmon> goblinmon;
    public int money;
    public string npcId;
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

    public void PopulateEnemyParty(List<Goblinmon> newUnits, int playerReward, string i)
    {
        foreach (Goblinmon u in newUnits){
            goblinmon.Add(u);
        }
        money = playerReward;
        if (npcId != null) npcId = i;
    }

    public void PopulateEnemy(Goblinmon unit)
    {
        goblinmon.Add(unit);
    }

    public void MarkNPCAsDefeated()
    {
        // Add this NPC's ID to the global list of defeated NPCs
        if (npcId != null) NPCTracker.MarkDefeated(npcId);
    }
}
