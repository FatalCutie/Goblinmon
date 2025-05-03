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


    void Start(){
        titleScreenUI.SetActive(true);
        infoScreenUI.SetActive(false);
        unitChoice.SetActive(false);
        jugan.SetActive(false);
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
        FindObjectOfType<AudioManager>().Stop("introduction");
        sc.TransitionScene("Overworld");
    }

}
