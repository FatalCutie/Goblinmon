using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldUI : MonoBehaviour
{
    [SerializeField] private Animator UnitMenuAnimator;
    private bool UnitMenuOpen;
    [SerializeField] private Animator ItemUIAnimator;
    private bool ItemUIOpen;
    public GameObject unitMenu;
    public GameObject fusionItemUI;
    public GameObject catchingItemUI;
    public PlayerTileMovement player;

    public void fucker(){
        UnitMenuAnimator.SetBool("PanelOpen", true);

    }

    void Update()
    {
        //Open and close unit menu panel
        if(Input.GetKeyDown(KeyCode.Tab)){
            switch(UnitMenuOpen){
                case false:
                    UnitMenuAnimator.SetBool("PanelOpen", true);
                    if(!ItemUIOpen) ItemUIAnimator.SetBool("ItemsOpen", true);
                    player.movementLocked = true;
                    UnitMenuOpen = true;
                    return;
                case true:
                    UnitMenuAnimator.SetBool("PanelOpen", false);
                    ItemUIAnimator.SetBool("ItemsOpen", false);
                    player.idleTimer = 0f;
                    player.playerIsIdle = false;
                    player.movementLocked = false;
                    UnitMenuOpen = false;
                    return;
            }
        }
    }

    public void OpenItemMenuOnIdle(){
        ItemUIAnimator.SetBool("ItemsOpen", true);
    }

    public void CloseItemMenuOnIdle(){
        ItemUIAnimator.SetBool("ItemsOpen", false);
    }
}
