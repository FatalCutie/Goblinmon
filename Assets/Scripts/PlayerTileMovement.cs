using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileMovement : MonoBehaviour
{
    public float movespeed = 5f;
    public Transform movepoint;
    public Animator animator;
    public OverworldUI overworldUI;
    public bool movementLocked = true;
    Vector2 movement; //This is sloppy and a bandaid port. Fix this later
    private Vector3 currentTilePosition;
    [SerializeField] private Tilemap longGrassTilemap;
    [SerializeField] private GameObject tilemapDetector;
    public bool inGrass;
    private Vector2 lastPosition;
    public float idleTimer = 0f;
    public float idleThreshold = 3f;
    public bool playerIsIdle = false;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 currentPosition;

    public LayerMask movementStopperLayer;

    void Start()
    {
        movepoint.parent = null;
        currentPosition = transform.position;
        StartCoroutine(UnlockPlayerMovement()); //Unlock player movement after transition
    }

    void Update()
    {
        PlayerMovement();
        if (!movementLocked) CheckIfPlayerIsIdle();
        currentPosition = transform.position; //this sucks
        direction = currentPosition - (movepoint.position + new Vector3(0, 0.25f, 0));
        UpdateAnimator(direction);
    }

    void PlayerMovement()
    {
        //if (!movementLocked) 
        transform.position = Vector3.MoveTowards(transform.position, movepoint.position + new Vector3(0, 0.25f, 0), movespeed * Time.deltaTime); // + new Vector3(0, 0.5f, 0)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Vector3.Distance(transform.position, movepoint.position + new Vector3(0, 0.25f, 0)) <= 0.05f //+ new Vector3(0, 0.5f, 0)
        && !movementLocked)
        {

            //This only moves the player when a button is getting pressed down.
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (!IsMovementBlocked('x'))
                {
                    movepoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f);

                    //animator.SetFloat("Horizontal", )
                }

            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (!IsMovementBlocked('y'))
                {
                    movepoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);

                }
            }
        }
        // if (!movementLocked) animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void CheckIfPlayerIsIdle()
    {
        if (!playerIsIdle)
        {
            // Check for movement in the 2D plane (x and y positions)
            if (Vector2.Distance(transform.position, lastPosition) > 0.1f)
            {
                // Player is moving, reset the idle timer
                idleTimer = 0f;
                lastPosition = transform.position;
            }
            else
            {
                // Player isn't moving, increase idle time
                idleTimer += Time.deltaTime;
            }

            // If idle time exceeds threshold, trigger UI
            if (idleTimer >= idleThreshold)
            {
                overworldUI.OpenItemMenuOnIdle();
                playerIsIdle = true;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, lastPosition) > 0.1f)
            {
                // Player is moving, reset the idle timer
                idleTimer = 0f;
                lastPosition = transform.position;
                overworldUI.CloseItemMenuOnIdle();
                playerIsIdle = false;
            }
        }
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
                idleTimer = 0f;
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
                idleTimer = 0f;
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
    // void UpdateAnimator(char axis)
    // {
    //     if (axis == 'x')
    //     {
    //         animator.SetFloat("Horizontal", movement.x);
    //         animator.SetFloat("Vertical", 0);
    //     }
    //     else if (axis == 'y')
    //     {
    //         animator.SetFloat("Vertical", movement.y);
    //         animator.SetFloat("Horizontal", 0);

    //     }
    //     else
    //     {
    //         Debug.LogWarning("UpdateAnimator fed unrecognized character. Please check PlayerTileMovement.");
    //     }
    // }
    void UpdateAnimator(Vector2 dir)
    {
        animator.SetFloat("Horizontal", -dir.x);
        animator.SetFloat("Vertical", -dir.y);
        animator.SetFloat("Speed", dir.sqrMagnitude); // For blend tree or idle detection
        // If the player is moving, update last direction
        if (dir.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("LastHorizontal", dir.x);
            animator.SetFloat("LastVertical", dir.y);
        }
    }

    IEnumerator UnlockPlayerMovement()
    {
        FindObjectOfType<PlayerPositionManager>().RememberPlayerPosition(); //Sets player position
        yield return new WaitForSeconds(2); //This is hardcoded. Adjust this with animation speed
    }

    //This only seems to trigger half the time but I can't figure out how to make it trigger all the time.
    //I wanted to move this to TallGrass but it only works on the player. Oh Well!
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Flip inGrass bool
        if (other.CompareTag("Tall Grass"))
        {
            Debug.Log("Entering tall grass!");
            // inGrass = true;
            other.GetComponent<TallGrass>().RandomEncounter();
        }
    }

}