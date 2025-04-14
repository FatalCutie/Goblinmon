
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    #region Variables
    ButtonManager bm;
    private EnemyAI eAI;
    SwitchingManager sm;
    [SerializeField] private PartyStorage ps;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [SerializeField] private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;


    [SerializeField] private TextMeshProUGUI dialogueText;

    public Goblinmon playerUnit;
    public GameObject playerGO;
    public GameObject enemyGO;
    public Goblinmon enemyUnit;
    SpriteRenderer pSpriteR;
    SpriteRenderer eSpriteR;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    public System.Random rnd = new System.Random();
    private bool firstScroll = true;
    public float standardWaitTime = 1;
    private int playerRamping; //Turns left in debuffing player if ramped
    [SerializeField] public List<SOType> types; //For random type move
    public SOMove twoTurnMove; //Stores move if it's a two turner

    [SerializeField] public Animator playerAnimator;
    [SerializeField] public Animator battleAnimator;
    [SerializeField] public Animator enemyAnimator;
    public Animator enemyUIAnimator;
    public Animator playerUIAnimator;
    public bool SkipOpeningAnimations = false;

    #endregion
    void Start()
    {
        bm = FindObjectOfType<ButtonManager>();
        eAI = FindObjectOfType<EnemyAI>();
        sm = FindObjectOfType<SwitchingManager>();
        ps = FindObjectOfType<PartyStorage>();
        state = BattleState.START;
        //FindObjectOfType<AudioManager>().Play("battle");
        StartCoroutine(SetupBattle());
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.I)){
            state = BattleState.WON;
            EndBattle();
        }
        if(Input.GetKeyDown(KeyCode.O)){
            state = BattleState.LOST;
            EndBattle();
        }
    }

    IEnumerator SetupBattle()
    { //Prefab scales with battle station, fix is unclear

        //Check for first unit that isn't dead
        int j = 0;
        for (int i = 0; i < 6; i++)
        {
            if (ps.goblinmon[i].currentHP > 0)
            {
                j = i;
                break;
            }
        }

        //Instantiate battle based on above unit
        playerGO.GetComponent<Goblinmon>().goblinData = ps.goblinmon[j].goblinData;
        playerUnit = playerGO.GetComponent<Goblinmon>();
        // playerUnit.gameObject.transform.position += new UnityEngine.Vector3(0, 1, 0);
        playerUnit.currentHP = ps.goblinmon[j].currentHP;
        playerUnit.CloneIdFrom(ps.goblinmon[j]); //Identity theft is kickass, actually
        pSpriteR = playerUnit.GetComponent<SpriteRenderer>();
        pSpriteR.sprite = playerUnit.goblinData.sprite;

        enemyUnit = enemyGO.GetComponent<Goblinmon>();
        eSpriteR = enemyUnit.GetComponent<SpriteRenderer>();
        eAI.InitializeUnitsForEnemyAI(enemyUnit, playerUnit);
        eSpriteR.sprite = enemyUnit.goblinData.sprite;
        // enemyGO.transform.position += new UnityEngine.Vector3(0, 1, 0);

        //Updates the HUD
        playerHUD.SetHUD(playerUnit);
        bm.SetPlayerMoves(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        if (!SkipOpeningAnimations) yield return new WaitForSeconds(.5f); //to account for transition
        if (eAI.enemyType == EnemyAI.EnemyType.WILD)
        {
            StartCoroutine(ScrollText($"A wild {enemyUnit.goblinData.gName} approches!"));
            enemyAnimator.SetBool("EnemyBeingCaught", false);
            enemyUIAnimator.SetBool("PanelOpen", true);
            if (!SkipOpeningAnimations) yield return new WaitForSeconds(1);
        }
        else
        {
            StartCoroutine(ScrollText($"A trainer approaches!"));
            if (!SkipOpeningAnimations) yield return new WaitForSeconds(2);
            StartCoroutine(ScrollText($"They send out {enemyUnit.goblinData.gName}!"));
            yield return new WaitForSeconds(standardWaitTime);
            battleAnimator.SetTrigger("ThrowOutEnemy");
            if (!SkipOpeningAnimations) yield return new WaitForSeconds(0.67f);
            enemyAnimator.SetBool("EnemyBeingCaught", false);
            enemyUIAnimator.SetBool("PanelOpen", true);
        }

        //TODO: Enemy entry animation

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    #region Player Attacks

    //Runs correct Coroutine based on move action enum
    public void StartPlayerAttack(SOMove move)
    {
        bm.disableButtonsDuringAttack();
        switch (move.moveAction)
        {
            case SOMove.MoveAction.ATTACK:
                if (move.moveModifier == SOMove.MoveModifier.ATTACK) //Debug Pass move
                {
                    state = BattleState.ENEMYTURN;
                    eAI.FindOptimalOption();
                }
                else StartCoroutine(PlayerAttack(move)); //Move modifiers handled in IEnumerator
                break;
            case SOMove.MoveAction.BUFF:
                StartCoroutine(BuffPlayer(move));
                break;
            case SOMove.MoveAction.DEBUFF:
                StartCoroutine(DebuffEnemy(move));
                break;
            case SOMove.MoveAction.HEAL:
                StartCoroutine(PlayerHeal(move));
                break;
        }
    }

    public IEnumerator PlayerAttack(SOMove move)
    {
        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} used {move.name}!"));
        yield return new WaitForSeconds(standardWaitTime);
        if (!eAI.twoTurnMove)
        {
            //Multi Hit Move attack
            if (move.moveModifier == SOMove.MoveModifier.MULTI_HIT)
            {
                StartCoroutine(MultiHitAttack(move));
            }
            else if (move.moveModifier == SOMove.MoveModifier.TWO_TURN)
            {
                //Hide unit
                playerUnit.GetComponent<SpriteRenderer>().sprite = null;
                StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} dove under water!"));
                yield return new WaitForSeconds(standardWaitTime);

                //End turn, move will land in PlayerTurn() on the next turn
                twoTurnMove = move; //stash move for PlayerTurn()
                state = BattleState.ENEMYTURN;
                eAI.FindOptimalOption();
            }
            else
            {
                if (move.moveModifier == SOMove.MoveModifier.RANDOM_TYPE)
                {
                    int i = rnd.Next(0, types.Capacity);
                    SOType temp = types[i];
                    StartCoroutine(ScrollText($"The move switches type to {temp.name}!"));
                    yield return new WaitForSeconds(standardWaitTime);
                }

                bool isDead = enemyUnit.TakeDamage(move, playerUnit);

                enemyHUD.setHP(enemyUnit.currentHP, enemyUnit);
                if (enemyUnit.GetWeaknessMultiplier(move) > 1) //If super effective 
                {
                    FindObjectOfType<AudioManager>().Play("superEffective");
                    yield return new WaitForSeconds(standardWaitTime / 2);
                    StartCoroutine(ScrollText("The attack is super effective!"));
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("damage");
                    yield return new WaitForSeconds(standardWaitTime / 2);
                    StartCoroutine(ScrollText("The attack is successful!"));
                }
                if (move.moveModifier == SOMove.MoveModifier.RECOIL)
                {
                    yield return new WaitForSeconds(standardWaitTime);
                    int recoilDamage;
                    recoilDamage = Mathf.RoundToInt(move.damage * enemyUnit.GetWeaknessMultiplier(move) * (move.statModifier * 0.01f));
                    StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} was hurt by recoil!"));
                    playerUnit.currentHP -= recoilDamage;
                    if (playerUnit.currentHP < 0) playerUnit.currentHP = 0;
                    playerHUD.setHP(playerUnit.currentHP, playerUnit);
                    if (playerUnit.currentHP == 0)
                    {
                        StartCoroutine(RetrieveDeadUnit());
                    }
                }
                yield return new WaitForSeconds(standardWaitTime);
                if (isDead)
                {
                    StartCoroutine(KillEnemyUnit());
                }
                else
                {
                    state = BattleState.ENEMYTURN;
                    eAI.FindOptimalOption();
                }
            }
        }
        else
        {
            StartCoroutine(ScrollText("The attack missed!"));
            yield return new WaitForSeconds(standardWaitTime);
            state = BattleState.ENEMYTURN;
            eAI.FindOptimalOption();
        }

    }

    public IEnumerator MultiHitAttack(SOMove move)
    {
        bool dead = false;
        int hits = rnd.Next(2, 5);
        bool strongAttack = enemyUnit.GetWeaknessMultiplier(move) > 1;
        for (int i = 0; i < hits; i++)
        {
            if (!enemyUnit.TakeDamage(move, playerUnit))
            {
                //Deal damage
                enemyHUD.setHP(enemyUnit.currentHP, enemyUnit);
                if (strongAttack) FindObjectOfType<AudioManager>().Play("superEffective");
                else FindObjectOfType<AudioManager>().Play("damage");
                yield return new WaitForSeconds(standardWaitTime / 2);
            }
            else //Unit dies
            {
                //Deal last hit
                enemyHUD.setHP(enemyUnit.currentHP, enemyUnit);
                if (strongAttack) FindObjectOfType<AudioManager>().Play("superEffective");
                else FindObjectOfType<AudioManager>().Play("damage");
                yield return new WaitForSeconds(standardWaitTime / 2);
                //Kill enemy
                if (strongAttack) StartCoroutine(ScrollText("The attack is super effective!"));
                else StartCoroutine(ScrollText("The attack is successful!"));
                yield return new WaitForSeconds(standardWaitTime);
                StartCoroutine(KillEnemyUnit());
                dead = true;
                break;
            }
        }
        //Enemy survives
        if (!dead)
        {
            if (strongAttack) StartCoroutine(ScrollText("The attack is super effective!"));
            else StartCoroutine(ScrollText("The attack is successful!"));
            yield return new WaitForSeconds(standardWaitTime);
            state = BattleState.ENEMYTURN;
            eAI.FindOptimalOption();
        }
    }
    //This can be cleaned up
    public IEnumerator BuffPlayer(SOMove move)
    {
        StartCoroutine(ScrollText("Player used " + move.moveName + "!"));
        yield return new WaitForSeconds(standardWaitTime);

        //Buff Player
        switch (move.moveModifier)
        {
            case SOMove.MoveModifier.ATTACK:
                {
                    playerUnit.attackModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (playerUnit.attackModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.attackModifier = 6;
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack can't go any higher!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack was increased by {move.statModifier}!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }

            case SOMove.MoveModifier.DEFENSE:
                {
                    playerUnit.defenseModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (playerUnit.defenseModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.defenseModifier = 6;
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s defense can't go any higher!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s defense was increased by {move.statModifier}!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }
            case SOMove.MoveModifier.RAMP:
                {

                    playerUnit.attackModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (playerUnit.attackModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.attackModifier = 6;
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack can't go any higher!"));
                        playerRamping = 3; //Attack decreased for 3 turns
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack was increased by {move.statModifier}!"));
                        playerRamping = 3; //Attack decreased for 3 turns
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }
            case SOMove.MoveModifier.ATTACK_DEFENSE:
                {
                    playerUnit.defenseModifier += move.statModifier;
                    playerUnit.attackModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (playerUnit.defenseModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.defenseModifier = 6;
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s defense can't go any higher!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    if (playerUnit.attackModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.attackModifier = 6;
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack can't go any higher!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack and defense were increased by {move.statModifier}!"));
                    yield return new WaitForSeconds(standardWaitTime);
                    break;
                }

            case SOMove.MoveModifier.NONE:
                {
                    Debug.LogWarning("WARNING:" + move.moveName + " does not have an assigned stat to modify. Check SO!");
                    break;
                }
        }

        //End Turn
        state = BattleState.ENEMYTURN;
        eAI.FindOptimalOption();
    }
    //This can be cleaned up
    public IEnumerator DebuffEnemy(SOMove move)
    {
        StartCoroutine(ScrollText($"Player used {move.moveName}!"));
        yield return new WaitForSeconds(standardWaitTime);

        //Debuff Enemy
        switch (move.moveModifier)
        {
            case SOMove.MoveModifier.ATTACK:
                {
                    enemyUnit.attackModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (enemyUnit.attackModifier < -6)
                    {
                        //clamp debuff at 6
                        enemyUnit.attackModifier = -6;
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s attack can't go any lower!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s attack was lowered!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    break;
                }

            case SOMove.MoveModifier.DEFENSE:
                {
                    enemyUnit.defenseModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (enemyUnit.defenseModifier < -6)
                    {
                        enemyUnit.defenseModifier = -6; //clamp
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s defense can't go any lower!"));
                        yield return new WaitForSeconds(standardWaitTime);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s defense was lowered!"));
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

        //End Turn
        state = BattleState.ENEMYTURN;
        eAI.FindOptimalOption();
    }

    public IEnumerator PlayerHeal(SOMove move)
    {
        StartCoroutine(ScrollText("Player used " + move.moveName + "!"));
        yield return new WaitForSeconds(standardWaitTime);

        //Heal Player
        playerUnit.currentHP += move.damage; //heal
        if (playerUnit.currentHP > playerUnit.goblinData.maxHP) playerUnit.currentHP = playerUnit.goblinData.maxHP; //So player doesn't heal over max
        playerHUD.setHP(playerUnit.currentHP, playerUnit);
        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} restored their health!"));
        yield return new WaitForSeconds(standardWaitTime);

        //End Turn
        state = BattleState.ENEMYTURN;
        eAI.FindOptimalOption();
    }
    #endregion

    public void EndBattle()
    {
        if (state == BattleState.WON)
        {
            FindObjectOfType<AudioManager>().Stop("battle");
            FindObjectOfType<AudioManager>().Play("win");
            StartCoroutine(ScrollText("You won the battle!"));
            sm.SavePlayerData(); //Save health of active Goblinmon
            StartCoroutine(ReturnToOverworld());
            //StartCoroutine(BackToTitle());
        }
        else if (state == BattleState.LOST)
        {
            FindObjectOfType<AudioManager>().Stop("battle");
            FindObjectOfType<AudioManager>().Play("run");
            StartCoroutine(ScrollText("You were defeated."));
            StartCoroutine(ReturnToOverworld());
            //StartCoroutine(BackToTitle());
        }
    }

    public IEnumerator BackToTitle()
    {
        yield return new WaitForSeconds(10f);
        FindObjectOfType<AudioManager>().Stop("win");
        SceneManager.LoadScene("Title Screen");
    }

    public IEnumerator ReturnToOverworld()
    {
        yield return new WaitForSeconds(standardWaitTime * 3);
        FindObjectOfType<SceneController>().TransitionScene("OverworldScene");
        yield return new WaitForSeconds(2f); //Hardcoded with transition time
        FindObjectOfType<AudioManager>().Stop("win");
    }

    public void PlayerTurn()
    {
        if (playerRamping > 0)
        {
            playerRamping--;
            playerUnit.attackModifier--;
            StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack fell by 1!"));
            if (!twoTurnMove) return; //skip choose action text

        }
        if (twoTurnMove)
        {
            //Finish two turn move
            StartCoroutine(CompleteTwoTurnMove());
            return;
        }
        //Prevents text overlap
        if (firstScroll)
        {
            //Throw out unit
            StartCoroutine(ThrowOutUnit());
            firstScroll = !firstScroll;
        }
        else StartCoroutine(ScrollText("Choose an action:"));
    }

    //Makes text scroll across dialogue box
    public IEnumerator ScrollText(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;

            // if (Char.ToString(letter) == "." || Char.ToString(letter) == "!" || Char.ToString(letter) == "?")
            // {
            //     yield return new WaitForSeconds(.15f); //pause text with sentance completion, give dialogue a better flow
            // }
            // else if (Char.ToString(letter) == ",") yield return new WaitForSeconds(.1f);
            //else

            yield return new WaitForSeconds(standardWaitTime * .01f); //wait a bit to continue (number subject to change)
        }
    }

    public IEnumerator TempEnemyTurn()
    {
        //dialogueText.text = enemyUnit.goblinData.gName + " attacks!";
        dialogueText.text = "Enemy attacking is not implemented yet!";

        yield return new WaitForSeconds(1f);

        bool isDead = false;//playerUnit.TakeDamage(enemyUnit.damage);

        //playerHUD.setHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);
        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            bm.enableBasicButtonsOnPress();
            PlayerTurn();
        }
    }

    public IEnumerator KillEnemyUnit()
    {
        //Unit dies
        enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName} fainted!"));
        yield return new WaitForSeconds(standardWaitTime);

        //Check for more units
        eAI.SaveUnitData();
        if (eAI.CheckForMoreUnits())
        {
            Goblinmon switchIn = eAI.FindSafeSwitch(true);
            StartCoroutine(eAI.SwitchAction(switchIn, true));
        }
        else
        {
            state = BattleState.WON;
            EndBattle();
        }
    }

    private IEnumerator CompleteTwoTurnMove()
    {
        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} lunges from the water!")); //this can be adjusted if another two turner is added later
        yield return new WaitForSeconds(standardWaitTime);
        playerUnit.GetComponent<SpriteRenderer>().sprite = playerUnit.goblinData.sprite;
        bool strongAttack = enemyUnit.GetWeaknessMultiplier(twoTurnMove) > 1;
        bool isDead = enemyUnit.TakeDamage(twoTurnMove, playerUnit);
        enemyHUD.setHP(enemyUnit.currentHP, enemyUnit);
        if (strongAttack) //If super effective 
        {
            FindObjectOfType<AudioManager>().Play("superEffective");
            yield return new WaitForSeconds(standardWaitTime / 2);
            StartCoroutine(ScrollText("The attack is super effective!"));
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("damage");
            yield return new WaitForSeconds(standardWaitTime / 2);
            StartCoroutine(ScrollText("The attack is successful!"));
        }
        yield return new WaitForSeconds(standardWaitTime);
        if (isDead)
        {
            twoTurnMove = null;
            StartCoroutine(KillEnemyUnit());
        }
        else
        {
            twoTurnMove = null;
            state = BattleState.ENEMYTURN;
            eAI.FindOptimalOption();
        }
    }

    private IEnumerator ThrowOutUnit()
    {
        StartCoroutine(ScrollText($"Go, {playerUnit.goblinData.gName}!"));
        if (!SkipOpeningAnimations) yield return new WaitForSeconds(1);
        battleAnimator.SetTrigger("ThrowOutPlayer");
        yield return new WaitForSeconds(.84f); //hard coded for animation
        //Debug.Log("I'm Done!");
        //TODO: Play Sound Effect
        playerAnimator.SetBool("Player Out", true);
        playerUIAnimator.SetBool("PanelOpen", true);
        if (!SkipOpeningAnimations) yield return new WaitForSeconds(1.4f);
        StartCoroutine(ScrollText("Choose an action:"));
        bm.enableBasicButtonsOnPress();
    }

    public IEnumerator RetrieveDeadUnit()
    {
        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} Fainted!"));
        yield return new WaitForSeconds(1);
        playerAnimator.SetBool("Player Out", false);
        yield return new WaitForSeconds(.4f);
        battleAnimator.SetTrigger("RetrievePlayer");
        playerUIAnimator.SetBool("PanelOpen", false);
        yield return new WaitForSeconds(.5f);
        sm.SavePlayerData();
        yield return new WaitForSeconds(.5f);
        if (sm.DoesPlayerHaveUnits())
        {
            sm.GetNewPlayerUnit();
            //Continue Battle
        }
        else
        {
            state = BattleState.LOST;
            EndBattle();
        }
    }
}
