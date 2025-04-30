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


    void Start(){
        titleScreenUI.SetActive(true);
        infoScreenUI.SetActive(false);
        unitChoice.SetActive(false);
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
        FindObjectOfType<AudioManager>().Play("introduction");
        titleScreenUI.SetActive(false);
        StartCoroutine(dm.ScrollText(openingCrawl, null));
    }

    public void RevealStarters()
    {
        unitChoice.SetActive(true);
    }

    public void LoadIntoGame()
    {
        FindObjectOfType<AudioManager>().Stop("introduction");
        sc.TransitionScene("Overworld");
    }

}
