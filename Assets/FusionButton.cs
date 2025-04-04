using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class FusionButton : MonoBehaviour
{
    public enum ButtonMode {OVERWORLD, FUSION}
    public ButtonMode buttonMode = ButtonMode.OVERWORLD; //Default to overworld
    public GameObject UnitButtonHolder;
    [SerializeField] private Goblinmon selectedUnit1;
    [SerializeField] private Goblinmon selectedUnit2;
    [SerializeField] private FusionCalculator fusionCalculator;
    [SerializeField] private PartyStorage partyManager;

    void Start()
    {
        gameObject.SetActive(true);
    }

    public void SwitchButtonMode(){
        switch(buttonMode){
            case ButtonMode.OVERWORLD:
                //Switch unit buttons to fusion mode
                foreach(Transform t in UnitButtonHolder.transform){
                    if(t.GetComponent<UnitButton>()){
                        UnitButton ub = t.GetComponent<UnitButton>();
                        ub.buttonMode = UnitButton.ButtonMode.FUSION;
                        //TODO: Change button color tint
                    }
                }
                buttonMode = ButtonMode.FUSION; //Switch self to match unit buttons
                Debug.Log("Switched to Fusion Mode!");
                return;
            case ButtonMode.FUSION:
                foreach(Transform t in UnitButtonHolder.transform){
                    if(t.GetComponent<UnitButton>()){
                        UnitButton ub = t.GetComponent<UnitButton>();
                        ub.buttonMode = UnitButton.ButtonMode.OVERWORLD;
                        //TODO: Change button color tint
                    }
                }
                buttonMode = ButtonMode.OVERWORLD; //Switch self to match unit buttons
                //Clear selected units
                selectedUnit1 = null;
                selectedUnit2 = null;
                Debug.Log("Switched out of Fusion Mode!");
                return;
        }
    }

    public void SelectUnitForFusion(Goblinmon g){
        if (selectedUnit1 == null)
        {
            selectedUnit1 = g;
        }
        else if (selectedUnit2 == null && g != selectedUnit1)
        {
            selectedUnit2 = g;
            SOGoblinmon fusion = fusionCalculator.CalculateFusionUnit(selectedUnit1.goblinData, selectedUnit2.goblinData); //get fusion
            if(!fusion){
                Debug.LogWarning("Fusion calculator returned null. Please check if selected units are valid!");
                return; //I don't think this return does anything but I like how it looks
            } 
            Goblinmon unit = InitializeFusedGoblinmon(fusion); //initialize fusion
            RemoveFusedUnits(selectedUnit1, selectedUnit2); 
        }
    }

    //Initializes fused goblinmon on PartyManager
    public Goblinmon InitializeFusedGoblinmon(SOGoblinmon g){
        Goblinmon gob = partyManager.gameObject.AddComponent<Goblinmon>();
        partyManager.goblinmon.Add(gob);
        gob.goblinData = g;
        gob.currentHP = g.maxHP;
        gob.friendOrFoe = Goblinmon.FriendOrFoe.FRIEND; 
        return gob;
    }

    //Clears fused units from goblinmon, adds new unit
    public void RemoveFusedUnits(Goblinmon u1, Goblinmon u2){
        //Search for goblinmon ID's in list and trim them
        partyManager.goblinmon.RemoveAll(item => item == null || item.Equals(null));; //Remove new empty units from list
    }
}
