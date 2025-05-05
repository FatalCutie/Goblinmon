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
    public GameObject shopUI;
    public PlayerTileMovement player;
    public PartyStorage partyStorage;
    [SerializeField] private FusionButton fb;
    [SerializeField] private SwitchingButton sb;
    public TextMeshProUGUI fusionItems;
    public UnityEngine.UI.Image fusionItemsImage;
    private bool itemsVisable;
    public TextMeshProUGUI captureItems;
    public PlayerPositionManager ppm;

    void Start()
    {
        partyStorage = FindObjectOfType<PartyStorage>();
        shopUI.SetActive(false);
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

        if (!ppm) ppm = FindObjectOfType<PlayerPositionManager>();
        if (itemsVisable && Int32.Parse(fusionItems.text) != ppm.fusionItems) fusionItems.text = $"{ppm.fusionItems}";
        if (itemsVisable && Int32.Parse(captureItems.text) != ppm.captureItems) captureItems.text = $"{ppm.captureItems}";
    }

    //Opens the unit and item panel
    public void OpenPanel()
    {
        if (shopUI.activeSelf) return; //can't open menu in shop
        if (FindObjectOfType<PlayerTileMovement>().movementLocked) return;
        //if (!dataPopulated) UpdateUnitInformation(); //Populate buttons with party
        partyStorage.menuOpen = true;
        UnitMenuAnimator.SetBool("PanelOpen", true);
        if (!ItemUIOpen) ItemUIAnimator.SetBool("ItemsOpen", true); //Open items if not already open from AFK
        //Lock player
        player.movementLocked = true;
        UnitMenuOpen = true;
        itemsVisable = true;
    }

    //Closes the unit and item panel
    public void ClosePanel()
    {
        //Uncheck buttons when closing menu
        if (fb.buttonMode == FusionButton.ButtonMode.FUSION) fb.SwitchButtonMode();
        else if (sb.buttonMode == SwitchingButton.ButtonMode.SWITCH) sb.SwitchButtonMode();

        partyStorage.menuOpen = false;
        UnitMenuAnimator.SetBool("PanelOpen", false);
        ItemUIAnimator.SetBool("ItemsOpen", false);
        player.idleTimer = 0f; //Reset idle so item's dont open immediately after closing menu
        player.playerIsIdle = false;
        //Unlock Player
        player.movementLocked = false;
        UnitMenuOpen = false;
        itemsVisable = false;
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

    public void OpenItemMenuOnIdle()
    {
        ItemUIAnimator.SetBool("ItemsOpen", true);
        itemsVisable = true;
    }

    public void CloseItemMenuOnIdle()
    {
        ItemUIAnimator.SetBool("ItemsOpen", false);
        itemsVisable = false;
    }

    public void OpenShopUI()
    {
        if (FindObjectOfType<PlayerTileMovement>().movementLocked) return;
        shopUI.SetActive(true);
        FindObjectOfType<PlayerTileMovement>().movementLocked = true;
        FindObjectOfType<ShopButton>().RefreshPlayerMoneyTotal();
    }

    public void CloseShopUI()
    {
        shopUI.SetActive(false);
        FindObjectOfType<AudioManager>().Play("press");
        FindObjectOfType<PlayerTileMovement>().movementLocked = false;
    }

    public IEnumerator FlashFusionItemsRed()
    {
        fusionItemsImage.color = Color.red;
        FindObjectOfType<AudioManager>().Play("damage");
        yield return new WaitForSeconds(.2f);
        fusionItemsImage.color = Color.white;
    }


}
