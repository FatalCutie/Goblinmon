using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonsBasic;
    public GameObject buttonsAttack;

    void Start()
    {
        if (buttonsAttack.activeSelf) buttonsAttack.SetActive(false);
        if (!buttonsBasic.activeSelf) buttonsBasic.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void enableAttackButtonsOnPress()
    {
        buttonsBasic.SetActive(false);
        buttonsAttack.SetActive(true);
    }

    public void enableBasicButtonsOnPress()
    {
        buttonsAttack.SetActive(false);
        buttonsBasic.SetActive(true);
    }

    public void unimplementedButtonError()
    {
        Debug.LogWarning("Pressed button is not yet Implemented! If this error is unexpected please check your code!");
    }

    public void SetPlayerMoves(Goblinmon unit)
    //Run when goblinmon is switched, sets attack buttons on HUD
    {
        int i = 0;
        try
        {


            foreach (Transform go in buttonsAttack.transform) //this is also pretty dumb
            {
                TextMeshProUGUI moveNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                moveNameText.text = unit.moveset[i].moveName;
                i++;
            }
        }
        //Not the optimal solution but a functional one
        //My favorite kind!
        catch (ArgumentOutOfRangeException) { }

    }
}