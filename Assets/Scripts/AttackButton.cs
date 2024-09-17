using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public SOMove move;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] private TextMeshProUGUI descriptionText;



    public void UseMoveOnButtonPress()
    {
        switch (move.moveAction)
        {
            case SOMove.MoveAction.ATTACK:
                {
                    //Attack the enemy based on move damage and typing
                    battleSystem.PlayerAttack();
                    break;
                }
            case SOMove.MoveAction.BUFF:
                {
                    //Buff player based on selected buff type
                    break;
                }
            case SOMove.MoveAction.DEBUFF:
                {
                    //Debuff enemy based on selected debuff type
                    break;
                }
        }
    }

    public void OnMouseHover()
    {
        try
        {
            descriptionText.text = move.moveDescription;
        }
        catch (NullReferenceException)
        {
            descriptionText.text = "Somthing went wrong and this button does not have an assigned move";
        }

    }

    public void OnMouseHoverLeave()
    {
        descriptionText.text = "Choose an action:"; //This is hardcoded LOL!
        //Don't forget it!
    }
}

