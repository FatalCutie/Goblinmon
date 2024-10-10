using TMPro;
using System.Collections;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    public SOGoblinmon unit;
    public BattleHUD playerHUD;
    private TextMeshProUGUI text;
    public BattleSystem bs;
    public ButtonManager bm;
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

    public void SwitchUnitOnPress()
    {
        StartCoroutine(SwitchUnit());
    }

    public IEnumerator SwitchUnit()
    {
        Goblinmon gob = this.GetComponent<Goblinmon>();

        //Makes switching look smooth for player
        bs.dialogueText.text = "Come back " + bs.playerUnit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(2);
        bs.dialogueText.text = "Go, " + gob.goblinData.gName + "!";
        yield return new WaitForSeconds(1);

        //Actually switching unit
        UpdatePlayerInformation();
        playerHUD.SetHUD(gob);
        bs.playerUnit.GetComponent<SpriteRenderer>().sprite = gob.goblinData.sprite;
        yield return new WaitForSeconds(1);

        //bm.SetPlayerMoves(gob); //Attack buttons are inactive so this throws an error
        //Change moves to update every open?

        //This uses the players turn
        bs.state = BattleState.ENEMYTURN;
        Debug.Log("enemy turn");
        StartCoroutine(bs.EnemyTurn());
    }

    public void UpdatePlayerInformation()
    {
        Goblinmon playerGob = bs.playerUnit.GetComponent<Goblinmon>();
        //Goblinmon buttonGob = this.GetComponent<Goblinmon>();
        playerGob.goblinData = unit;
        playerGob.currentHP = playerGob.goblinData.maxHP; //This will need to be changed
    }
}
