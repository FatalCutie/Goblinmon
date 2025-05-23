using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SwitchingManager : MonoBehaviour
{
    public List<SOGoblinmon> goblinmon;
    public GameObject unitButtonHolder;
    private BattleSystem bs;
    private ButtonManager bm;
    private PartyStorage ps;
    private EnemyAI eAI;

    [SerializeField] private List<UnitButton> unitButtons = new List<UnitButton>(); //For easy access and saving HP

    void Awake()
    {
        //PopulateUnits(); 
    }

    void Start()
    {
        eAI = FindObjectOfType<EnemyAI>();
        bm = FindObjectOfType<ButtonManager>();
        bs = FindObjectOfType<BattleSystem>();
        ps = FindAnyObjectByType<PartyStorage>();
        PopulateUnits();
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
                if (ps.goblinmon[i] != null)
                {
                    //Create a Goblinmon script on the Unit Button to hold data
                    ub.unit = ps.goblinmon[i];
                    unitNameText.text = ub.unit.goblinData.gName;
                    ub.unitImage.sprite = ub.unit.goblinData.sprite;
                    if (!ub.unit.goblinData.isFusion) ub.fusionIcon.enabled = false;
                    else ub.fusionIcon.enabled = true;
                    ub.level.text = $"Lv. {ub.unit.goblinData.gLevel}";
                    ub.unitNumber = i;

                    //Sets HP bar values
                    ub.hp.maxValue = ub.unit.goblinData.maxHP;
                    ub.hp.value = ub.unit.currentHP;

                    unitButtons.Add(ub);
                    i++;
                }
            }
        }
        catch (ArgumentOutOfRangeException) { } //In case less units than buttons
    }

    //Switches buttons to release goblinmon (if capturing unit with full party)
    public void FlipUnitButtonFunctionality()
    {
        try
        {
            foreach (Transform go in unitButtonHolder.transform)
            {
                UnitButton ub = go.gameObject.GetComponent<UnitButton>();
                ub.buttonMode = UnitButton.ButtonMode.RELEASE;
            }
        }
        catch (NullReferenceException) { } //In case less units than buttons

    }

    public bool DoesPlayerHaveUnits()
    {
        foreach (Goblinmon go in ps.goblinmon)
        {
            //Look for a unit that isn't dead
            if (go.currentHP > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void GetNewPlayerUnit()
    {
        bm.enableSwitchingMenu();
        bm.cantClose = true;
    }

    //Makes sure player is not switching to active unit
    public void CheckUnitBeforeSwitching(Goblinmon unit)
    {
        if (unit.ID == bs.playerUnit.GetComponent<Goblinmon>().ID)
        {
            FindObjectOfType<AudioManager>().Play("damage");
            Debug.LogWarning("Cannot switch to a unit that is already active!");
        }
        else //switch unit
        {
            //Debug.Log($"Switching to {unit.goblinData.gName} which has {unit.currentHP}");
            FindObjectOfType<AudioManager>().Play("press");
            StartCoroutine(SwitchUnit(unit));
            bm.disableButtonsDuringAttack();
            bm.disableSwitchingMenuAfterSwitch();
        }

    }

    public IEnumerator SwitchUnit(Goblinmon unit)
    {
        //Makes switching look smooth for player
        if (bs.playerUnit.currentHP >= 0)
        {
            StartCoroutine(bs.ScrollText("Come back " + bs.playerUnit.goblinData.gName + "!"));
            yield return new WaitForSeconds(1);
            bs.playerAnimator.SetBool("Player Out", false);
            yield return new WaitForSeconds(.4f);
            bs.battleAnimator.SetTrigger("RetrievePlayer");
            bs.playerUIAnimator.SetBool("PanelOpen", false);
            yield return new WaitForSeconds(.5f);
        }


        //Save health data of Unit being swapped
        SavePlayerData();
        if (bs.healEveryTurn) bs.healEveryTurn = !bs.healEveryTurn;

        //Switches the active unit
        UpdatePlayerInformation(unit);
        FindObjectOfType<BattleHUD>().SetHUD(unit);
        StartCoroutine(bs.ScrollText("Go, " + unit.goblinData.gName + "!"));
        yield return new WaitForSeconds(1);
        bs.battleAnimator.SetTrigger("ThrowOutPlayer");
        yield return new WaitForSeconds(.84f);
        bs.playerAnimator.SetBool("Player Out", true);
        bs.playerUIAnimator.SetBool("PanelOpen", true);
        yield return new WaitForSeconds(1);
        if (bs.twoTurnMove) bs.twoTurnMove = null; //clear two turn move
        //End the players turn unless just switched from KO
        if (bs.state == BattleState.ENEMYTURN)
        {
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

        int i = 0;
        try
        {
            foreach (Goblinmon g in ps.goblinmon)
            {
                if (g.ID == playerUnitToSave.ID)
                {
                    break;
                }
                // Debug.Log($"Iteration {i}: ID {g.ID} does not equal {playerUnitToSave.ID}");
                i++;
            }
            ps.goblinmon[i].currentHP = playerUnitToSave.currentHP;
        }
        catch (ArgumentOutOfRangeException) { Debug.LogWarning("SWITCHING MANAGER OVERFLOW FROM SAVEPLAYERDATA()"); } //For if a new gob was caught

        //Update health of unit being switched out
    }

    //Updates player data after switch
    public void UpdatePlayerInformation(Goblinmon newData)
    {
        Goblinmon playerGob = bs.playerUnit;
        playerGob.goblinData = newData.goblinData;
        playerGob.CloneIdFrom(newData);

        playerGob.currentHP = newData.currentHP;
        playerGob.GetComponent<SpriteRenderer>().sprite = newData.goblinData.sprite;

        //Reset stat changes
        playerGob.attackModifier = 0;
        playerGob.defenseModifier = 0;
    }
}
