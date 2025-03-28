using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarpTile : MonoBehaviour
{
    public Vector3 warpCoords;
    [SerializeField] Animator transitionAnim;

    IEnumerator WarpToCoords(){
            PlayerTileMovement player = FindObjectOfType<PlayerTileMovement>();
            player.movementLocked = true;
            transitionAnim.SetTrigger("EndFast");
            yield return new WaitForSeconds(1);
            player.movepoint.position = warpCoords - new Vector3(0f, 0.5f, 0);
            player.gameObject.transform.position = warpCoords;
            yield return new WaitForSeconds(0.5f);
            transitionAnim.SetTrigger("StartFast");
            yield return new WaitForSeconds(1);
            player.movementLocked = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding!");
        if (collision.collider.CompareTag("Movepoint"))
        {
            Debug.Log("Player!");
            StartCoroutine(WarpToCoords());
        }
    }
}
