using UnityEngine.UIElements;

public interface Interactable
{
    
    public string InteractionPrompt { get; }

    public bool Interact(Interactor interactor);

}
