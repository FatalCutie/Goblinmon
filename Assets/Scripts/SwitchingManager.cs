using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Collections;

public class SwitchingManager : MonoBehaviour
{
    public List<SOGoblinmon> goblinmon;
    public GameObject unitButtonHolder;
    private BattleSystem bs;
    private SOGoblinmon gobData; //basically a holder to update player info

    void Awake()
    {
        PopulateUnits(); 
    }

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
    }

    public void PopulateUnits()
    {
        int i = 0;
        try
        {
            foreach (Transform go in unitButtonHolder.transform)
            {
                TextMeshProUGUI unitNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                UnitButton ub = go.GetComponent<UnitButton>();

                //Init button
                Goblinmon gob = go.gameObject.AddComponent<Goblinmon>();
                gob.goblinData = goblinmon[i];
                gob.currentHP = gob.goblinData.maxHP; //This will need to be changed
                ub.unit = goblinmon[i];
                unitNameText.text = ub.unit.gName;

                i++;
            }
        }
        catch (ArgumentOutOfRangeException) { }

    }

    public void BeginUnitSwitch(Goblinmon unit){
        StartCoroutine(SwitchUnit(unit));
    }

    public IEnumerator SwitchUnit(Goblinmon unit)
    {
        gobData = unit.goblinData;

        //Makes switching look smooth for player
        bs.dialogueText.text = "Come back " + bs.playerUnit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(2);
        bs.dialogueText.text = "Go, " + unit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);

        //Switches the active unit
        UpdatePlayerInformation();
        FindObjectOfType<BattleHUD>().SetHUD(unit);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = unit.goblinData.sprite;
        yield return new WaitForSeconds(1);

        //End the players turn
        bs.state = BattleState.ENEMYTURN;
        Debug.Log("enemy turn");
        StartCoroutine(bs.EnemyTurn());
    }

        //Updates player data after switch
        public void UpdatePlayerInformation()
    {
        Goblinmon playerGob = bs.playerUnit.GetComponent<Goblinmon>();
        playerGob.goblinData = gobData;
        playerGob.currentHP = playerGob.goblinData.maxHP; //This will need to be changed
    }
}
