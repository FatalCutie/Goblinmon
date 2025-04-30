using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public DialogueManager dm;
    public DialogueSO speech;
    public enum NPCBehavior { NPC_TALK, NPC_BATTLE, NPC_SHOP, NPC_HEAL };
    public NPCBehavior behavior = NPCBehavior.NPC_TALK;
    public bool canInteract = true;

    public void Interact()
    {
        if(canInteract){
            canInteract = false;

            switch (behavior)
            {
                case NPCBehavior.NPC_TALK:
                    StartCoroutine(dm.ScrollText(speech, this));
                    break;
                case NPCBehavior.NPC_BATTLE:
                    StartCoroutine(dm.ScrollText(speech, this));
                    break;
                case NPCBehavior.NPC_SHOP:
                    FindObjectOfType<OverworldUI>().OpenShopUI();
                    canInteract = true;
                    break;
                case NPCBehavior.NPC_HEAL:
                    HealPlayerUnits();
                    StartCoroutine(dm.ScrollText(speech, this));
                    break;
            }
        }
    }

    public void HealPlayerUnits()
    {
        PartyStorage ps = FindObjectOfType<PartyStorage>();
        for (int i = 0; i < ps.goblinmon.Count; i++)
        {
            ps.goblinmon[i].currentHP = ps.goblinmon[i].goblinData.maxHP;
        }
        FindObjectOfType<AudioManager>().Play("catch");
    }

    void Start()
    {
        dm = FindObjectOfType<DialogueManager>();
    }

}
