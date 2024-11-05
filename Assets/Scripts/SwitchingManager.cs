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
    [SerializeField] private List<UnitButton> unitButtons = new List<UnitButton>();
    private SOGoblinmon gobData; //holder to update player info

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
                    //Create a Goblinmon script on the Unit Button to hold data
                    Goblinmon gob = go.gameObject.AddComponent<Goblinmon>();
                    gob.goblinData = goblinmon[i];
                    ub.unit = goblinmon[i];
                    unitNameText.text = ub.unit.gName;
                    gob.currentHP = gob.goblinData.maxHP; //Set units to Max HP at start of battle
                    unitButtons.Add(ub);
                    i++;
                }

            }
        }
        catch (ArgumentOutOfRangeException) { } //In case less units than buttons

    }

    public bool DoesPlayerHaveUnits()
    {
        foreach (Transform go in unitButtonHolder.transform)
        {
            //Initialize switching buttons and units tied to them
            if (go.GetComponent<Goblinmon>().currentHP >= 0)
            {
                return true;
            }
        }
        return false;
    }

    public void GetNewPlayerUnit()
    {
        bm.enableSwitchingMenu();
    }

    //Makes sure player is not switching to active unit
    public void CheckUnitBeforeSwitching(Goblinmon unit)
    {
        if (unit.goblinData == bs.playerUnit.GetComponent<Goblinmon>().goblinData)
        {
            FindObjectOfType<AudioManager>().Play("damage");
            Debug.LogWarning("Cannot switch to a unit that is already active!");

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
        //Makes switching look smooth for player
        bs.dialogueText.text = "Come back " + bs.playerUnit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(2);
        bs.dialogueText.text = "Go, " + unit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);

        //Save health data of Unit being swapped
        SavePlayerData();

        //Switches the active unit
        UpdatePlayerInformation(unit);
        FindObjectOfType<BattleHUD>().SetHUD(unit);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = unit.goblinData.sprite;
        eAI.UpdatePlayerUnit(unit); //TODO: Update version of player that is not used in damage calculations
        yield return new WaitForSeconds(1);

        //End the players turn unless just switched from KO
        if (bs.state == BattleState.ENEMYTURN)
        {
            //eAI.UpdatePlayerUnit(null);
            bs.PlayerTurn();
            bm.enableBasicButtonsOnPress();
        }
        else
        {
            bs.state = BattleState.ENEMYTURN;
            eAI.FindOptimalOption();
        }

    }

    //Saves active units current HP
    public void SavePlayerData()
    {
        Goblinmon playerUnitToSave = bs.playerUnit.GetComponent<Goblinmon>();
        int unitID = 0;
        //Gets the index for unit being switched out to save data
        //NOTE: This does not account for multiple Goblinmon of the same type being in game!!
        foreach (SOGoblinmon un in goblinmon)
        {
            if (un.gName == playerUnitToSave.goblinData.gName)
            {
                break;
            }
            unitID++;
        }
        //Update health of unit being switched out
        unitButtons[unitID].GetComponent<Goblinmon>().currentHP = playerUnitToSave.currentHP;
    }

    //Updates player data after switch
    public void UpdatePlayerInformation(Goblinmon newData)
    {
        //Sets Goblinmon script to be Unit button, not player unit
        //TODO: Fix that
        //SOLUTIONS: Offload player Goblinmon onto an asset like Enemy and use ID system for buttons?
        //Add Unit Buttons to Array and call from there?
        Goblinmon playerGob = bs.playerUnit.GetComponent<Goblinmon>();
        playerGob.goblinData = newData.goblinData;
        playerGob.currentHP = newData.currentHP;

        //Reset stat changes
        playerGob.attackModifier = 0;
        playerGob.defenseModifier = 0;
    }
}
