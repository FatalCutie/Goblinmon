using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;

    // Start is called before the first frame update
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            if (!FindObjectOfType<PlayerTileMovement>().movementLocked) return;
            interactableInRange?.Interact();
            //Close AFK panel if interacting
            if (FindObjectOfType<PlayerTileMovement>().idleTimer >= 3f)
            {
                FindObjectOfType<PlayerTileMovement>().idleTimer = 0;
                FindObjectOfType<OverworldUI>().CloseItemMenuOnIdle();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable))
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable))
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
