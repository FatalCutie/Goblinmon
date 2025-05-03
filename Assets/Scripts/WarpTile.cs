using System.Collections;
using UnityEngine;

public class WarpTile : MonoBehaviour
{
    public Vector3 warpCoords;
    [SerializeField] Animator transitionAnim;

    public void Start()
    {
        transitionAnim = FindObjectOfType<SceneController>().transitionAnim;
    }

    IEnumerator WarpToCoords()
    {
        PlayerTileMovement player = FindObjectOfType<PlayerTileMovement>();
        player.movementLocked = true;
        transitionAnim.SetTrigger("EndFast");
        yield return new WaitForSeconds(1);
        player.movepoint.position = warpCoords - new Vector3(0f, 0.1f, 0); //this is hard coded LOL!
        player.gameObject.transform.position = warpCoords;
        yield return new WaitForSeconds(0.5f);
        transitionAnim.SetTrigger("StartFast");
        yield return new WaitForSeconds(1);
        player.movementLocked = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding!");
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player!");
            StartCoroutine(WarpToCoords());
        }
    }
}
