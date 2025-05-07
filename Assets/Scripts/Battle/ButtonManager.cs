using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonsBasic;
    public GameObject buttonsAttack;
    public GameObject switchingMenu;
    public TextMeshProUGUI captureItemsCount;
    [SerializeField] private UnityEngine.UI.Image catchButtonImage;
    [SerializeField] private UnityEngine.UI.Image runButtonImage;
    [SerializeField] private BattleSystem bs;
    [SerializeField] private PlayerPositionManager ppm;
    [SerializeField] private List<AttackButton> attackButtons;
    public bool releaseMode = false;
    public bool cantClose = false;
    public bool guaranteeRun = false;


    void Start()
    {
        if (buttonsAttack.activeSelf) buttonsAttack.SetActive(false);
        if (buttonsBasic.activeSelf) buttonsBasic.SetActive(false);
        switchingMenu.SetActive(false);
    }
    void Update()
    {
        if (!ppm) ppm = FindObjectOfType<PlayerPositionManager>();
        UpdateCatcherCount();
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (guaranteeRun)
            {
                guaranteeRun = !guaranteeRun;
                FindObjectOfType<AudioManager>().Play("damage");
            }
            else
            {
                guaranteeRun = !guaranteeRun;
                FindObjectOfType<AudioManager>().Play("press");
            }
        }
    }

    //If disabling the game object raises issues in the future
    //you can try disabling the renderer instead?
    #region Button Press Commands
    public void enableAttackButtonsOnPress()
    {
        FindObjectOfType<AudioManager>().Play("press");
        buttonsAttack.SetActive(true);
        SetPlayerMoves(bs.playerUnit);
        buttonsBasic.SetActive(false);
    }

    public void disableButtonsDuringAttack()
    {
        buttonsBasic.SetActive(false);
        buttonsAttack.SetActive(false);
    }

    public void enableBasicButtonsOnPress()
    {
        FindObjectOfType<AudioManager>().Play("press");
        buttonsAttack.SetActive(false);
        buttonsBasic.SetActive(true);
    }

    public void enableSwitchingMenu()
    {
        FindObjectOfType<AudioManager>().Play("press");
        disableButtonsDuringAttack();
        cantClose = false;
        switchingMenu.SetActive(true);
        //TODO: Update goblinmon on open?
    }

    public void disableSwitchingMenu()
    {
        if (cantClose)
        {
            FindObjectOfType<AudioManager>().Play("damage");
        }
        else if (!releaseMode)
        {
            FindObjectOfType<AudioManager>().Play("press");
            switchingMenu.SetActive(false);
            buttonsBasic.SetActive(true);
        }
        else
        {
            HideSwitchingMenuAfterCapture();
            bs.state = BattleState.WON;
            bs.EndBattle();
        }
    }

    public void HideSwitchingMenuAfterCapture()
    {
        FindObjectOfType<AudioManager>().Play("press");
        switchingMenu.SetActive(false);
    }

    public void UpdateCatcherCount()
    {
        if (int.TryParse(captureItemsCount.text.TrimStart('x'), out int currentCount))
        {
            if (currentCount != ppm.captureItems)
            {
                captureItemsCount.text = $"x{ppm.captureItems}";
            }
        }
    }

    public void Catch()
    {
        if (CheckForCatchingResources() && CheckIfCatchingTrainersUnit())
        {
            FindObjectOfType<AudioManager>().Play("press");
            disableButtonsDuringAttack();
            ppm.captureItems--;
            StartCoroutine(FindObjectOfType<CatchSystem>().AttemptToCatch(bs.enemyUnit));
        }

    }

    public bool CheckForCatchingResources()
    {
        if (ppm.captureItems > 0)
        {
            return true;
        }
        else
        {
            StartCoroutine(FlashCatchingButtonRed());
            return false;
        }
    }

    public bool CheckIfCatchingTrainersUnit()
    {
        if (FindObjectOfType<EnemyPartyStorage>().battleMusic == TriggerBattleOverworld.BattleMusic.BM_TRAINER
            || FindObjectOfType<EnemyPartyStorage>().battleMusic == TriggerBattleOverworld.BattleMusic.BM_ELITE)
        {
            StartCoroutine(bs.ScrollText("You can't catch another trainers monster!"));
            StartCoroutine(FlashCatchingButtonRed());
            return false;
        }
        else return true;
    }

    private IEnumerator FlashCatchingButtonRed()
    {
        catchButtonImage.color = Color.red;
        FindObjectOfType<AudioManager>().Play("damage");
        yield return new WaitForSeconds(.2f);
        catchButtonImage.color = Color.white;
    }

    public void RunDecider()
    {
        if (guaranteeRun) TestingRun();
        else RunAway();
    }

    public void TestingRun()
    {
        switch (FindAnyObjectByType<EnemyPartyStorage>().battleMusic)
        {
            case TriggerBattleOverworld.BattleMusic.BM_TRAINER:
                StartCoroutine(bs.ScrollText("You can't flee from a trainer fight!"));
                StartCoroutine(FlashRunButtonRed());
                return;
            case TriggerBattleOverworld.BattleMusic.BM_ELITE:
                StartCoroutine(bs.ScrollText("You can't flee from a trainer fight!"));
                StartCoroutine(FlashRunButtonRed());
                return;
            case TriggerBattleOverworld.BattleMusic.BM_LEGENDARY:
                FindObjectOfType<AudioManager>().Stop("battleLegendary");
                break;
            case TriggerBattleOverworld.BattleMusic.BM_WILD:
                FindObjectOfType<AudioManager>().Stop("battleWild");
                break;
        }
        FindObjectOfType<AudioManager>().Play("run");
        bs.ScrollText("You ran away!");
        FindObjectOfType<SceneController>().TransitionScene("Overworld");
    }

    public void RunAway()
    {
        if (!DoesPlayerGetAway())
        {
            StartCoroutine(PlayerFailsToRun());
        }
        else
        {
            disableButtonsDuringAttack();
            switch (FindAnyObjectByType<EnemyPartyStorage>().battleMusic)
            {
                case TriggerBattleOverworld.BattleMusic.BM_TRAINER:
                    StartCoroutine(bs.ScrollText("You can't flee from a trainer fight!"));
                    StartCoroutine(FlashRunButtonRed());
                    return;
                case TriggerBattleOverworld.BattleMusic.BM_ELITE:
                    StartCoroutine(bs.ScrollText("Don't think you can run from me!"));
                    StartCoroutine(FlashRunButtonRed());
                    return;
                case TriggerBattleOverworld.BattleMusic.BM_LEGENDARY:
                    FindObjectOfType<AudioManager>().Stop("battleLegendary");
                    FindObjectOfType<AudioManager>().Play("run");
                    StartCoroutine(bs.ScrollText("You ran away!"));
                    FindObjectOfType<SceneController>().TransitionScene("Overworld");
                    break;
                case TriggerBattleOverworld.BattleMusic.BM_WILD:
                    FindObjectOfType<AudioManager>().Stop("battleWild");
                    FindObjectOfType<AudioManager>().Play("run");
                    StartCoroutine(bs.ScrollText("You ran away!"));
                    FindObjectOfType<SceneController>().TransitionScene("Overworld");
                    break;
            }
        }
    }

    public IEnumerator PlayerFailsToRun()
    {
        disableButtonsDuringAttack();
        StartCoroutine(bs.ScrollText("You couldn't get away!"));
        yield return new WaitForSeconds(bs.standardWaitTime);
        bs.EndTurn();
    }

    //This is ripped from CatchSystem. It may have to be updated if CatchSystem is changed
    public bool DoesPlayerGetAway()
    {
        SOGoblinmon gd = bs.enemyUnit.goblinData;
        float number = gd.maxHP * gd.catchRate * 4 / (bs.enemyUnit.currentHP * 10);
        float numberToBeat = bs.rnd.Next(0, 255);
        if (number * 2 >= numberToBeat) return true;
        return false;
    }

    private IEnumerator FlashRunButtonRed()
    {
        runButtonImage.color = Color.red;
        FindObjectOfType<AudioManager>().Play("damage");
        yield return new WaitForSeconds(.2f);
        runButtonImage.color = Color.white;
    }

    public void disableSwitchingMenuAfterSwitch(){
        switchingMenu.SetActive(false);
    }

    public void unimplementedButtonError()
    {
        Debug.LogWarning("Pressed button is not yet Implemented! If this error is unexpected please check your code!");
    }
    #endregion

    public void SetPlayerMoves(Goblinmon unit)
    //This can be improved but I'm so tired of working on it
    { 
        int i = 0;
        try
        {   //Sets up attack buttons on HUD
            foreach (Transform go in buttonsAttack.transform)
            {
                TextMeshProUGUI moveNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                AttackButton ab = go.GetComponent<AttackButton>();
                ab.move = unit.goblinData.moveset[i];
                moveNameText.text = ab.move.moveName;
                ab.gameObject.SetActive(true);
                i++;
            }
        }catch (ArgumentOutOfRangeException) {} //The Goblinmon has run out of moves

        //Hides any unused move slots
        for (int j = i; j < 4; j++)
        {
            attackButtons[j].gameObject.SetActive(false);
        }
    }
    
}
