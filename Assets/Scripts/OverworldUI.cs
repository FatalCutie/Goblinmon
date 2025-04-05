using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class OverworldUI : MonoBehaviour
{
    [SerializeField] private Animator UnitMenuAnimator;
    private bool UnitMenuOpen;
    [SerializeField] private Animator ItemUIAnimator;
    private bool ItemUIOpen;
    public GameObject unitMenu;
    public GameObject fusionItemUI;
    public GameObject catchingItemUI;
    public PlayerTileMovement player;
    public PartyStorage partyStorage;

    void Start()
    {
        partyStorage = FindObjectOfType<PartyStorage>();
    }
    void Update()
    {
        //Open and close unit menu panel
        if(Input.GetKeyDown(KeyCode.Tab)){
            switch(UnitMenuOpen){
                case false:
                    OpenPanel();
                    return;
                case true:
                    ClosePanel();
                    return;
            }
        }
    }

    //Opens the unit and item panel
    public void OpenPanel()
    {
        //if (!dataPopulated) UpdateUnitInformation(); //Populate buttons with party
        partyStorage.menuOpen = true;
        UnitMenuAnimator.SetBool("PanelOpen", true);
        if (!ItemUIOpen) ItemUIAnimator.SetBool("ItemsOpen", true); //Open items if not already open from AFK
        //Lock player
        player.movementLocked = true;
        UnitMenuOpen = true;
    }

    //Closes the unit and item panel
    public void ClosePanel()
    {
        partyStorage.menuOpen = false;
        UnitMenuAnimator.SetBool("PanelOpen", false);
        ItemUIAnimator.SetBool("ItemsOpen", false);
        player.idleTimer = 0f; //Reset idle so item's dont open immediately after closing menu
        player.playerIsIdle = false;
        //Unlock Player
        player.movementLocked = false;
        UnitMenuOpen = false;
    }

    //Populate overworld unit buttons with data
    public void UpdateUnitInformation()
    {
        int i = 0;
        foreach (Transform go in unitMenu.transform)
        {
            if(!go.gameObject.GetComponent<UnitButton>()) return; //return if button is not a unit button

            TextMeshProUGUI unitNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
            UnitButton ub = go.GetComponent<UnitButton>();

            //Initialize switching buttons and units tied to them
            try
            {
                if (partyStorage.goblinmon[i] != null)
                {
                    //Create a Goblinmon script on the Unit Button to hold data
                    ub.unit = partyStorage.goblinmon[i];
                    unitNameText.text = ub.unit.goblinData.gName;
                    ub.unitImage.sprite = ub.unit.goblinData.sprite; //update preview sprite
                    if (!ub.unit.goblinData.isFusion) ub.fusionIcon.enabled = false;
                    else ub.fusionIcon.enabled = true;
                    ub.level.text = $"Lv. {ub.unit.goblinData.gLevel}";
                    ub.unitNumber = i;

                    //Sets HP bar values
                    ub.hp.maxValue = ub.unit.goblinData.maxHP;
                    ub.hp.value = ub.unit.currentHP;
                    i++;
                }
            }
            catch (ArgumentOutOfRangeException) { go.gameObject.SetActive(false); } //Disable excess buttons
        }
    }

    public void OpenItemMenuOnIdle(){
        UpdateItemUI();
        ItemUIAnimator.SetBool("ItemsOpen", true);
    }

    public void CloseItemMenuOnIdle(){
        UpdateItemUI();
        ItemUIAnimator.SetBool("ItemsOpen", false);
    }

    public void UpdateItemUI()
    {
        //TODO: Implement Items
    }


}
