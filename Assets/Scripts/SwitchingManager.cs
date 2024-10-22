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
    private ButtonManager bm;
    private SOGoblinmon gobData; //basically a holder to update player info

    void Awake()
    {
        PopulateUnits(); 
    }

    void Start()
    {
        bm = FindObjectOfType<ButtonManager>();
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

    public void CheckUnitBeforeSwitching(Goblinmon unit)
    {
        if (unit.goblinData == bs.playerUnit.GetComponent<Goblinmon>().goblinData)
        { //This will need to be adjusted if you can catch multiple of the same Goblinmon
            //Maybe introduce id system?
            FindObjectOfType<AudioManager>().Play("damage");
            Debug.LogWarning("Cannot switch to a unit that is already active!");
            //Makes sure selected unit isn't active unit
        }
        else //switch unit
        {
            FindObjectOfType<AudioManager>().Play("press");
            StartCoroutine(SwitchUnit(unit));
            bm.disableButtonsDuringAttack();
            bm.disableSwitchingMenuAfterSwitch();
        }

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
