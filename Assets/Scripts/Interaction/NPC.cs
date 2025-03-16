using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public DialogueManager dm;
    public DialogueSO speech;
    public bool canInteract = true;
    public bool triggerBattle = false;

    public void Interact()
    {
        if(canInteract){
            canInteract = false;
            StartCoroutine(dm.ScrollText(speech, this));
        }
    }

    void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
        if(this.gameObject.GetComponent<TriggerBattleOverworld>()) triggerBattle = true;
    }

}
