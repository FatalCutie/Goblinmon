
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    #region Variables
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

    #endregion
    void Start()
    {
        bMan = GetComponent<ButtonManager>();
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
        bMan.SetPlayerMoves(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    public void StartPlayerAttack(SOMove move)
    {
        bMan.disableButtonsDuringAttack();
        StartCoroutine(PlayerAttack(move));
    }
    public IEnumerator PlayerAttack(SOMove move)
    {
        int moveDamage = move.damage;
        bool strongAttack = enemyUnit.goblinData.type.weakAgainstEnemyType(move.moveType);
        if (strongAttack)
        {
            dialogueText.text = "The attack is super effective!";
            yield return new WaitForSeconds(1f);

            FindObjectOfType<AudioManager>().Play("superEffective");
            bool isDead = GetComponent<BattleSystem>().enemyUnit.TakeDamage(move.damage, strongAttack);
            enemyHUD.setHP(enemyUnit.currentHP);
            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.WON;
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

    public IEnumerator EnemyTurn()
    {
        //dialogueText.text = enemyUnit.goblinData.gName + " attacks!";
        dialogueText.text = "Enemy attacking is not implemented yet!";

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
            bMan.enableBasicButtonsOnPress();
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
