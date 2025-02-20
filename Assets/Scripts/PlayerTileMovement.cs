using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileMovement : MonoBehaviour
{
    public float movespeed = 5f;
    public Transform movepoint;
    public Animator animator;
    public bool movementLocked = true;
    Vector2 movement; //This is sloppy and a bandaid port. Fix this later
    private Vector3 currentTilePosition;
    [SerializeField] private Tilemap longGrassTilemap;
    [SerializeField] private GameObject tilemapDetector;
    public bool inGrass;

    public LayerMask movementStopperLayer;

    // Start is called before the first frame update
    void Start()
    {
        movepoint.parent = null;
        StartCoroutine(UnlockPlayerMovement()); //Unlock player movement after transition
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        PlayerMovement();
    }

    IEnumerator UnlockPlayerMovement()
    {
        FindObjectOfType<PlayerPositionManager>().RememberPlayerPosition(); //Sets player position
        yield return new WaitForSeconds(2); //This is hardcoded. Adjust this with animation speed
    }

    void PlayerMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, movepoint.position + new Vector3(0, 0.5f, 0), movespeed * Time.deltaTime);
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Vector3.Distance(transform.position, movepoint.position + new Vector3(0, 0.5f, 0)) <= 0.05f
        && !movementLocked)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                UpdateAnimator('x');
                if (!IsMovementBlocked('x'))
                {
                    movepoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);

                    //animator.SetFloat("Horizontal", )
                }

            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                UpdateAnimator('y');
                if (!IsMovementBlocked('y'))
                {
                    movepoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);

                }
            }
        }
        //TODO This plays even if player can't move. Figure it out later
        animator.SetFloat("Speed", movement.sqrMagnitude);
        //CheckTallGrass();
    }

    //This code is worthless because I am a stupid idiot who made an
    //ass out of you AND me
    void CheckTallGrass()
    {
        if (!inGrass) return;

        Vector3Int cellPosition = longGrassTilemap.WorldToCell(tilemapDetector.transform.position);
        TileBase tile = longGrassTilemap.GetTile(cellPosition);

        //If on the same tile do not do an encounter check
        if (tile == null || !tile.name.Equals("Tall Grass") || currentTilePosition == cellPosition)
            return;

        currentTilePosition = cellPosition;

        Debug.Log("I'm on a new grass tile!");
        //other.GetComponent<TallGrass>().RandomEncounter();
    }

    //Makes sure player can move to object
    //NOTE: It's way more optimal to check Used By Composite in Tilemap Collider 2D but then the collision checking doesn't work
    //I don't know if this will be relevant to performance problems later but I'm leaving the note here just in case
    bool IsMovementBlocked(char axis)
    {
        if (axis == 'x')
        {
            if (!Physics2D.OverlapCircle(movepoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .3f, movementStopperLayer))
                return false;
            else
            {
                FindObjectOfType<AudioManager>().Play("damage");
                return true;
            }
        }
        else if (axis == 'y')
        {
            if (!Physics2D.OverlapCircle(movepoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .3f, movementStopperLayer))
                return false;
            else
            {
                FindObjectOfType<AudioManager>().Play("damage");
                return true;
            }
        }
        else
        {
            Debug.LogWarning("IsMovementBlocked fed unrecognized character. Please check PlayerTileMovment.");
            return true;
        }
    }

    //Called to update player animator based on movement
    //NOTE: This shit is so ass but also quite functional
    void UpdateAnimator(char axis)
    {
        if (axis == 'x')
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", 0);
        }
        else if (axis == 'y')
        {
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Horizontal", 0);

        }
        else
        {
            Debug.LogWarning("UpdateAnimator fed unrecognized character. Please check PlayerTileMovement.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Flip inGrass bool
        if (other.CompareTag("Tall Grass"))
        {
            // Debug.Log("Entering tall grass!");
            // inGrass = true;
            other.GetComponent<TallGrass>().RandomEncounter();
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Tall Grass"))
    //     {
    //         // Debug.Log("Exiting tall grass!");
    //         // inGrass = false;
    //         // currentTilePosition = new Vector3(-100, -100, -100); //reset currentTilePosition
    //     }
    // }

}