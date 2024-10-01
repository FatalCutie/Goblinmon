using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    public SOGoblinmon unit;
    public BattleHUD playerHUD;
    private TextMeshProUGUI text;
    public BattleSystem bs;
    private bool lagSwitch;

    void Start()
    {
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

    public void SwitchUnit()
    {
        Goblinmon gob = this.GetComponent<Goblinmon>();
        UpdatePlayerInformation();
        playerHUD.SetHUD(gob);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = gob.goblinData.sprite;

        //TODO: Update Moves too using existing void in Button Manager
        //TODO: Wrap in Ienumerator to feel more natural
    }

    public void UpdatePlayerInformation()
    {
        Goblinmon playerGob = bs.playerUnit.GetComponent<Goblinmon>();
        //Goblinmon buttonGob = this.GetComponent<Goblinmon>();
        playerGob.goblinData = unit;
        playerGob.currentHP = playerGob.goblinData.maxHP; //This will need to be changed
    }
}
