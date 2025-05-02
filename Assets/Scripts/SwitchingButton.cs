using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//In the future this could be merged into FusionButton, but for now this is functional, if unoptimal
public class SwitchingButton : MonoBehaviour
{
    [SerializeField] private Goblinmon selectedUnit1;
    [SerializeField] private Goblinmon selectedUnit2;
    [SerializeField] private PartyStorage partyStorage;
    public enum ButtonMode {OVERWORLD, SWITCH}
    public ButtonMode buttonMode = ButtonMode.OVERWORLD; //Default to overworld
    public GameObject unitButtonHolder;
    [SerializeField] private FusionButton fb;

    void Start()
    {
        partyStorage = FindObjectOfType<PartyStorage>();
    }

        public void SwitchButtonMode(){
        switch(buttonMode){
            case ButtonMode.OVERWORLD:
                //Switch unit buttons to fusion mode
                ButtonCheck(); //Check to see if in fusion mode
                foreach(Transform t in unitButtonHolder.transform){
                    if(t.GetComponent<UnitButton>()){
                        UnitButton ub = t.GetComponent<UnitButton>();
                        ub.buttonMode = UnitButton.ButtonMode.MOVE;
                        //TODO: Change button color tint
                    }
                }
                //Switch button color
                Image b = gameObject.GetComponent<Button>().GetComponent<Image>();
                b.color = Color.yellow;
                buttonMode = ButtonMode.SWITCH; //Switch self to match unit buttons
                return;
            case ButtonMode.SWITCH:
                foreach(Transform t in unitButtonHolder.transform){
                    if(t.GetComponent<UnitButton>()){
                        UnitButton ub = t.GetComponent<UnitButton>();
                        ub.buttonMode = UnitButton.ButtonMode.OVERWORLD;
                        //TODO: Change button color tint
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

    public void PickPartyMembersToSwitch(Goblinmon g){
        if (selectedUnit1 == null)
        {
            //Debug.Log("First unit picked!");
            selectedUnit1 = g;
            FlipButtonColor();
        }
        else if (g == selectedUnit1)
        {
            selectedUnit1 = null; //deselect unit
            FlipButtonColor();
        }
        else if (selectedUnit2 == null && g != selectedUnit1)
        {
            //Debug.Log("Second unit picked!");
            selectedUnit2 = g;
            SwitchUnitsInMenu();
        }
    }

    public void SwitchUnitsInMenu(){
        int i = 0; //unit 1's list position
        int j = 0; //unit 2's list position
        int k = 0; //temp var

        //Loop through unit buttons to find positions of units to switch
        foreach(Goblinmon g in partyStorage.goblinmon){
            //Debug.Log(k);
            if(g.ID == selectedUnit1.ID) i = k;
            else if(g.ID == selectedUnit2.ID) j = k;
            k++;
        }

        //Switch Units
        partyStorage.goblinmon[i] = selectedUnit2;
        partyStorage.goblinmon[j] = selectedUnit1;
        //Clear selected units, remain in switching mode
        selectedUnit1 = null;
        selectedUnit2 = null;
        FlipButtonColor();

        //SwitchButtonMode(); //reset buttons
    }

    private void ButtonCheck(){
        if(fb.buttonMode == FusionButton.ButtonMode.FUSION){
            fb.SwitchButtonMode();
        }
    }

    private void FlipButtonColor()
    {
        foreach (Transform t in unitButtonHolder.transform)
        {
            //Structured for readability not optimization
            if (t.GetComponent<UnitButton>())
            {
                Image b = t.GetComponent<Button>().GetComponent<Image>();
                try
                {
                    if (b.color != Color.white)
                    {
                        b.color = Color.white;
                        return;
                    }
                    else if (selectedUnit1 != null && t.GetComponent<UnitButton>().unit.ID == selectedUnit1.ID)
                    {
                        if (b.color != Color.yellow) b.color = Color.yellow;
                    }
                }
                catch (NullReferenceException) { } //I'm not quite sure why this is throwing.


            }
        }
    }
}
