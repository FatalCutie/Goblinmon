using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    // Start is called before the first frame update
    public DialogueManager dm;
    public DialogueSO end;
    public GameObject endingStuffs;
    public GameObject god;
    void Awake()
    {
        dm = FindObjectOfType<DialogueManager>();
    }
    void Start()
    {
        endingStuffs.SetActive(false);
        FindObjectOfType<AudioManager>().Play("end");
        StartCoroutine(dm.ScrollText(end, null));
    }

    public void EndGame()
    {
        god.SetActive(false);
        endingStuffs.SetActive(true);
    }

    public void Restart()
    {
        FindObjectOfType<SceneController>().TransitionScene("Title Screen");
        FindObjectOfType<AudioManager>().Stop("end");
    }

}
