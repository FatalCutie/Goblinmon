using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [SerializeField] public Animator transitionAnim;
    [SerializeField] GameObject blackScreen;

    private void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    void Start(){
        if(!blackScreen.activeSelf) blackScreen.SetActive(true);
    }

    public void TransitionScene(string sceneName){
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string levelName){
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2); //This is hard coded. Adjust this if tranition anim is ever changed.
        SceneManager.LoadSceneAsync(levelName);
        transitionAnim.SetTrigger("Start");
    }
}
