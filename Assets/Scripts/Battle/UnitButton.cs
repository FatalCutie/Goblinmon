using System.Data;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

//Populated in Switching Manager
public class UnitButton : MonoBehaviour
{
    public Goblinmon unit;
    public int unitNumber;
    private TextMeshProUGUI text;
    private SwitchingManager sm;
    public Slider hp;
    public enum ButtonMode { SWITCH, RELEASE }
    public bool activeUnit = false;
    public ButtonMode buttonMode = ButtonMode.SWITCH;

    void Start()
    {
        sm = FindObjectOfType<SwitchingManager>();
        text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

    }

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (unit == null && buttonMode != ButtonMode.RELEASE && text.text != "")
        {
            text.text = "";
            hp.gameObject.SetActive(false);
        }

        //This is woefully inefficient
        //Oh well!
        if (unit.ID == FindObjectOfType<BattleSystem>().playerUnit.ID
            && hp.value != FindObjectOfType<BattleSystem>().playerUnit.currentHP)
            hp.value = FindObjectOfType<BattleSystem>().playerUnit.currentHP;

    }

    public void SwitchUnitOnPress()
    {
        if (buttonMode == ButtonMode.SWITCH)
        {
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
        }
        else if (buttonMode == ButtonMode.RELEASE)
        {
            FindObjectOfType<CatchSystem>().BeginReleaseUnit(unitNumber);
        }
    }
}
