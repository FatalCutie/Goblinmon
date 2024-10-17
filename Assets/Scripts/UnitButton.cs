using TMPro;
using System.Collections;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    public SOGoblinmon unit;
    private TextMeshProUGUI text;
    private BattleSystem bs;
    private ButtonManager bm;
    private SwitchingManager sm;

    void Start()
    {
        sm = FindObjectOfType<SwitchingManager>();
        bs = FindObjectOfType<BattleSystem>();
        bm = FindObjectOfType<ButtonManager>();
        text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); //this can be cleaned up(?)
    }

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (unit == null)
        {
            text.text = "";
        }
    }

    public void SwitchUnitOnPress()
    {
        sm.CheckUnitBeforeSwitching(this.GetComponent<Goblinmon>());
    }

    //What does this do?
    public void UpdatePlayerInformation()
    {
        Goblinmon playerGob = bs.playerUnit.GetComponent<Goblinmon>();
        //Goblinmon buttonGob = this.GetComponent<Goblinmon>();
        playerGob.goblinData = unit;
        playerGob.currentHP = playerGob.goblinData.maxHP; //This will need to be changed
    }

    
}
