using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region Variables
    private BattleSystem bs;
    private ButtonManager bm;
    private SwitchingManager sm;
    [SerializeField] private Goblinmon self;
    [SerializeField] private Goblinmon internalPlayer; //Player used in calculations, updates at end of enemy turn
    private enum EnemyType { TRAINER, WILD }
    private EnemyType enemyType;
    [SerializeField] SOMove emptyMove;
    [SerializeField] private List<SOGoblinmon> units;
    [SerializeField] private GameObject unitHolder; //This is so fucking awful
    [SerializeField] private List<Goblinmon> party;
    private Goblinmon actualPlayer;
    System.Random rnd = new System.Random();

    #endregion

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
        bm = FindObjectOfType<ButtonManager>();
        sm = FindObjectOfType<SwitchingManager>();
    }

    //Initilizes EnemyAI, returns Goblinmon for BattleSystem to set hud
    public Goblinmon InitilizeUnitsForEnemyAI(Goblinmon eu, Goblinmon pr)
    {
        //Self initilized as first Goblinmon in array
        internalPlayer = this.AddComponent<Goblinmon>();
        self = eu;
        self.goblinData = units[0];
        self.currentHP = self.goblinData.maxHP;

        //Set up internal and actual player for damage calculations/ai decisionmaking
        actualPlayer = pr;

        internalPlayer.currentHP = pr.currentHP;
        internalPlayer.goblinData = pr.goblinData;

        //Check if trainer by seeing if there's any Goblinmon in party
        if (units.Count > 1) enemyType = EnemyType.TRAINER;
        else enemyType = EnemyType.WILD;

        //Initilizes a list of Goblinmon for enemy to save their health totals
        foreach (SOGoblinmon unit in units)
        {
            Goblinmon newUnit = unitHolder.AddComponent<Goblinmon>();
            newUnit.goblinData = unit;
            newUnit.currentHP = unit.maxHP;
            party.Add(newUnit);
        }

        return self;
    }

    //Updates player in calculations to actual player
    //Prevents ai from "predicting" a switch in
    public void UpdateInternalPlayerUnit()
    {
        internalPlayer.goblinData = actualPlayer.goblinData;
        internalPlayer.currentHP = actualPlayer.currentHP;
        internalPlayer.attackModifier = actualPlayer.attackModifier;
        internalPlayer.defenseModifier = actualPlayer.attackModifier;
    }

    //Find the best action and take it
    //TODO: Introduce point system to weigh tactics? (Buff/Debuff if high health? Switch if in bad situation?)
    public void FindOptimalOption()
    {
        //First check if health is low, if so find something to switch to
        float healthRangeModifier = rnd.Next(1, 21); //Will switch randomly between 10% health and 30% health
        if (enemyType == EnemyType.TRAINER && self.currentHP <= self.goblinData.maxHP * (.09 + healthRangeModifier * 0.01))
        {
            Debug.Log("Things look hairy, I'm gonna try and switch");
            Goblinmon safeSwitch = FindSafeSwitch(false);
            if (safeSwitch != null)
            {
                Debug.Log($"{safeSwitch.goblinData.gName} looks like a safe option, I'm gonna switch!");
                StartCoroutine(SwitchAction(safeSwitch, false));
                return;
            }
            else Debug.Log("Nevermind, I'm not gonna switch");
        }

        //Second see if there is a move that kills player
        SOMove lethalMove = IsEnemyKillable();
        if (lethalMove != null)
        {
            Debug.Log($"Found a lethal move {lethalMove.moveName}, using it");
            StartCoroutine(AttackPlayerAction(lethalMove));
            return;
        }

        //If neither generate a random number and decide action based on that
        int decider = rnd.Next(1, 11); //Create number between 1 and 10
        if (decider <= 5 || decider > 8) //70% chance to attack
        {
            SOMove bestAttackingMove = FindAttackingMove();
            Debug.Log($"Using best move {bestAttackingMove.moveName}");
            StartCoroutine(AttackPlayerAction(bestAttackingMove));
        }
        else //if rnd rolls between a 6 and 8
        {
            if (DoesUnitHaveBuffingMove())
            {
                SOMove chosenBuffingMove = FindBuffingMove();
                if (chosenBuffingMove.moveAction == SOMove.MoveAction.BUFF)
                {
                    Debug.Log("I'm gonna buff myself!");
                    StartCoroutine(BuffEnemyAction(chosenBuffingMove));
                }
                else if (chosenBuffingMove.moveAction == SOMove.MoveAction.DEBUFF)
                {
                    Debug.Log("I'm gonna debuff the player!");
                    StartCoroutine(DebuffPlayerAction(chosenBuffingMove));
                }
                else
                {
                    Debug.LogWarning($"{chosenBuffingMove.moveName} does not have a proper move action... How did this even happen?");
                }
            }
            else //If no buffing move just attack
            {
                SOMove bestAttackingMove = FindAttackingMove();
                Debug.Log($"I don't have any buffing moves so I am going to attack using {bestAttackingMove.moveName} instead!");
                StartCoroutine(AttackPlayerAction(bestAttackingMove));
            }
        }
        //TODO: Decide between using an attacking or status move then attacking
        //TODO: Add chance to use random move instead of best move if attacking (80/20), scales with health?

    }

    #region Switching

    //Finds a safe unit (relative to internal player) to switch into
    public Goblinmon FindSafeSwitch(bool needUnitForSwitch)
    {
        SOType playerType = internalPlayer.goblinData.type;


        //Find something strong against the enemy type
        foreach (Goblinmon unit in party)
        {
            if (playerType.weakAgainstEnemyType(unit.goblinData.type)
            && unit != self
            && unit.currentHP > 0)
                return unit;
        }

        //If nothing then look for something neutral
        foreach (Goblinmon unit in party)
        {
            SOType selfType = unit.goblinData.type;
            if (selfType.weakAgainstEnemyType(playerType)
            && unit != self
            && unit.currentHP > 0)
            {
                //Continue
            }
            else
            {
                if (unit.currentHP > 0) return unit;
            }
        }

        if (needUnitForSwitch) //Only true if previous unit was knocked out
        {
            foreach (Goblinmon unit in party)
            {
                if (unit.currentHP > 0) return unit;
            }
        }

        return null; //If nothing neutral pick another option

    }

    //Returns true if there are alive units, returns false otherwise
    public bool CheckForMoreUnits()
    {
        if (enemyType == EnemyType.WILD) return false; //fast track process if nothing in party

        foreach (Goblinmon unit in party)
        {
            if (unit.currentHP > 0 && unit.goblinData != null)
            {
                Debug.Log(unit.goblinData.gName);
                return true;
            }
        }
        return false;
    }

    //Finds active units place in party and saves their current HP
    public void SaveUnitData()
    {
        Goblinmon unitToSave = self;
        int unitID = 0;
        foreach (Goblinmon un in party)
        {
            if (un.goblinData == unitToSave.goblinData)
            {
                break;
            }
            unitID++;
        }
        party[unitID].currentHP = unitToSave.currentHP;
    }

    //Switch into the given safe unit
    public IEnumerator SwitchAction(Goblinmon unit, bool justDied)
    {
        //Makes switching look smooth for player
        if (justDied)
        {
            StartCoroutine(bs.ScrollText($"Go, {unit.goblinData.gName}!"));
            yield return new WaitForSeconds(2);
        }
        else
        {
            StartCoroutine(bs.ScrollText($"Come back{bs.enemyUnit.goblinData.gName}!"));
            yield return new WaitForSeconds(1);
            bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
            yield return new WaitForSeconds(2);
            StartCoroutine(bs.ScrollText($"Go, {unit.goblinData.gName}!"));
            yield return new WaitForSeconds(1);
        }


        //Saves data before switching
        SaveUnitData();

        //Switches the active unit
        Goblinmon gob = bs.enemyUnit;
        gob.goblinData = unit.goblinData;
        gob.currentHP = unit.currentHP;
        //Clear stat changes
        gob.defenseModifier = 0;
        gob.attackModifier = 0;

        //Update HUD
        bs.enemyHUD.SetHUD(gob);
        bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = self.goblinData.sprite;
        yield return new WaitForSeconds(1);

        //End the enemys turn
        bs.state = BattleState.PLAYERTURN;
        if (internalPlayer != actualPlayer) UpdateInternalPlayerUnit();
        bm.enableBasicButtonsOnPress();
        bs.PlayerTurn();
    }


    #endregion

    #region Attacking

    //Checks if there is a move that kills the internal player and returns it
    private SOMove IsEnemyKillable()
    {
        int playerHP = internalPlayer.currentHP;
        SOType playerType = internalPlayer.goblinData.type;
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
        SOType playerType = internalPlayer.goblinData.type;
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
        int selection = rnd.Next(0, movePool.Count - 1);

        return movePool[selection];
    }

    //Use an attacking move against the actual player
    private IEnumerator AttackPlayerAction(SOMove move)
    {

        //Update player unit for accurate damage calcutations/changes
        if (internalPlayer != actualPlayer) UpdateInternalPlayerUnit();

        //TODO: Attack based on BattleSystem instead of internal tracking of player?
        bool strongAttack = actualPlayer.goblinData.type.weakAgainstEnemyType(move.moveType); ;

        StartCoroutine(bs.ScrollText($"{self.goblinData.name} used {move.name}!"));
        yield return new WaitForSeconds(2);

        //Different text/sounds based on attack effectiveness
        if (strongAttack)
        {
            StartCoroutine(bs.ScrollText("It was super effective!"));
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("superEffective");
        }
        else
        {
            StartCoroutine(bs.ScrollText("The attack was successful!"));
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("damage");
        }

        //Attack player
        bool isDead = actualPlayer.TakeDamage(move.damage, strongAttack, self);
        bs.playerHUD.setHP(actualPlayer.currentHP, actualPlayer);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {   //Check if player has available units
            bs.playerUnit.GetComponent<SpriteRenderer>().sprite = null;
            StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName} Fainted!"));
            yield return new WaitForSeconds(2f);
            sm.SavePlayerData();
            if (sm.DoesPlayerHaveUnits())
            {
                sm.GetNewPlayerUnit();
                //Continue Battle
            }
            else
            {
                bs.state = BattleState.LOST;
                bs.EndBattle();
            }
        }
        else
        {   //End enemy's turn
            bs.state = BattleState.PLAYERTURN;
            UpdateInternalPlayerUnit();
            bm.enableBasicButtonsOnPress();
            bs.PlayerTurn();
        }

    }

    //Buffs enemy
    public IEnumerator BuffEnemyAction(SOMove move)
    {
        StartCoroutine(bs.ScrollText($"{self.goblinData.gName} used {move.moveName}!"));
        yield return new WaitForSeconds(2f);

        //Buff Player
        switch (move.statModified)
        {
            case SOMove.StatModified.ATTACK:
                {
                    self.attackModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (self.attackModifier > 6)
                    {
                        //clamp buff at 6
                        self.attackModifier = 6;
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s attack can't go any higher!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s attack was increased!"));
                        yield return new WaitForSeconds(2f);
                    }
                    break;
                }

            case SOMove.StatModified.DEFENSE:
                {
                    self.defenseModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (self.defenseModifier > 6)
                    {
                        //clamp buff at 6
                        self.defenseModifier = 6;
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s defense can't go any higher!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s defense was increased!"));
                        yield return new WaitForSeconds(2f);
                    }
                    break;
                }
            case SOMove.StatModified.NONE:
                {
                    Debug.LogWarning("WARNING:" + move.moveName + " does not have an assigned stat to modify. Check SO!");
                    break;
                }
        }

        //End Turn
        bs.state = BattleState.PLAYERTURN;
        bm.enableBasicButtonsOnPress();
        bs.PlayerTurn();
    }

    //Debuffs Player
    public IEnumerator DebuffPlayerAction(SOMove move)
    {
        StartCoroutine(bs.ScrollText($"{self.goblinData.gName} used {move.moveName}!"));
        yield return new WaitForSeconds(2f);

        //Debuff Enemy
        switch (move.statModified)
        {
            case SOMove.StatModified.ATTACK:
                {
                    actualPlayer.attackModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (actualPlayer.attackModifier < -6)
                    {
                        //clamp debuff at 6
                        actualPlayer.attackModifier = -6;
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s attack can't go any lower!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s attack was lowered!"));
                        yield return new WaitForSeconds(2f);
                    }
                    break;
                }

            case SOMove.StatModified.DEFENSE:
                {
                    bs.enemyUnit.defenseModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (actualPlayer.defenseModifier < -6)
                    {
                        actualPlayer.defenseModifier = -6; //clamp
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s defense can't go any lower!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s defense was lowered!"));
                        yield return new WaitForSeconds(2f);
                    }
                    break;
                }

            case SOMove.StatModified.NONE:
                {
                    Debug.LogWarning($"WARNING: {move.moveName} does not have an assigned stat to modify. Check SO!");
                    break;
                }
        }

        //End Turn
        bs.state = BattleState.PLAYERTURN;
        bm.enableBasicButtonsOnPress();
        bs.PlayerTurn();
    }
    #endregion

}
