
using System.Collections;
using TMPro;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    #region Variables
    ButtonManager bm;
    private EnemyAI eAI;
    SwitchingManager sm;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [SerializeField] private Transform playerBattleStation;
    [SerializeField] private Transform enemyBattleStation;

    public TextMeshProUGUI dialogueText;

    public Goblinmon playerUnit;
    public Goblinmon enemyUnit;
    SpriteRenderer pSpriteR;
    SpriteRenderer eSpriteR;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    #endregion
    void Start()
    {
        bm = FindObjectOfType<ButtonManager>();
        eAI = FindObjectOfType<EnemyAI>();
        sm = FindObjectOfType<SwitchingManager>();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    { //Prefab scales with battle station, fix is unclear
    
        //Instantiate player
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerGO.GetComponent<Goblinmon>().goblinData = sm.goblinmon[0];
        playerUnit = playerGO.GetComponent<Goblinmon>();
        //Will have to adjust sprite positions during sprite production
        pSpriteR = playerUnit.GetComponent<SpriteRenderer>();
        pSpriteR.sprite = playerUnit.goblinData.sprite;

        //Instantiate enemy
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Goblinmon>();
        eSpriteR = enemyUnit.GetComponent<SpriteRenderer>();
        eSpriteR.sprite = enemyUnit.goblinData.sprite;
        eAI.InitilizeUnitsForEnemyAI(enemyUnit, playerUnit);


        dialogueText.text = "A wild " + enemyUnit.goblinData.gName + " approches!";

        //Updates the HUD
        playerHUD.SetHUD(playerUnit);
        bm.SetPlayerMoves(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    #region Player Attacks

    //Runs correct Coroutine based on move action
    public void StartPlayerAttack(SOMove move)
    {
        bm.disableButtonsDuringAttack();
        switch (move.moveAction)
        {
            case SOMove.MoveAction.ATTACK:
                StartCoroutine(PlayerAttack(move));
                break;
            case SOMove.MoveAction.BUFF:
                StartCoroutine(BuffPlayer(move));
                break;
            case SOMove.MoveAction.DEBUFF:
                StartCoroutine(DebuffEnemy(move));
                break;
        }
    }


    public IEnumerator PlayerAttack(SOMove move)
    {
        dialogueText.text = $"{playerUnit.goblinData.name} used {move.name}!";
        yield return new WaitForSeconds(1.5f);
        bool strongAttack = enemyUnit.goblinData.type.weakAgainstEnemyType(move.moveType);
        if (strongAttack) //If super effective 
        {
            dialogueText.text = "The attack is super effective!";

            FindObjectOfType<AudioManager>().Play("superEffective");
        }
        else
        {
            dialogueText.text = "The attack is successful!";

            FindObjectOfType<AudioManager>().Play("damage");
        }

        bool isDead = enemyUnit.TakeDamage(move.damage, strongAttack, playerUnit);
        enemyHUD.setHP(enemyUnit.goblinData.currentHP);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            eAI.FindOptimalOption();
        }

    }

    public IEnumerator BuffPlayer(SOMove move)
    {
        dialogueText.text = "Player used " + move.moveName + "!";
        yield return new WaitForSeconds(2f);

        //Buff Player
        switch (move.statModified)
        {
            case SOMove.StatModified.ATTACK:
                {
                    playerUnit.attackModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (playerUnit.attackModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.attackModifier = 6;
                        dialogueText.text = $"{playerUnit.goblinData.gName}'s attack can't go any higher!";
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        dialogueText.text = $"{playerUnit.goblinData.gName}'s attack was increased!";
                        yield return new WaitForSeconds(2f);
                    }
                    break;
                }

            case SOMove.StatModified.DEFENSE:
                {
                    playerUnit.defenseModifier += move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning("WARNING: " + move.moveName + "s stat modifier is 0. Is this intentional?");
                    if (playerUnit.defenseModifier > 6)
                    {
                        //clamp buff at 6
                        playerUnit.defenseModifier = 6;
                        dialogueText.text = $"{playerUnit.goblinData.gName}'s defense can't go any higher!";
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        dialogueText.text = $"{playerUnit.goblinData.gName}'s defense was increased!";
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
        state = BattleState.ENEMYTURN;
        eAI.FindOptimalOption();
    }

    public IEnumerator DebuffEnemy(SOMove move)
    {
        dialogueText.text = $"Player used {move.moveName}!";
        yield return new WaitForSeconds(2f);

        //Debuff Enemy
        switch (move.statModified)
        {
            case SOMove.StatModified.ATTACK:
                {
                    enemyUnit.attackModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (enemyUnit.attackModifier < -6)
                    {
                        //clamp debuff at 6
                        enemyUnit.attackModifier = -6;
                        dialogueText.text = $"{enemyUnit.goblinData.gName}'s attack can't go any lower!";
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        dialogueText.text = $"{enemyUnit.goblinData.gName}'s attack was lowered!";
                        yield return new WaitForSeconds(2f);
                    }
                    break;
                }

            case SOMove.StatModified.DEFENSE:
                {
                    enemyUnit.defenseModifier -= move.statModifier;
                    if (move.statModifier <= 0) Debug.LogWarning($"WARNING: {move.moveName}s stat modifier is 0. Is this intentional?");
                    if (enemyUnit.defenseModifier < -6)
                    {
                        enemyUnit.defenseModifier = -6; //clamp
                        dialogueText.text = $"{enemyUnit.goblinData.gName}'s defense can't go any lower!";
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        dialogueText.text = $"{playerUnit.goblinData.gName}'s defense was lowered!";
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
        state = BattleState.ENEMYTURN;
        eAI.FindOptimalOption();
    }
    #endregion

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

    public void EndBattle()
    {
        if (state == BattleState.WON)
        {
            FindObjectOfType<AudioManager>().Play("win");
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            FindObjectOfType<AudioManager>().Play("run");
            dialogueText.text = "You were defeated.";
        }
    }

    public void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }


    // public void OnAttackButton()
    // {
    //     if (state != BattleState.PLAYERTURN) return;

    //     StartCoroutine(PlayerAttack());
    // }

    // public void OnHealButton()
    // {
    //     if (state != BattleState.PLAYERTURN) return;

    //     StartCoroutine(PlayerHeal());
    // }



}
