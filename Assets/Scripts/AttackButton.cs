using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public SOMove move;
    [SerializeField] BattleSystem bs;
    [SerializeField] ButtonManager bm;
    [SerializeField] private TextMeshProUGUI descriptionText;

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (move == null) this.gameObject.SetActive(false);
    }


    public void UseMoveOnButtonPress()
    {
        switch (move.moveAction)
        {
            case SOMove.MoveAction.ATTACK:
                {
                    FindObjectOfType<AudioManager>().Play("press");
                    bs.StartPlayerAttack(move);
                    break;
                }
            case SOMove.MoveAction.BUFF:
                {
                    //TODO: Buff player based on selected buff type
                    FindObjectOfType<AudioManager>().Play("press");
                    break;
                }
            case SOMove.MoveAction.DEBUFF:
                {
                    //TODO: Debuff enemy based on selected debuff type
                    FindObjectOfType<AudioManager>().Play("press");
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

    public void SetMove(SOMove newMove){
        this.move = newMove;
    }

    public void OnMouseHoverLeave()
    {
        descriptionText.text = "Choose an action:"; //This is hardcoded LOL!
        //Don't forget it!
    }
}

