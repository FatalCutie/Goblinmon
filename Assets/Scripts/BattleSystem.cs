
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    #region Variables
    ButtonManager bm;

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
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    { //Prefab scales with battle station, fix is unclear
    
        //Instantiate player
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Goblinmon>();
        //Will have to adjust sprite positions during sprite production
        pSpriteR = playerUnit.GetComponent<SpriteRenderer>();
        pSpriteR.sprite = playerUnit.goblinData.sprite;

        //Instantiate enemy
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Goblinmon>();
        eSpriteR = enemyUnit.GetComponent<SpriteRenderer>();
        eSpriteR.sprite = enemyUnit.goblinData.sprite;

        dialogueText.text = "A wild " + enemyUnit.goblinData.gName + " approches!";

        //Updates the HUD
        playerHUD.SetHUD(playerUnit);
        bm.SetPlayerMoves(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    //Runs correct function based on move info
    public void StartPlayerAttack(SOMove move)
    {
        bm.disableButtonsDuringAttack();
        StartCoroutine(PlayerAttack(move));
    }

    #region Player Attacks
    public IEnumerator PlayerAttack(SOMove move)
    {
        int moveDamage = move.damage;
        bool strongAttack = enemyUnit.goblinData.type.weakAgainstEnemyType(move.moveType);
        if (strongAttack) //If super effective 
        {
            dialogueText.text = "The attack is super effective!";
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("superEffective");
            bool isDead = enemyUnit.TakeDamage(move.damage, strongAttack);
            enemyHUD.setHP(enemyUnit.currentHP);
            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.WON;
                print(state);
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else
        {
            dialogueText.text = "The attack is successful!";
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("damage");
            bool isDead = enemyUnit.TakeDamage(move.damage, strongAttack);
            enemyHUD.setHP(enemyUnit.currentHP);
            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.WON;
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
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
        StartCoroutine(EnemyTurn());
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
        StartCoroutine(EnemyTurn());
    }
    #endregion

    public IEnumerator EnemyTurn()
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

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal() //I don't know why I haven't deleted this
    {
        playerUnit.Heal(5); //Don't forget this is hard coded :trolla:
        playerHUD.setHP(playerUnit.currentHP);
        dialogueText.text = "You feel new stength!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

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
