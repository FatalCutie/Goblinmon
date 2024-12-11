using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    public bool interacting = false;

    private readonly Collider2D[] colliders = new Collider2D[3]; //set to 3 for performance 
    [SerializeField] private int numFound; //testing variable
    private Interactable interactable;

    private void Update()
    {
        if (!interacting) //Pauses checking for interactables if player is interacting with something
        {
            numFound = Physics2D.OverlapCircleNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask); //Increments numFound based on interactables

            if (numFound > 0)
            {
                interactable = colliders[0].GetComponent<Interactable>();

                if (interactable != null)
                {
                    if (!interactionPromptUI.IsDisplayed) interactionPromptUI.SetUp(interactable.InteractionPrompt);

                    // if(InputManager.instance.getKeyDown("interact"))
                    // {
                    //     interactable.Interact(this);
                    // } 
                }
            }
            else
            {
                if (interactable != null) interactable = null;
                if (interactionPromptUI.IsDisplayed) interactionPromptUI.Close();
            }
        }
        else
        {
            if (interactable != null) interactable = null;
            if (interactionPromptUI.IsDisplayed) interactionPromptUI.Close();
        }

    }

}
