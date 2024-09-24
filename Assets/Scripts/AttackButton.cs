using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public SOMove move;
    [SerializeField] BattleSystem bs;
    [SerializeField] ButtonManager bm;
    [SerializeField] private TextMeshProUGUI descriptionText;


    public void UseMoveOnButtonPress()
    {
        switch (move.moveAction)
        {
            case SOMove.MoveAction.ATTACK:
                {
                    //TODO: Hide buttons when attakc is chosen
                    StartCoroutine(PlayerAttack(move));
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

    IEnumerator PlayerAttack(SOMove move)
    {

        int moveDamage = move.damage;
        bool strongAttack = bs.enemyUnit.goblinData.type.weakAgainstEnemyType(move.moveType);
        if (strongAttack)
        {
            descriptionText.text = "The attack is super effective!";
            //TODO play super effective sound
            yield return new WaitForSeconds(1f);
            bool isDead = bs.GetComponent<BattleSystem>().enemyUnit.TakeDamage(move.damage, strongAttack);
            bs.enemyHUD.setHP(bs.enemyUnit.goblinData.currentHP);
            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                bs.state = BattleState.WON;
                bs.EndBattle();
            }
            else
            {
                bs.state = BattleState.ENEMYTURN;
                StartCoroutine(bs.EnemyTurn());
            }
        }
        else
        {
            descriptionText.text = "The attack is successful!";
            //TODO play super effective sound
            yield return new WaitForSeconds(1f);
            bool isDead = bs.enemyUnit.TakeDamage(move.damage, strongAttack);
            bs.enemyHUD.setHP(bs.enemyUnit.goblinData.currentHP);
            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                bs.state = BattleState.WON;
            }
            else
            {
                bs.state = BattleState.ENEMYTURN;
                StartCoroutine(bs.EnemyTurn());
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

