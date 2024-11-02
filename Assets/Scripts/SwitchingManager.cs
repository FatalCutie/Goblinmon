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
    private EnemyAI eAI;
    private SOGoblinmon gobData; //holder to update player info
    private int activeUnitID;

    void Awake()
    {
        PopulateUnits(); 
    }

    void Start()
    {
        eAI = FindObjectOfType<EnemyAI>();
        bm = FindObjectOfType<ButtonManager>();
        bs = FindObjectOfType<BattleSystem>();
    }

    //Populates the switching menu
    public void PopulateUnits()
    {
        activeUnitID = 0;
        int i = 0;
        try
        {
            foreach (Transform go in unitButtonHolder.transform)
            {
                TextMeshProUGUI unitNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                UnitButton ub = go.GetComponent<UnitButton>();

                //Initialize switching buttons and units tied to them
                if (goblinmon[i] != null)
                {
                    Goblinmon gob = go.gameObject.AddComponent<Goblinmon>();
                    gob.goblinData = goblinmon[i];
                    gob.goblinData.InitilizeGoblinmonHP();
                    ub.unit = goblinmon[i];
                    unitNameText.text = ub.unit.gName;
                    ub.ID = i;
                    i++;
                }

            }
        }
        catch (ArgumentOutOfRangeException) { } //In case less units than buttons

    }

    //Makes sure player is not switching to active unit
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

    //TODO: Save current SO to proper button when switching so health is persistant between switches
    public IEnumerator SwitchUnit(Goblinmon unit)
    {
        //Makes switching look smooth for player
        bs.dialogueText.text = "Come back " + bs.playerUnit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(2);
        bs.dialogueText.text = "Go, " + unit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);

        //Switches the active unit
        UpdatePlayerInformation(unit.goblinData);
        FindObjectOfType<BattleHUD>().SetHUD(unit);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = unit.goblinData.sprite;
        //TODO: Delay this so the opponent doesn't just lead with something super effective after a switch
        eAI.UpdatePlayerUnit(unit);
        yield return new WaitForSeconds(1);

        //End the players turn
        bs.state = BattleState.ENEMYTURN;
        eAI.FindOptimalOption();
    }

    //Updates player data after switch
    public void UpdatePlayerInformation(SOGoblinmon newData)
    {
        Goblinmon playerGob = bs.playerUnit.GetComponent<Goblinmon>();
        playerGob.goblinData = newData;
    }
}
