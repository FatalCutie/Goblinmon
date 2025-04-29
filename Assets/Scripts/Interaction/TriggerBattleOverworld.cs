using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

//This script is for Trainer Battles. Check the appropriate Route RandomEncounter script
//For wild battles.
public class TriggerBattleOverworld : MonoBehaviour
{
    private List<Goblinmon> enemyTeam = new List<Goblinmon>();
    [SerializeField] private List<SOGoblinmon> enemyTeamSO;
    public GameObject enemyPartyStoragePrefab;
    public enum BattleMusic { BM_TRAINER, BM_ELITE, BM_LEGENDARY, BM_WILD };
    public BattleMusic battleMusic = BattleMusic.BM_TRAINER;
    [SerializeField] int rewardMoney = 1000;

    void Start(){
    }

    void Update()
    {
        //Triggers battle for Debug purposes
        if (Input.GetKeyDown(KeyCode.U))
        {
            TriggerBattleSequence();
            //SceneManager.LoadScene("BattleScene");
        }
    }

    void InitializeGoblinmonParty(GameObject go)
    {
        //Check if list is empty before running
        try
        {
            for (int i = 0; i < 6; i++) //party maximum is 6 units
            {
                if (enemyTeamSO[i] != null) //if a SO exists
                {
                    //Create a Goblinmon script on the Unit Button to hold data
                    //TODO instantiate goblinmon on different object then clear it later like switching manager
                    Goblinmon gob = go.AddComponent<Goblinmon>();
                    enemyTeam.Add(gob);
                    gob.goblinData = enemyTeamSO[i];
                    gob.currentHP = enemyTeamSO[i].maxHP; //set Goblinmon to max health at init. I don't think this will cause problems later down the line :clueless:
                    enemyTeam[i] = gob;
                    //Destroy(gameObject.GetComponent<Goblinmon>());
                    }
                }
            }
            catch (ArgumentOutOfRangeException) { }
    }

    public void TriggerBattleSequence(){

        //Instantiate prefab with goblinmon team
        GameObject go = Instantiate(enemyPartyStoragePrefab); //Instantiate Object
        InitializeGoblinmonParty(go); //populate Goblinmon
        go.GetComponent<EnemyPartyStorage>().PopulateEnemyParty(enemyTeam, rewardMoney); //
        go.GetComponent<EnemyPartyStorage>().battleMusic = battleMusic;

        //Load Scene
        FindObjectOfType<PlayerTileMovement>().movementLocked = true; //Lock player movement during transition
        FindObjectOfType<PlayerPositionManager>().SavePlayersPosition(); //Save players position for after battle
        switch (battleMusic)
        {
            case BattleMusic.BM_TRAINER:
                FindObjectOfType<AudioManager>().Play("battleTrainer");
                break;
            case BattleMusic.BM_ELITE:
                FindObjectOfType<AudioManager>().Play("battleElite");
                break;
            case BattleMusic.BM_LEGENDARY:
                FindObjectOfType<AudioManager>().Play("battleLegendary");
                break;
        }
        FindObjectOfType<SceneController>().TransitionScene("BattleScene");
        // SceneManager.LoadScene("BattleScene");
    }
}
