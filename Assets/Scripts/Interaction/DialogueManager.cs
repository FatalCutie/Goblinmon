using System.Collections;
using System;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    private PlayerTileMovement pTM;
    [SerializeField] private Animator animator;
    public DialogueSO so;
    public GameObject nextLineImage;

    void Start()
    {
        pTM = FindObjectOfType<PlayerTileMovement>();
        nextLineImage.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)) animator.SetBool("Open", true);
        if(Input.GetKeyDown(KeyCode.P)) animator.SetBool("Open", false);
        if (Input.GetKeyDown(KeyCode.I)) StartCoroutine(ScrollText(so, null));
    }
    public IEnumerator ScrollText(DialogueSO so, NPC toReturn)
    {
        int i = 0;
        dialogueText.text = ""; //Clear previous text/sample text from editor
        //Open textbox
        animator.SetBool("Open", true);
        if (pTM) pTM.movementLocked = true;
        yield return new WaitForSeconds(0.25f); //Panel open animation takes a fourth of a second

        foreach (string line in so.text){
            //Scrolls text
            dialogueText.text = "";
            foreach (char letter in so.text[i].ToCharArray())
            {
                dialogueText.text += letter;

                if (Char.ToString(letter) == "." || Char.ToString(letter) == "!" || Char.ToString(letter) == "?")
                {
                    yield return new WaitForSeconds(.15f); //pause text with sentance completion, give dialogue a better flow
                }
                else if (Char.ToString(letter) == ",") yield return new WaitForSeconds(.1f);
                else yield return new WaitForSeconds(.01f); //wait a bit to continue (number subject to change)
            }

            //On standby until player inputs something to continue
            bool waitingForInput = true;
            while (waitingForInput)
            {
                nextLineImage.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
                {
                    waitingForInput = false;
                    nextLineImage.SetActive(false);
                }
                yield return null;  //Wait one frame before checking again
            }
            i++;     
        }
        //Close textbox when out of lines
        animator.SetBool("Open", false);
        if (pTM) pTM.movementLocked = false;
        if (toReturn)
        {
            if (toReturn.behavior == NPC.NPCBehavior.NPC_BATTLE)
            {
                toReturn.gameObject.GetComponent<TriggerBattleOverworld>().TriggerBattleSequence();
            }
            if (toReturn)
            {
                toReturn.canInteract = true;
            }
        }
        else
        {
            //If on title screen
            if (FindObjectOfType<TitleScreen>()) FindObjectOfType<TitleScreen>().RevealStarters();
        }
    }

}
