using System;
using TMPro;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonsBasic;
    public GameObject buttonsAttack;
    public GameObject switchingMenu;
    public BattleSystem bs;
    private bool firstTime = true;

    void Start()
    {
        if (buttonsAttack.activeSelf) buttonsAttack.SetActive(false);
        if (!buttonsBasic.activeSelf) buttonsBasic.SetActive(true);
        // switchingMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    //TODO: CHANGE ALL ENABLING/DISABLE TO DISABLE RENDERER
    //INSTEAD OF GAMEOBJECT
    public void enableAttackButtonsOnPress()
    {
        buttonsAttack.SetActive(true);
        SetPlayerMoves(bs.playerUnit);
        buttonsBasic.SetActive(false);
    }

    public void disableButtonsDuringAttack()
    {
        buttonsBasic.SetActive(false);
        buttonsAttack.SetActive(false);
    }

    public void enableBasicButtonsOnPress()
    {
        FindObjectOfType<AudioManager>().Play("press");
        buttonsAttack.SetActive(false);
        buttonsBasic.SetActive(true);
    }

    public void openSwitchingMenu()
    {
        FindObjectOfType<AudioManager>().Play("press");
        disableButtonsDuringAttack();
        switchingMenu.SetActive(true);
        switchingMenu.GetComponent<SwitchingManager>().PopulateUnits();
    }

    public void closeSwitchingMenu()
    {
        switchingMenu.SetActive(false);
        buttonsBasic.SetActive(true);
    }

    public void unimplementedButtonError()
    {
        Debug.LogWarning("Pressed button is not yet Implemented! If this error is unexpected please check your code!");
    }

    public void SetPlayerMoves(Goblinmon unit)
    //Run when goblinmon is switched, sets attack buttons on HUD
    {
        //Scrubs attack buttons incase # of moves differs after switch
        if (firstTime)
        {
            firstTime = false;
        }
        else ClearPlayerMoves();


        int i = 0;
        try
        {
            foreach (Transform go in buttonsAttack.transform)
            {
                TextMeshProUGUI moveNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                AttackButton ab = go.GetComponent<AttackButton>();
                ab.move = unit.goblinData.moveset[i];
                moveNameText.text = ab.move.name;
                i++;
            }
        }
        //Not the optimal solution but a functional one
        //My favorite kind!
        catch (ArgumentOutOfRangeException) { }
    }

    public void ClearPlayerMoves()
    {
        try
        {
            foreach (Transform go in buttonsAttack.transform) //this is also pretty dumb
            {
                TextMeshProUGUI moveNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                AttackButton ab = go.GetComponent<AttackButton>();
                ab.move = null; //This errors out
                moveNameText.text = "";
            }
        }
        catch (ArgumentOutOfRangeException) { }
    }
}
