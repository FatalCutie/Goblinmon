using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages players position when switching scenes
public class PlayerPositionManager : MonoBehaviour
{
    [SerializeField] private Vector3 playerPosition = new Vector3();
    public static PlayerPositionManager instance;
    public int fusionItems;
    public int captureItems;
    public int playerMoney;
    public bool chungusBattled = false;
    public bool lost;
    public DialogueSO losingText;
    public float movepointOffset = 0.1f;
    public ZoneManager.Area area = ZoneManager.Area.AREA_TOWN;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void CheckIfLoser()
    {
        if (lost)
        {
            lost = !lost;
            StartCoroutine(IfPlayerLost());
        }
    }


    public IEnumerator IfPlayerLost()
    {
        StartCoroutine(FindObjectOfType<DialogueManager>().ScrollText(losingText, null));
        FindObjectOfType<ZoneManager>().area = ZoneManager.Area.AREA_CENTER;
        yield return new WaitForSeconds(.5f);
        FindObjectOfType<NPC>().HealPlayerUnits();
    }

    public void SavePlayersPosition(){
        Transform player = FindObjectOfType<PlayerTileMovement>().movepoint;
        playerPosition = player.position + new Vector3(0, movepointOffset, 0);
        area = FindObjectOfType<ZoneManager>().area;

    }

    //Places player in previous position
    //If you have problems with player breaking grid check player position and PlayerMovementManager position
    public void RememberPlayerPosition(){
        PlayerTileMovement player = FindObjectOfType<PlayerTileMovement>();
        //If player position is set and doesen't match with internal position
        if (playerPosition != new Vector3(2.625f, -0.25f, 0) && player.movepoint.position != playerPosition)
        {
            player.movementLocked = true;
            player.movepoint.position = playerPosition - new Vector3(0f, movepointOffset, 0); //Move movepoint first 
            player.gameObject.transform.position = playerPosition; //Then actual player
        }
        if (!lost) FindObjectOfType<ZoneManager>().area = area;
        else FindObjectOfType<ZoneManager>().area = ZoneManager.Area.AREA_CENTER;
        FindObjectOfType<ZoneManager>().SwitchAreaMusic();
    }

    public void PlayerLostBattle()
    {
        playerPosition = new Vector3(-0.375f, 8, 0);
        lost = true;
    }
}
