using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script is the base for each route's encounter table
public abstract class RandomEncounter : MonoBehaviour
{
    public List<SOGoblinmon> unitsOnRoute;
    public GameObject enemyPartyStoragePrefab;
    public System.Random rnd = new System.Random();

    //Decides what Goblinmon will be encountered
    //Weights will be changed based on route, hence abstract
    public abstract void GenerateRandomEncounter();

    
    public void InitializeBattle(SOGoblinmon unit){
        //Instantiate prefab with goblinmon
        GameObject go = Instantiate(enemyPartyStoragePrefab); //Instantiate Object
        Goblinmon g = CreateGoblinmon(unit, go); //Create Goblinmon unit
        go.GetComponent<EnemyPartyStorage>().PopulateEnemy(g); //Adds goblinmon to party

        //Load Scene
        FindObjectOfType<PlayerTileMovement>().movementLocked = true; //Lock player movement during transition
        FindObjectOfType<AudioManager>().Play("battle"); //Battle music needs to be trimmed, plays as scene transitions
        FindObjectOfType<SceneController>().TransitionScene("BattleScene");
        // SceneManager.LoadScene("BattleScene");
    }

    //Creates the Goblinmon component and adds it to enemyPartyStoragePrefab
    public Goblinmon CreateGoblinmon(SOGoblinmon unit, GameObject go){
        Goblinmon g = go.AddComponent<Goblinmon>();
        g.goblinData = unit;
        g.currentHP = g.goblinData.maxHP;
        return g;
    }
}
