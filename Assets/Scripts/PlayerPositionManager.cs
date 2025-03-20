using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages players position when switching scenes
public class PlayerPositionManager : MonoBehaviour
{
    [SerializeField] private Vector3 playerPosition = new Vector3();
    private PlayerPositionManager instance;

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

    public void SavePlayersPosition(){
        Transform player = FindObjectOfType<PlayerTileMovement>().movepoint;
        playerPosition = player.position;
    }

    //Places player in previous position
    //If you have problems with player breaking grid check player position and PlayerMovementManager position
    public void RememberPlayerPosition(){
        PlayerTileMovement player = FindObjectOfType<PlayerTileMovement>();
        //If player position is set and doesen't match with internal position
        if(playerPosition != new Vector3() && player.movepoint.position != playerPosition){
            player.movepoint.position = playerPosition - new Vector3(0f, 0.5f, 0); //Move movepoint first 
            player.gameObject.transform.position = playerPosition; //Then actual player

        } 
    }

    private Vector3 RoundPlayerCoordinates(){
        return new Vector3();
    }
}
