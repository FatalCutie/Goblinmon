using System;
using TMPro;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public SOMove move;
    private BattleSystem bs;
    [SerializeField] private TextMeshProUGUI descriptionText;

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
    }

    public void SetMove(SOMove newMove) //I think this is redundent
    {
        this.move = newMove;
    }

    void FixedUpdate()
    {
        //this is awful, but it's easy and it works
        if (move == null) this.gameObject.SetActive(false);
    }


    public void UseMoveOnButtonPress()
    {
        FindObjectOfType<AudioManager>().Play("press");
        bs.StartPlayerAttack(move);
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

