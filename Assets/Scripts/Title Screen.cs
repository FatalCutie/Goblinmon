using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreenUI;
    public GameObject infoScreenUI;
    public SceneController sc;
    public DialogueManager dm;
    public DialogueSO openingCrawl;
    public GameObject unitChoice;
    public bool skipOpening = false;
    public GameObject jugan;
    public SpriteRenderer background;
    public List<Sprite> sprites;
    public Animator animator;




    void Start(){
        titleScreenUI.SetActive(true);
        infoScreenUI.SetActive(false);
        unitChoice.SetActive(false);
        jugan.SetActive(false);
        StartCoroutine(OpenTitleScreen());
    }

    public IEnumerator OpenTitleScreen()
    {
        yield return new WaitForSeconds(1);
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Mon");
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Buttons");
        yield return new WaitForSeconds(1.5f);
        animator.SetTrigger("Float");
        animator.SetBool("Done", true);
        FindObjectOfType<AudioManager>().Play("title");
    }

    public void RestartTitle()
    {
        animator.SetBool("Done", false);
        animator.SetTrigger("Restart");
        FindObjectOfType<AudioManager>().Stop("title");
        FindObjectOfType<OopsieScript>().SetMenuItemsInactive();
        StartCoroutine(OpenTitleScreen());
    }

    public void LoadBattleScene(){
        FindObjectOfType<AudioManager>().Play("press");
        SceneManager.LoadScene("BattleScene");
    }

    public void DisplayInfo(){
        FindObjectOfType<AudioManager>().Play("press");
        titleScreenUI.SetActive(false);
        infoScreenUI.SetActive(true);
    }

    public void ReturnToTitle(){
        FindObjectOfType<AudioManager>().Play("press");
        titleScreenUI.SetActive(true);
        infoScreenUI.SetActive(false);
        animator.SetBool("Done", true);
    }

    public void BeginOpeningCrawl()
    {
        if (skipOpening)
        {
            titleScreenUI.SetActive(false);
            unitChoice.SetActive(true);
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("introduction");
            FindObjectOfType<AudioManager>().Stop("title");
            titleScreenUI.SetActive(false);
            jugan.SetActive(true);
            StartCoroutine(dm.ScrollText(openingCrawl, null));
        }
    }

    public void OpeningCrawlToggle(bool toggleValue)
    {
        skipOpening = toggleValue;
    }

    public void RevealStarters()
    {
        jugan.SetActive(false);
        unitChoice.SetActive(true);
    }

    public void LoadIntoGame()
    {
        unitChoice.SetActive(false); //Only one starter for you
        try
        {
            FindObjectOfType<AudioManager>().Stop("introduction");
            FindObjectOfType<AudioManager>().Stop("title");
        }
        catch (NullReferenceException) { } //For if you skip intro or not

        if (!sc) sc = FindObjectOfType<SceneController>();
        sc.TransitionScene("Overworld");
    }

}
