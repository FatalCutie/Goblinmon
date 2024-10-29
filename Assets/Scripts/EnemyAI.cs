using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region Variables
    private BattleSystem bs;
    private ButtonManager bm;
    private Goblinmon self;
    private Goblinmon player;
    private enum EnemyType { TRAINER, WILD }
    private EnemyType enemyType;
    [SerializeField] private List<Goblinmon> units;

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
        if (enemyType == EnemyType.TRAINER && self.currentHP <= self.currentHP * .15)
        {
            Goblinmon safeSwitch = FindSafeSwitch();
            if (safeSwitch != null) StartCoroutine(Switch(safeSwitch));
        }

        //Second see if there is a move that kills player
        SOMove lethalMove = IsEnemyKillable();
        if (lethalMove != null) StartCoroutine(AttackPlayer(lethalMove));

        //TODO: Find a super effective move to attack player

        //TODO: Decide between using an attacking or status move then attacking

    }

    #region Switching

    //Finds a safe unit to switch into
    private Goblinmon FindSafeSwitch()
    {
        SOType playerType = player.goblinData.type;
        SOType selfType = self.goblinData.type;

        //Find something strong against the enemy type
        foreach (Goblinmon unit in units)
        {
            if (playerType.weakAgainstEnemyType(selfType)
            && unit != self) return unit;
        }

        //If nothing then look for something neutral
        foreach (Goblinmon unit in units)
        {
            if (selfType.weakAgainstEnemyType(playerType)
            && unit != self)
            {
                //Continue
            }
            else return unit;
        }

        //If nothing neutral take another option
        return null;
    }

    //Switch into the safe unit
    private IEnumerator Switch(Goblinmon unit)
    {
        //Makes switching look smooth for player
        bs.dialogueText.text = "Come back " + bs.enemyUnit.goblinData.gName + "!";
        yield return new WaitForSeconds(1);
        bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
        yield return new WaitForSeconds(2);
        bs.dialogueText.text = "Go, " + self.goblinData.gName + "!";
        yield return new WaitForSeconds(1);

        //Switches the active unit
        self = unit;
        bs.enemyHUD.SetHUD(unit);
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
        int playerHP = player.currentHP;
        SOType playerType = player.goblinData.type;
        foreach (SOMove move in self.goblinData.moveset)
        {
            //If attack super effective & kills return move
            if (playerType.weakAgainstEnemyType(move.moveType) && move.moveAction == SOMove.MoveAction.ATTACK)
            {
                if (move.damage * 2 > playerHP) return move;
            }
            else
            //If move kills return move
            {
                if (move.damage > playerHP) return move;
            }
        }
        return null;
    }

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
        bool isDead = player.TakeDamage(move.damage, strongAttack);
        bs.playerHUD.setHP(player.currentHP);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {   //End turn 
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
