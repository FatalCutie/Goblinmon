using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private BattleSystem bs;
    private Goblinmon self;
    private Goblinmon player;
    private enum EnemyType { TRAINER, WILD }
    private EnemyType enemyType;

    //TODO: Allow AI to recognize if Trainer or Wild
    private List<Goblinmon> units;

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
    }

    //TODO: Find elegant way to initalize enemy, maybe in BattleSystem?
    //Take List and Goblinmon, initalize from there?
    public void InitilizeUnits(Goblinmon sf, Goblinmon pr)
    {
        sf = self;
        pr = player;
    }

    //Find the best action and take it
    public void FindOptimalOption()
    {
        //First check if health is low, if so find something to switch to
        if (enemyType == EnemyType.TRAINER && self.currentHP <= self.currentHP * .15)
        {
            Goblinmon safeSwitch = FindSafeSwitch();
            if (safeSwitch != null) return;
        }

        //Second see if there is a move that kills player
        SOMove lethalMove = IsEnemyKillable();
        if (lethalMove != null) return;

    }

    //Finds a safe unit to switch into
    private Goblinmon FindSafeSwitch()
    {
        SOType playerType = player.goblinData.type;
        SOType selfType = self.goblinData.type;

        //Find something strong against the enemy type
        foreach (Goblinmon unit in units)
        {
            if (playerType.weakAgainstEnemyType(selfType)) return unit;
        }

        //If nothing then look for something neutral
        foreach (Goblinmon unit in units)
        {
            if (selfType.weakAgainstEnemyType(playerType))
            {
                //Continue
            }
            else return unit;
        }

        //If nothing neutral take another option
        return null;
    }

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
}
