using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour//, Interactable
{
    [SerializeField] private string prompt;
    [SerializeField] public string InteractionPrompt => prompt;
    // private LevelLoader levelloader;

    // public bool Interact(Interactor interactor)
    // {
    //     FindObjectOfType<LevelLoader>().LoadNextLevel();
    //     return true;
    // }
}
