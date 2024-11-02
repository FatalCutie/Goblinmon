using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region Variables
    private BattleSystem bs;
    private ButtonManager bm;
    [SerializeField] private Goblinmon self;
    [SerializeField] private Goblinmon player;
    private enum EnemyType { TRAINER, WILD }
    [SerializeField] private EnemyType enemyType;
    [SerializeField] SOMove emptyMove;
    [SerializeField] private List<SOGoblinmon> units;
    System.Random rnd = new System.Random();

    #endregion

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
        bm = FindObjectOfType<ButtonManager>();

    }

    public void InitilizeUnitsForEnemyAI(Goblinmon sf, Goblinmon pr)
    {
        self = sf;
        player = pr;

        //Check if trainer by seeing if there's any Goblinmon in party
        if (units.Count > 0) enemyType = EnemyType.TRAINER;
        else enemyType = EnemyType.WILD;
    }

    public void UpdatePlayerUnit(Goblinmon pu)
    {
        player = pu;
    }

    //Find the best action and take it
    public void FindOptimalOption()
    {
        //First check if health is low, if so find something to switch to
        if (enemyType == EnemyType.TRAINER && self.goblinData.currentHP <= self.goblinData.maxHP * .15)
        {
            Debug.Log("Things look hairy, I'm gonna try and switch");
            SOGoblinmon safeSwitch = FindSafeSwitch();
            if (safeSwitch != null)
            {
                Debug.Log($"{safeSwitch.gName} looks like a safe option, I'm gonna switch!");
                StartCoroutine(Switch(safeSwitch));
            }
            else Debug.Log("Nevermind, I'm not gonna switch");
            return;
        }

        //Second see if there is a move that kills player
        SOMove lethalMove = IsEnemyKillable();
        if (lethalMove != null)
        {
            Debug.Log($"Found a lethal move {lethalMove.moveName}, using it");
            StartCoroutine(AttackPlayer(lethalMove));
            return;
        }



        //TODO: Decide between using an attacking or status move then attacking
        //TODO: Add chance to use random move instead of best move if attacking (80/20), scales with health?
        SOMove bestAttackingMove = FindAttackingMove();
        Debug.Log($"Using best move {bestAttackingMove.moveName}, using it");
        StartCoroutine(AttackPlayer(bestAttackingMove));
    }

    #region Switching

    //Finds a safe unit to switch into
    private SOGoblinmon FindSafeSwitch()
    {
        SOType playerType = player.goblinData.type;
        SOType selfType = self.goblinData.type;

        //Find something strong against the enemy type
        foreach (SOGoblinmon unit in units)
        {
            if (playerType.weakAgainstEnemyType(selfType)
            && unit != self) return unit;
        }

        //If nothing then look for something neutral
        foreach (SOGoblinmon unit in units)
        {
            if (selfType.weakAgainstEnemyType(playerType)
            && unit != self)
            {
                //Continue
            }
            else return unit;
        }

        //If nothing neutral pick another option
        return null;
    }

    //Switch into the safe unit
    private IEnumerator Switch(SOGoblinmon unit)
    {
        //Makes switching look smooth for player
        bs.dialogueText.text = "Come back " + bs.enemyUnit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);
        bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(2);
        bs.dialogueText.text = "Go, " + self.goblinData.gName + "!";
        yield return new WaitForSeconds(1);

        //Initilize new unit
        Goblinmon gob = this.gameObject.AddComponent<Goblinmon>();
        gob.goblinData = unit;

        //Switches the active unit
        self = gob;
        bs.enemyHUD.SetHUD(gob);
        bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = self.goblinData.sprite;
        yield return new WaitForSeconds(1);

        //End the enemys turn
        bs.state = BattleState.PLAYERTURN;
        bm.enableBasicButtonsOnPress();
        bs.PlayerTurn();
    }

    #endregion

    #region Attack Player

    //Checks if there is a move that kills the player and returns it
    private SOMove IsEnemyKillable()
    {
        int playerHP = player.goblinData.currentHP;
        SOType playerType = player.goblinData.type;
        foreach (SOMove move in self.goblinData.moveset)
        {
            //If attack super effective & kills return move
            if (playerType.weakAgainstEnemyType(move.moveType) && move.moveAction == SOMove.MoveAction.ATTACK)
            {
                //Factor in damage modifiers with calculation
                int moveDamage = self.ApplyDamageModifiers(move.damage * 2, self);
                if (moveDamage > playerHP) return move;
            }
            else if (move.moveAction == SOMove.MoveAction.ATTACK)
            //If move kills return move
            {
                //Factor in damage modifiers with calculation
                int moveDamage = self.ApplyDamageModifiers(move.damage, self);
                if (moveDamage > playerHP) return move;
            }
        }
        return null;
    }

    //Find the highest damaging move to attack with
    //TODO: Add randomized damage modifier range (.85 - 1) for more random choices
    private SOMove FindAttackingMove()
    {
        SOType playerType = player.goblinData.type;
        SOMove returnMove = emptyMove;

        //Loop through all attacks to find highest damage
        foreach (SOMove move in self.goblinData.moveset)
        {
            //If attack super effective & kills return move
            if (playerType.weakAgainstEnemyType(move.moveType) && move.moveAction == SOMove.MoveAction.ATTACK)
            {
                //Factor in damage modifiers with calculation
                int moveDamage = self.ApplyDamageModifiers(move.damage * 2, self);
                if (moveDamage * 2 > returnMove.damage) returnMove = move;
            }
            else
            {
                //Factor in damage modifiers with calculation
                int moveDamage = self.ApplyDamageModifiers(move.damage, self);
                if (moveDamage > returnMove.damage) returnMove = move;
            }
        }
        return returnMove;
    }

    //Loop through each move to check if any of them buff/debuff
    private bool DoesUnitHaveBuffingMove()
    {
        foreach (SOMove move in self.goblinData.moveset)
        {
            if (move.moveAction == SOMove.MoveAction.BUFF || move.moveAction == SOMove.MoveAction.DEBUFF) return true;
        }
        return false;
    }

    //Returns a buff/debuff move
    private SOMove FindBuffingMove()
    {
        List<SOMove> movePool = new List<SOMove>();
        foreach (SOMove move in self.goblinData.moveset)
        {
            if (move.moveAction == SOMove.MoveAction.BUFF || move.moveAction == SOMove.MoveAction.DEBUFF) movePool.Add(move);
        }
        //Skip number generation if there's only one option
        if (movePool.Count == 1) return movePool[0];
        //Generate a random number which decides which move from movepool to return
        int selection = rnd.Next(0, movePool.Count);


        return movePool[selection];
    }

    //Use an attacking move against the player
    private IEnumerator AttackPlayer(SOMove move)
    {
        bool strongAttack = player.goblinData.type.weakAgainstEnemyType(move.moveType);
        bs.dialogueText.text = $"{self.goblinData.name} used {move.name}!";
        yield return new WaitForSeconds(2);

        //Different text/sounds based on attack effectiveness
        if (strongAttack)
        {
            bs.dialogueText.text = "It was super effective!";
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("superEffective");
        }
        else
        {
            bs.dialogueText.text = "The attack was successful!";
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("damage");
        }

        //Attack player
        bool isDead = player.TakeDamage(move.damage, strongAttack, self);
        bs.playerHUD.setHP(player.goblinData.currentHP);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {   //End battle
            bs.state = BattleState.LOST;
            bs.EndBattle();
        }
        else
        {   //End enemy's turn
            bs.state = BattleState.PLAYERTURN;
            bm.enableBasicButtonsOnPress();
            bs.PlayerTurn();
        }

    }
    #endregion


}
