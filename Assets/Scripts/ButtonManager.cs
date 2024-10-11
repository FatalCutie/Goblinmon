using System;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonsBasic;
    public GameObject buttonsAttack;
    public GameObject switchingMenu;
    public SOMove emptyMove;
    [SerializeField] private BattleSystem bs;
    [SerializeField] private AttackButtonManager abm;
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
        for(int j = i; j < 3; j++)
        {
            abm.attackButtons[j].gameObject.SetActive(false);
        }
    }
}
