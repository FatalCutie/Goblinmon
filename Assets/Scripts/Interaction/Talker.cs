using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Talker : MonoBehaviour, Interactable
{
    [SerializeField] private string prompt;
    [SerializeField] public string InteractionPrompt => prompt;

    //public Dialogue dialogue;
    //public List<Responses> responses;
    public bool canDoubleTalk;
    //public Dialogue secondDialogue;
    private bool hasTalkedTo;


    public bool Interact(Interactor interactor)
    {
        if (!hasTalkedTo || canDoubleTalk == false)
        {
            //FindObjectOfType<DialogueManager>().StartDialogue(dialogue); //MAKE CUBE TALK
            if (hasTalkedTo == false) hasTalkedTo = !hasTalkedTo;
            return true;
        }
        else
        {
            //FindObjectOfType<DialogueManager>().StartDialogue(secondDialogue); //MAKE CUBE AGAIN
            return true;
        }

    }

    public void TestResponses()
    {
        print("Has not been implemented!");
        //FindObjectOfType<ResponseManager>().ShowResponses(responses);
    }
}
