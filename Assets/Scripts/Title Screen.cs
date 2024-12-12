using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    

    public GameObject titleScreenUI;
    public GameObject infoScreenUI;

    void Start(){
        titleScreenUI.SetActive(true);
        infoScreenUI.SetActive(false);
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
}
