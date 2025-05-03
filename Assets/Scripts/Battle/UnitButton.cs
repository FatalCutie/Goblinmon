using System.Data;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

//Populated in Switching Manager
public class UnitButton : MonoBehaviour
{
    public Goblinmon unit;
    public int unitNumber;
    private TextMeshProUGUI text;
    public TextMeshProUGUI level;
    private SwitchingManager sm;
    public Slider hp;
    public enum ButtonMode { SWITCH, RELEASE, OVERWORLD, FUSION, MOVE, TITLE_SCREEN }
    public Image unitImage;

    public bool activeUnit = false;
    public Image fusionIcon;
    public ButtonMode buttonMode = ButtonMode.SWITCH;

    void Start()
    {
        sm = FindObjectOfType<SwitchingManager>();
        text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

    }

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (unit == null && buttonMode == ButtonMode.SWITCH && text.text != "")
        {
            text.text = "";
            hp.gameObject.SetActive(false);
            fusionIcon.gameObject.SetActive(false);
            unitImage.gameObject.SetActive(false);
            level.gameObject.SetActive(false);
        }

        //This is woefully inefficient
        //Oh well!
        //I CHANGED THIS. IF BUTTONS ACTING WEIRD IN BATTLE LOOK AT THIS
        if (buttonMode == ButtonMode.SWITCH || buttonMode == ButtonMode.RELEASE) //If we're in a battle
        {
            if (unit != null && unit.ID == FindObjectOfType<BattleSystem>().playerUnit.ID
                && hp.value != FindObjectOfType<BattleSystem>().playerUnit.currentHP)
            {
                //Debug.Log("HP does not line up with internal! Fixing!");
                hp.value = FindObjectOfType<BattleSystem>().playerUnit.currentHP;
            }
        }

        if (buttonMode == ButtonMode.TITLE_SCREEN) UpdateUnitButtonDisplay();

    }

    public void UnitButtonAction()
    {
        switch(buttonMode){
            case ButtonMode.SWITCH:
                //Don't switch if no attached unit or attached unit is dead
                if (unit == null || unit.currentHP <= 0)
                {
                    FindObjectOfType<AudioManager>().Play("damage");
                }
                else
                {
                    // Debug.Log($"Switching to {unit.goblinData.gName} which has {unit.currentHP}!");
                    sm.CheckUnitBeforeSwitching(unit);
                }
            break;
            case ButtonMode.RELEASE:
                FindObjectOfType<CatchSystem>().BeginReleaseUnit(unitNumber);
            break;
            case ButtonMode.FUSION:
                AddUnitToFusionButton();
            break;
            case ButtonMode.MOVE:
                AddUnitToMovingButton();
                break;
            case ButtonMode.TITLE_SCREEN:
                //Add unit to party
                Goblinmon g = FindObjectOfType<PartyStorage>().AddComponent<Goblinmon>();
                g.goblinData = unit.goblinData;
                g.currentHP = g.goblinData.maxHP;
                FindObjectOfType<PartyStorage>().goblinmon.Add(g);
                FindObjectOfType<TitleScreen>().LoadIntoGame();
                //Transition to overworld
                break;
        }
    }

    public void AddUnitToFusionButton(){
        FindObjectOfType<FusionButton>().SelectUnitForFusion(unit);
    }

    public void AddUnitToMovingButton(){
        FindObjectOfType<SwitchingButton>().PickPartyMembersToSwitch(unit);
    }
    public void UpdateUnitButtonDisplay()
    {
        text.text = unit.goblinData.gName;
        unitImage.sprite = unit.goblinData.sprite; //update preview sprite
        level.text = $"Lv. {unit.goblinData.gLevel}";
    }
}
