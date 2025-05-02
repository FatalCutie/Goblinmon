using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class FusionButton : MonoBehaviour
{
    public enum ButtonMode {OVERWORLD, FUSION}
    public ButtonMode buttonMode = ButtonMode.OVERWORLD; //Default to overworld
    public GameObject unitButtonHolder;
    [SerializeField] private Image fusionButtonImage;
    [SerializeField] private Goblinmon selectedUnit1;
    [SerializeField] private Goblinmon selectedUnit2;
    [SerializeField] private FusionCalculator fusionCalculator;
    [SerializeField] private PartyStorage partyManager;
    [SerializeField] private SwitchingButton sb;

    void Start()
    {
        gameObject.SetActive(true);
        partyManager = FindObjectOfType<PartyStorage>();
    }

    //Switches unit buttons and fusion buttons between Overworld and Fusion mode
    public void SwitchButtonMode(){
        switch(buttonMode){
            case ButtonMode.OVERWORLD:
                if (FindObjectOfType<PlayerPositionManager>().fusionItems <= 0)
                {
                    StartCoroutine(FindObjectOfType<OverworldUI>().FlashFusionItemsRed());
                    StartCoroutine(FlashFusionButtonRed());
                    return;
                }
                //Switch unit buttons to fusion mode
                ButtonCheck(); //Check to see if in switching mode
                foreach(Transform t in unitButtonHolder.transform){
                    if(t.GetComponent<UnitButton>()){
                        UnitButton ub = t.GetComponent<UnitButton>();
                        ub.buttonMode = UnitButton.ButtonMode.FUSION;
                    }
                }
                //Switch button color
                Image b = gameObject.GetComponent<Button>().GetComponent<Image>();
                b.color = Color.yellow;
                buttonMode = ButtonMode.FUSION; //Switch self to match unit buttons
                return;
            case ButtonMode.FUSION:
                foreach(Transform t in unitButtonHolder.transform){
                    if(t.GetComponent<UnitButton>()){
                        UnitButton ub = t.GetComponent<UnitButton>();
                        ub.buttonMode = UnitButton.ButtonMode.OVERWORLD;
                    }
                }
                buttonMode = ButtonMode.OVERWORLD; //Switch self to match unit buttons
                //Switch button color
                Image c = gameObject.GetComponent<Button>().GetComponent<Image>();
                c.color = Color.white;
                //Clear selected units
                selectedUnit1 = null;
                selectedUnit2 = null;
                FlipButtonColor();
                return;
        }
    }

    //Add a unit to be fused. When there are two begin fusion process
    public void SelectUnitForFusion(Goblinmon g){
        if (selectedUnit1 == null)
        {
            if (!g.goblinData.isFusion)
            {
                selectedUnit1 = g;
                FlipButtonColor();
            }
            else FindObjectOfType<AudioManager>().Play("damage");

        }
        else if (g == selectedUnit1)
        {
            selectedUnit1 = null; //deselect unit
            FlipButtonColor();
        }
        else if (selectedUnit2 == null && g != selectedUnit1)
        {
            if (!g.goblinData.isFusion)
            {
                selectedUnit2 = g;
                FlipButtonColor();
                SOGoblinmon fusion = fusionCalculator.CalculateFusionUnit(selectedUnit1.goblinData, selectedUnit2.goblinData); //get fusion
                if (!fusion)
                {
                    Debug.LogWarning("Fusion calculator returned null. Please check if selected units are valid!");
                    SwitchButtonMode(); //Exit fusion mode
                    return; //I don't think this return does anything but I like how it looks
                }
                Goblinmon unit = InitializeFusedGoblinmon(fusion); //initialize fusion
                FuseUnits(selectedUnit1, selectedUnit2, unit);
                SwitchButtonMode(); //Exit fusion mode after fusion}
            }
            else FindObjectOfType<AudioManager>().Play("damage");
        }
    }

    //Initializes fused goblinmon on PartyManager GO
    public Goblinmon InitializeFusedGoblinmon(SOGoblinmon g){
        Goblinmon gob = partyManager.gameObject.AddComponent<Goblinmon>();
        //partyManager.goblinmon.Add(gob);
        gob.goblinData = g;
        gob.currentHP = g.maxHP;
        if(g.maxHP <= 0) Debug.LogWarning($"{g.gName}'s HP is 0, please check the ScriptableObject!");
        gob.friendOrFoe = Goblinmon.FriendOrFoe.FRIEND; 
        return gob;
    }

    //Clears fused units from goblinmon list, adds new unit to party
    public void FuseUnits(Goblinmon fusion1, Goblinmon fusion2, Goblinmon fused){
        int i = 7;
        int j = 0; //Tracks where to insert new unit
        //Search for goblinmon ID's in list and trim them
        foreach(Goblinmon g in partyManager.goblinmon){
            if(g.ID == fusion1.ID || g.ID == fusion2.ID){
                Destroy(g);
                if(j < i) i = j;
                //Debug.Log($"Removed the fused unit {g.goblinData.gName}");
            }
            j++;
        }
        FindObjectOfType<PlayerPositionManager>().fusionItems--;
        partyManager.goblinmon[i] = fused; //Add fusion to party
        //Debug.Log("Clearing list!");
    }

    private void ButtonCheck(){
        if(sb.buttonMode == SwitchingButton.ButtonMode.SWITCH){
            sb.SwitchButtonMode();
        }
    }

    private void FlipButtonColor()
    {
        foreach (Transform t in unitButtonHolder.transform)
        {
            //Structured for readability not optimization
            try
            {
                if (t.GetComponent<UnitButton>())
                {
                    Image b = t.GetComponent<Button>().GetComponent<Image>();
                    if (b.color != Color.white)
                    {
                        b.color = Color.white;
                        return;
                    }
                    else if (selectedUnit1 && t.GetComponent<UnitButton>().unit.ID == selectedUnit1.ID)
                    {
                        if (b.color != Color.yellow) b.color = Color.yellow;
                    }

                }
            }
            catch (NullReferenceException) { } //I'm not quite sure why this is throwing

        }
    }

    private IEnumerator FlashFusionButtonRed()
    {
        fusionButtonImage.color = Color.red;
        yield return new WaitForSeconds(.2f);
        fusionButtonImage.color = Color.white;
    }
}
