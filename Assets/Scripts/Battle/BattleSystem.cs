
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        playerUnit.currentHP = ps.goblinmon[j].currentHP;
        playerUnit.CloneIdFrom(ps.goblinmon[j]); //Identity theft is kickass, actually
        //Will have to adjust sprite positions during sprite production
        pSpriteR = playerUnit.GetComponent<SpriteRenderer>();
        pSpriteR.sprite = playerUnit.goblinData.sprite;

        //Instantiate enemy
        // GameObject enemyGO = Instantiate(enemyPrefab, new UnityEngine.Vector3(
        // enemyBattleStation.transform.position.x, enemyBattleStation.transform.position.y + 1f,
        // enemyBattleStation.transform.position.z), UnityEngine.Quaternion.identity, enemyBattleStation);
        // new Vector3(enemyBattleStation.transform.position.x, enemyBattleStation.transform.position.y + 50, enemyBattleStation.transform.position.z
        enemyUnit = enemyGO.GetComponent<Goblinmon>();
        eSpriteR = enemyUnit.GetComponent<SpriteRenderer>();
        eAI.InitializeUnitsForEnemyAI(enemyUnit, playerUnit);
        eSpriteR.sprite = enemyUnit.goblinData.sprite;

        //Updates the HUD
        playerHUD.SetHUD(playerUnit);
        bm.SetPlayerMoves(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(.5f); //to account for transition
        StartCoroutine(ScrollText($"A wild {enemyUnit.goblinData.gName} approches!"));

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
        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName} used {move.name}!"));
        yield return new WaitForSeconds(1.5f);
        bool strongAttack = enemyUnit.goblinData.type.weakAgainstEnemyType(move.moveType);
        if (strongAttack) //If super effective 
        {
            StartCoroutine(ScrollText("The attack is super effective!"));
            yield return new WaitForSeconds(1f);
            FindObjectOfType<AudioManager>().Play("superEffective");
        }
        else
        {
            StartCoroutine(ScrollText("The attack is successful!"));
            yield return new WaitForSeconds(1f);
            FindObjectOfType<AudioManager>().Play("damage");
        }

        bool isDead = enemyUnit.TakeDamage(move.damage, strongAttack, playerUnit);
        enemyHUD.setHP(enemyUnit.currentHP, enemyUnit);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            //Unit dies
            enemyUnit.GetComponent<SpriteRenderer>().sprite = null;
            StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName} fainted!"));
            yield return new WaitForSeconds(2f);

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
        else
        {
            state = BattleState.ENEMYTURN;
            eAI.FindOptimalOption();
        }

    }

    public IEnumerator BuffPlayer(SOMove move)
    {
        StartCoroutine(ScrollText("Player used " + move.moveName + "!"));
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
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack can't go any higher!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s attack was increased!"));
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
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s defense can't go any higher!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{playerUnit.goblinData.gName}'s defense was increased!"));
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
        StartCoroutine(ScrollText($"Player used {move.moveName}!"));
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
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s attack can't go any lower!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s attack was lowered!"));
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
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s defense can't go any lower!"));
                        yield return new WaitForSeconds(2f);
                    }
                    else
                    {
                        StartCoroutine(ScrollText($"{enemyUnit.goblinData.gName}'s defense was lowered!"));
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
        yield return new WaitForSeconds(5f);
        FindObjectOfType<SceneController>().TransitionScene("OverworldScene");
        yield return new WaitForSeconds(2f); //Hardcoded with transition time
        FindObjectOfType<AudioManager>().Stop("win");
    }

    public void PlayerTurn()
    {
        StartCoroutine(ScrollText("Choose an action:"));
    }

    //Makes text scroll across dialogue box
    public IEnumerator ScrollText(string sentence)
    {
        dialogueText.text = "";
        //TODO: This isn't properly clearing. Fix it 
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;

            // if (Char.ToString(letter) == "." || Char.ToString(letter) == "!" || Char.ToString(letter) == "?")
            // {
            //     yield return new WaitForSeconds(.15f); //pause text with sentance completion, give dialogue a better flow
            // }
            // else if (Char.ToString(letter) == ",") yield return new WaitForSeconds(.1f);
            //else

            yield return new WaitForSeconds(.01f); //wait a bit to continue (number subject to change)
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

}
