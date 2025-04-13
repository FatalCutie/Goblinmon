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
    public enum EnemyType { TRAINER, WILD }
    public EnemyType enemyType;
    [SerializeField] SOMove emptyMove;
    //[SerializeField] private List<SOGoblinmon> units;
    [SerializeField] private GameObject unitHolder; //This is so fucking awful
    [SerializeField] private List<Goblinmon> party;
    [SerializeField] private Goblinmon actualPlayer;
    System.Random rnd = new System.Random();
    public float standardWaitTime = 1; //Standard Wait Time
    public SOMove twoTurnMove;

    #endregion

    void Awake()
    {
        party = FindObjectOfType<EnemyPartyStorage>().goblinmon;
    }

    void Start()
    {
        bs = FindObjectOfType<BattleSystem>();
        bm = FindObjectOfType<ButtonManager>();
        sm = FindObjectOfType<SwitchingManager>();

    }

    //Initializes EnemyAI, returns Goblinmon for BattleSystem to set hud
    public Goblinmon InitializeUnitsForEnemyAI(Goblinmon eu, Goblinmon pr) //this is spelled wrong
    {
        //Self initialized as first Goblinmon in array
        internalPlayer = this.AddComponent<Goblinmon>();
        self = eu;
        self.goblinData = party[0].goblinData; //Enemy should never have a dead unit so no failsafe needed
        self.currentHP = self.goblinData.maxHP;

        //Set up internal and actual player for damage calculations/ai decisionmaking
        actualPlayer = pr;

        internalPlayer.currentHP = pr.currentHP;
        internalPlayer.goblinData = pr.goblinData;

        //Check if trainer by seeing if there's any Goblinmon in party
        if (party.Count > 1) enemyType = EnemyType.TRAINER;
        else enemyType = EnemyType.WILD;

        return self;
    }

    //Find the best action and take it
    public void FindOptimalOption()
    {
        if (!twoTurnMove)
        {
            //First check if health is low, if so find something to switch to
            float healthRangeModifier = rnd.Next(1, 21); //Will switch randomly between 10% health and 30% health
            if (enemyType == EnemyType.TRAINER && self.currentHP <= self.goblinData.maxHP * (.09 + healthRangeModifier * 0.01))
            {
                //Debug.Log("Things look hairy, I'm gonna try and switch");
                Goblinmon safeSwitch = FindSafeSwitch(false);
                if (safeSwitch != null)
                {
                    //Debug.Log($"{safeSwitch.goblinData.gName} looks like a safe option, I'm gonna switch!");
                    StartCoroutine(SwitchAction(safeSwitch, false));
                    return;
                }
                //else Debug.Log("Nevermind, I'm not gonna switch");
            }

            //Second see if there is a move that kills player
            SOMove lethalMove = IsEnemyKillable();
            if (lethalMove != null)
            {
                //Debug.Log($"Found a lethal move {lethalMove.moveName}, using it");
                StartCoroutine(AttackPlayerAction(lethalMove));
                return;
            }

            //If neither generate a random number and decide action based on that
            int decider = rnd.Next(1, 11); //Create number between 1 and 10
            if (decider <= 5 || decider > 8) //70% chance to attack
            {
                SOMove bestAttackingMove = FindAttackingMove();
                //Debug.Log($"Using best move {bestAttackingMove.moveName}");
                StartCoroutine(AttackPlayerAction(bestAttackingMove));
        }
        else //if rnd rolls between a 6 and 8
        {
            if (DoesUnitHaveBuffingMove())
            {
                SOMove chosenBuffingMove = FindBuffingMove();
                if (chosenBuffingMove.moveAction == SOMove.MoveAction.BUFF)
                {
                    //Debug.Log("I'm gonna buff myself!");
                    StartCoroutine(BuffEnemyAction(chosenBuffingMove));
                }
                else if (chosenBuffingMove.moveAction == SOMove.MoveAction.DEBUFF)
                {
                    //Debug.Log("I'm gonna debuff the player!");
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
                //Debug.Log($"I don't have any buffing moves so I am going to attack using {bestAttackingMove.moveName} instead!");
                StartCoroutine(AttackPlayerAction(bestAttackingMove));
            }
        }
            //TODO: Decide between using an attacking or status move then attacking
            //TODO: Add chance to use random move instead of best move if attacking (80/20), scales with health?

        }
        else
        { //finish two turn move
            StartCoroutine(CompleteTwoTurnMove());
        }

    }

    #region Switching

    //Finds a safe unit (relative to internal player) to switch into
    public Goblinmon FindSafeSwitch(bool needUnitForSwitch)
    {
        // List<SOType> playerTypes = internalPlayer.goblinData.types;
        //Find something strong against the enemy type
        foreach (Goblinmon unit in party)
        {
            if (unit.ID != self.ID
            && unit.currentHP > unit.goblinData.maxHP * 0.4 //only switch in if above 40% health
            ) //TODO: Switch to advantageous type
            {
                return unit;
            }

        }
        //Debug.Log("Nothing strong, Looking for neutral");

        //If nothing then look for something neutral
        foreach (Goblinmon unit in party)
        {
            // SOType selfType = unit.goblinData.type;
            if (unit.ID != self.ID
            && unit.currentHP > unit.goblinData.maxHP * 0.4
            ) //TODO: Switch to neutral type
            {
                return unit;
            }
        }
        //Debug.Log("Nothing neutral, do I need to switch?");

        if (needUnitForSwitch) //Look for the highest HP unit and switch to it
        {
            List<Goblinmon> availableUnits = new List<Goblinmon>();
            foreach (Goblinmon unit in party)
            {
                if (unit.currentHP > 0) availableUnits.Add(unit);
            }
            int highestHP = -1;
            Goblinmon highestHPUnit = null;
            foreach (Goblinmon unit in availableUnits)
            {
                if (unit.currentHP > highestHP) highestHPUnit = unit;
            }
            return highestHPUnit;
        }
        //Debug.Log("I don't need to switch");

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
    //This is currently throwing an error
    public void SaveUnitData()
    {
        Goblinmon unitToSave = self;
        int unitID = 0;
        foreach (Goblinmon un in party)
        {
            if (un.ID == unitToSave.ID) //Does ID get changed somewhere?
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
            yield return new WaitForSeconds(standardWaitTime);
        }
        else
        {
            StartCoroutine(bs.ScrollText($"Come back {bs.enemyUnit.goblinData.gName}!"));
            yield return new WaitForSeconds(standardWaitTime / 2);
            bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
            yield return new WaitForSeconds(standardWaitTime);
            StartCoroutine(bs.ScrollText($"Go, {unit.goblinData.gName}!"));
            yield return new WaitForSeconds(standardWaitTime / 2);
        }


        //Saves data before switching
        SaveUnitData();

        //Switches the active unit
        Goblinmon gob = bs.enemyUnit;
        gob.goblinData = unit.goblinData;
        gob.currentHP = unit.currentHP;
        gob.CloneIdFrom(unit);

        //Clear stat changes
        gob.defenseModifier = 0;
        gob.attackModifier = 0;

        //Update HUD
        bs.enemyHUD.SetHUD(gob);
        bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = self.goblinData.sprite;
        yield return new WaitForSeconds(standardWaitTime * .75f);

        EndTurn();
    }

    #endregion

    #region Attacking

    //Checks if there is a move that kills the internal player and returns it
    private SOMove IsEnemyKillable()
    {
        //Debug.Log($"I'm about to look at my internal player's hp, which is {internalPlayer.currentHP}");
        int playerHP = internalPlayer.currentHP;
        foreach (SOMove move in self.goblinData.moveset)
        {
            if (move.moveAction == SOMove.MoveAction.ATTACK)
            //If move kills return move
            {
                int moveDamage = self.ApplyDamageModifiers(move, self);
                if (move.moveModifier == SOMove.MoveModifier.MULTI_HIT) moveDamage *= rnd.Next(2, 5); //Account for multi hit moves
                if (moveDamage > playerHP) return move;
            }
        }
        return null;
    }

    //Find the highest damaging move to attack with
    //TODO: Add randomized damage modifier range (.85 - 1) for more random choices
    private SOMove FindAttackingMove()
    {
        SOMove returnMove = emptyMove;

        //Loop through all attacks to find highest damage
        foreach (SOMove move in self.goblinData.moveset)
        {
            //Factor in damage modifiers with calculation
            int moveDamage = self.ApplyDamageModifiers(move, self);
            if (move.moveModifier == SOMove.MoveModifier.MULTI_HIT) moveDamage *= rnd.Next(2, 5); //Account for multi hit moves
            if (moveDamage > returnMove.damage) returnMove = move;
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
        //TODO: This is resetting internal player health
        //if (internalPlayer != actualPlayer) UpdateInternalPlayerUnit();

        //TODO: Attack based on BattleSystem instead of internal tracking of player?

        StartCoroutine(bs.ScrollText($"{self.goblinData.gName} used {move.name}!"));
        yield return new WaitForSeconds(standardWaitTime);
        //Attack player
        if (!bs.twoTurnMove)
        {
            if (move.moveModifier == SOMove.MoveModifier.MULTI_HIT)
            {
                StartCoroutine(MultiHitAttack(move));
            }
            else if (move.moveModifier == SOMove.MoveModifier.TWO_TURN)
            {
                //Hide unit
                bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
                StartCoroutine(bs.ScrollText($"{bs.enemyUnit.goblinData.gName} dove under water!"));
                yield return new WaitForSeconds(standardWaitTime);

                //End turn, move will land in PlayerTurn() on the next turn
                twoTurnMove = move; //stash move for FindOptimalOption()
                EndTurn();
            }
            else
            {
                bool strongAttack = bs.playerUnit.GetWeaknessMultiplier(move) > 1;
                if (move.moveModifier == SOMove.MoveModifier.RANDOM_TYPE)
                {
                    int i = rnd.Next(0, bs.types.Capacity);
                    SOType temp = bs.types[i];
                    StartCoroutine(bs.ScrollText($"The move switches type to {temp.name}!"));
                    yield return new WaitForSeconds(standardWaitTime);
                }
                bool isDead = actualPlayer.TakeDamage(move, self);
                bs.playerHUD.setHP(actualPlayer.currentHP, actualPlayer);
                //Different text/sounds based on attack effectiveness
                if (strongAttack)
                {
                    FindObjectOfType<AudioManager>().Play("superEffective");
                    yield return new WaitForSeconds(standardWaitTime / 2);
                    StartCoroutine(bs.ScrollText("It was super effective!"));
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("damage");
                    yield return new WaitForSeconds(standardWaitTime / 2);
                    StartCoroutine(bs.ScrollText("The attack was successful!"));
                }
                if (move.moveModifier == SOMove.MoveModifier.RECOIL)
                {
                    yield return new WaitForSeconds(standardWaitTime);
                    int recoilDamage = Mathf.RoundToInt(move.damage * bs.playerUnit.GetWeaknessMultiplier(move) * (move.statModifier * 0.01f));
                    StartCoroutine(bs.ScrollText($"{bs.enemyUnit.goblinData.gName} was hurt by recoil!"));
                    bs.enemyUnit.currentHP -= recoilDamage;
                    if (bs.enemyUnit.currentHP < 0) bs.enemyUnit.currentHP = 0;
                    bs.enemyHUD.setHP(bs.enemyUnit.currentHP, bs.enemyUnit);
                    if (bs.enemyUnit.currentHP == 0)
                    {
                        StartCoroutine(bs.KillEnemyUnit());
                        yield return new WaitForSeconds(standardWaitTime * 4.5f); //Wait for enemy to faint and pick new unit
                    }
                }
                yield return new WaitForSeconds(standardWaitTime);

                if (isDead)
                {
                    StartCoroutine(bs.RetrieveDeadUnit());
                }
                else
                {
                    EndTurn();
                }
            }

        }
        else
        {
            StartCoroutine(bs.ScrollText("The attack missed!"));
            yield return new WaitForSeconds(standardWaitTime);
            EndTurn();
        }

    }

    public IEnumerator MultiHitAttack(SOMove move)
    {
        bool dead = false;
        int hits = rnd.Next(2, 5);
        bool strongAttack = bs.playerUnit.GetWeaknessMultiplier(move) > 1;
        for (int i = 0; i < hits; i++)
        {
            if (!bs.playerUnit.TakeDamage(move, actualPlayer))
            {
                //Deal damage
                bs.playerHUD.setHP(actualPlayer.currentHP, actualPlayer);
                if (strongAttack) FindObjectOfType<AudioManager>().Play("superEffective");
                else FindObjectOfType<AudioManager>().Play("damage");
                yield return new WaitForSeconds(standardWaitTime / 2);
            }
            else //Unit dies
            {
                //Deal last hit
                bs.playerHUD.setHP(actualPlayer.currentHP, actualPlayer);
                if (strongAttack) FindObjectOfType<AudioManager>().Play("superEffective");
                else FindObjectOfType<AudioManager>().Play("damage");
                yield return new WaitForSeconds(standardWaitTime / 2);
                //Kill enemy
                if (strongAttack) StartCoroutine(bs.ScrollText("The attack is super effective!"));
                else StartCoroutine(bs.ScrollText("The attack is successful!"));
                yield return new WaitForSeconds(standardWaitTime);
                StartCoroutine(bs.RetrieveDeadUnit());
                dead = true;
                break;
            }
        }
        //Enemy survives
        if (!dead)
        {
            if (strongAttack) StartCoroutine(bs.ScrollText("The attack is super effective!"));
            else StartCoroutine(bs.ScrollText("The attack is successful!"));
            yield return new WaitForSeconds(standardWaitTime);
            EndTurn();
        }
    }

    private IEnumerator CompleteTwoTurnMove()
    {
        StartCoroutine(bs.ScrollText($"{bs.enemyUnit.goblinData.gName} lunges from the water!")); //this can be adjusted if another two turner is added later
        yield return new WaitForSeconds(standardWaitTime);
        bs.enemyUnit.GetComponent<SpriteRenderer>().sprite = bs.enemyUnit.goblinData.sprite;
        bool strongAttack = bs.playerUnit.GetWeaknessMultiplier(twoTurnMove) > 1;
        bool isDead = actualPlayer.TakeDamage(twoTurnMove, bs.enemyUnit);
        bs.playerHUD.setHP(bs.playerUnit.currentHP, bs.playerUnit);
        if (strongAttack) //If super effective 
        {
            FindObjectOfType<AudioManager>().Play("superEffective");
            yield return new WaitForSeconds(standardWaitTime / 2);
            StartCoroutine(bs.ScrollText("The attack is super effective!"));
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("damage");
            yield return new WaitForSeconds(standardWaitTime / 2);
            StartCoroutine(bs.ScrollText("The attack is successful!"));
        }
        yield return new WaitForSeconds(standardWaitTime);
        if (isDead)
        {
            twoTurnMove = null;
            StartCoroutine(bs.RetrieveDeadUnit());
        }
        else
        {
            twoTurnMove = null;
            EndTurn();
        }
    }
    //Buffs enemy
    public IEnumerator BuffEnemyAction(SOMove move)
    {
        StartCoroutine(bs.ScrollText($"{self.goblinData.gName} used {move.moveName}!"));
        yield return new WaitForSeconds(standardWaitTime);

        //Buff Self
        switch (move.moveModifier)
        {
            case SOMove.MoveModifier.ATTACK:
                {
                    self.attackModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (self.attackModifier > 6)
                    {
                        //clamp buff at 6
                        self.attackModifier = 6;
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s attack can't go any higher!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s attack was increased!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }

            case SOMove.MoveModifier.DEFENSE:
                {
                    self.defenseModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (self.defenseModifier > 6)
                    {
                        //clamp buff at 6
                        self.defenseModifier = 6;
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s defense can't go any higher!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{self.goblinData.gName}'s defense was increased!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }
            case SOMove.MoveModifier.NONE:
                {
                    Debug.LogWarning("WARNING:" + move.moveName + " does not have an assigned stat to modify. Check SO!");
                    break;
                }
        }

        //End Turn
        EndTurn();
    }

    //Debuffs Player
    public IEnumerator DebuffPlayerAction(SOMove move)
    {
        StartCoroutine(bs.ScrollText($"{self.goblinData.gName} used {move.moveName}!"));
        yield return new WaitForSeconds(standardWaitTime);

        //Debuff Enemy
        switch (move.moveModifier)
        {
            case SOMove.MoveModifier.ATTACK:
                {
                    actualPlayer.attackModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (actualPlayer.attackModifier < -6)
                    {
                        //clamp debuff at 6
                        actualPlayer.attackModifier = -6;
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s attack can't go any lower!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s attack was lowered!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }

            case SOMove.MoveModifier.DEFENSE:
                {
                    bs.enemyUnit.defenseModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (actualPlayer.defenseModifier < -6)
                    {
                        actualPlayer.defenseModifier = -6; //clamp
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s defense can't go any lower!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(bs.ScrollText($"{actualPlayer.goblinData.gName}'s defense was lowered!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }

            case SOMove.MoveModifier.NONE:
                {
                    Debug.LogWarning($"WARNING: {move.moveName} does not have an assigned stat to modify. Check SO!");
                    break;
                }
        }

        EndTurn();
    }
    #endregion

    //Updates player in calculations to actual player
    //Prevents ai from intentionally "predicting" a switch in
    public void UpdateInternalPlayerUnit()
    {
        //Debug.Log("[EnemyAI]: Updating internal Unit");
        internalPlayer.goblinData = actualPlayer.goblinData;
        internalPlayer.currentHP = actualPlayer.currentHP;
        internalPlayer.attackModifier = actualPlayer.attackModifier;
        internalPlayer.defenseModifier = actualPlayer.attackModifier;
    }

    //Update internal player then end turn
    public void EndTurn()
    {
        bs.state = BattleState.PLAYERTURN;
        if (internalPlayer != actualPlayer //Pretty sure this always updates player because of IDs 
        || internalPlayer.currentHP != actualPlayer.currentHP)
            UpdateInternalPlayerUnit();

        if (!bs.twoTurnMove) bm.enableBasicButtonsOnPress();
        bs.PlayerTurn();
    }
}
