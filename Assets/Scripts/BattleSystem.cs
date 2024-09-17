
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] ButtonManager bMan;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public TextMeshProUGUI dialogueText;

    public Goblinmon playerUnit;
    public Goblinmon enemyUnit;
    SpriteRenderer pSpriteR;
    SpriteRenderer eSpriteR;
    public Sprite newSprite;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    void Start()
    {
        bMan = GetComponent<ButtonManager>();
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    { //Prefab scales with battle station, fix is unclear
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Goblinmon>();
        //Will have to adjust sprite positions during sprite production
        pSpriteR = playerUnit.GetComponent<SpriteRenderer>();
        pSpriteR.sprite = playerUnit.sprite;

        //Not instantiating correctly, type specifically
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Goblinmon>();
        eSpriteR = enemyUnit.GetComponent<SpriteRenderer>();
        eSpriteR.sprite = enemyUnit.sprite;

        dialogueText.text = "A wild " + enemyUnit.gName + " approches!";

        playerHUD.SetHUD(playerUnit);
        bMan.SetPlayerMoves(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    // public IEnumerator PlayerAttack() //Triggered by OnAttackButton
    // {
    //     dialogueText.text = "The attack is successful!";

    //     yield return new WaitForSeconds(1f);

    //     bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
    //     enemyHUD.setHP(enemyUnit.currentHP);


    //     yield return new WaitForSeconds(2f);

    //     if (isDead)
    //     {
    //         state = BattleState.WON;
    //     }
    //     else
    //     {
    //         state = BattleState.ENEMYTURN;
    //         StartCoroutine(EnemyTurn());
    //     }

    //     //Check if enemy is dead
    //     //Change state accordingly
    // }

    public IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.gName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = false;//playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.setHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);
        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
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
