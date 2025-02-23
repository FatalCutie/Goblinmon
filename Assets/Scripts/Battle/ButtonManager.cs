using System;

using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonsBasic;
    public GameObject buttonsAttack;
    public GameObject switchingMenu;
    [SerializeField] private BattleSystem bs;
    [SerializeField] private List<AttackButton> attackButtons;


    void Start()
    {
        if (buttonsAttack.activeSelf) buttonsAttack.SetActive(false);
        if (!buttonsBasic.activeSelf) buttonsBasic.SetActive(true);
        switchingMenu.SetActive(false);
    }

    //If disabling the game object raises issues in the future
    //you can try disabling the renderer instead?
    #region Button Press Commands
    public void enableAttackButtonsOnPress()
    {
        FindObjectOfType<AudioManager>().Play("press");
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

    public void enableSwitchingMenu()
    {
        FindObjectOfType<AudioManager>().Play("press");
        disableButtonsDuringAttack();
        switchingMenu.SetActive(true);
        //TODO: Update goblinmon on open?
    }

    public void disableSwitchingMenu()
    {
        FindObjectOfType<AudioManager>().Play("press");
        switchingMenu.SetActive(false);
        buttonsBasic.SetActive(true);
    }

    public void HideSwitchingMenuAfterCapture()
    {
        FindObjectOfType<AudioManager>().Play("press");
        switchingMenu.SetActive(false);
    }

    public void Catch()
    {
        FindObjectOfType<AudioManager>().Play("press");
        disableButtonsDuringAttack();
        StartCoroutine(FindObjectOfType<CatchSystem>().AttemptToCatch(bs.enemyUnit));
    }

    public void TestingRun()
    {
        FindObjectOfType<AudioManager>().Play("press");
        FindObjectOfType<AudioManager>().Stop("battle");
        FindObjectOfType<SceneController>().TransitionScene("OverworldScene");
    }

    public void disableSwitchingMenuAfterSwitch(){
        switchingMenu.SetActive(false);
    }

    public void unimplementedButtonError()
    {
        Debug.LogWarning("Pressed button is not yet Implemented! If this error is unexpected please check your code!");
    }
    #endregion

    public void SetPlayerMoves(Goblinmon unit)
    //This can be improved but I'm so tired of working on it
    { 
        int i = 0;
        try
        {   //Sets up attack buttons on HUD
            foreach (Transform go in buttonsAttack.transform)
            {
                TextMeshProUGUI moveNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                AttackButton ab = go.GetComponent<AttackButton>();
                ab.move = unit.goblinData.moveset[i];
                moveNameText.text = ab.move.name;
                ab.gameObject.SetActive(true);
                i++;
            }
        }catch (ArgumentOutOfRangeException) {} //The Goblinmon has run out of moves

        //Hides any unused move slots
        for (int j = i; j < 4; j++)
        {
            attackButtons[j].gameObject.SetActive(false);
        }
    }
    
}
