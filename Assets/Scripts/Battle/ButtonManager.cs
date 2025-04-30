using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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
        switchingMenu.SetActive(true);
        //TODO: Update goblinmon on open?
    }

    public void disableSwitchingMenu()
    {
        FindObjectOfType<AudioManager>().Play("press");
        switchingMenu.SetActive(false);
        buttonsBasic.SetActive(true);
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
