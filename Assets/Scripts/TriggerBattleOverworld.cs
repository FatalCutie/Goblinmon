using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class TriggerBattleOverworld : MonoBehaviour
{
    private List<Goblinmon> enemyTeam = new List<Goblinmon>();
    [SerializeField] private List<SOGoblinmon> enemyTeamSO;
    public GameObject enemyPartyStoragePrefab;

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

     void InitilizeGoblinmonParty(GameObject go)
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

    void TriggerBattleSequence(){

        //Instantiate prefab with goblinmon team
        GameObject go = Instantiate(enemyPartyStoragePrefab); //Instantiate Object
        InitilizeGoblinmonParty(go); //populate Goblinmon
        go.GetComponent<EnemyPartyStorage>().PopulateEnemyParty(enemyTeam); //

        //Load Scene
        //TODO: Battle transition fade
        SceneManager.LoadScene("BattleScene");
    }
}
