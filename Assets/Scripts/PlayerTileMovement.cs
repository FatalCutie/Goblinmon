using System.Collections;
using UnityEngine;

public class PlayerTileMovement : MonoBehaviour
{
    public float movespeed = 5f;
    public Transform movepoint;
    public Animator animator;
    public OverworldUI overworldUI;
    public bool movementLocked = true;
    Vector2 movement; //This is sloppy and a bandaid port. Fix this later
    private Vector3 currentTilePosition;
    // [SerializeField] private Tilemap longGrassTilemap;
    // [SerializeField] private GameObject tilemapDetector;
    public bool inGrass;
    private Vector2 lastPosition;
    public float idleTimer = 0f;
    public float idleThreshold = 3f;
    public bool playerIsIdle = false;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private float moveMultiplier = 1;
    private bool flipper = true;
    public int catchWalkRecalls = 4;
    int catchWalkRecallsBaseline;
    [SerializeField] bool cheatsEnabled;

    public LayerMask movementStopperLayer;
    public bool canWildEncounter = false; //enabled in UnlockPlayerMovement
    public DialogueSO endText;
    public NPC endingNPC; //I am ashamed at how utterly horrible this is but crunch is crunch
    public bool idleTimerPaused = false;

    void Start()
    {
        movepoint.parent = null;
        currentPosition = transform.position;
        StartCoroutine(UnlockPlayerMovement()); //Unlock player movement after transition
        catchWalkRecallsBaseline = catchWalkRecalls;
    }

    void Update()
    {
        PlayerMovement();
        if (!movementLocked) CheckIfPlayerIsIdle();
        currentPosition = transform.position; //this sucks
        direction = currentPosition - (movepoint.position + new Vector3(0, 0.10f, 0));
        UpdateAnimator(direction);
        if (cheatsEnabled)
        {
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                if (canWildEncounter)
                {
                    FindObjectOfType<AudioManager>().Play("press");
                    canWildEncounter = !canWildEncounter;
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("damage");
                    canWildEncounter = !canWildEncounter;
                }
            }
            if (Input.GetKeyDown(KeyCode.H)) FindObjectOfType<NPC>().HealPlayerUnits();
        }
    }

    void PlayerMovement()
    {
        // Get raw input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Update animation based on input every frame (even if not moving)
        UpdateAnimator(-movement);

        // Move towards the movepoint (offset slightly upward, e.g., for visual layering)
        transform.position = Vector3.MoveTowards(
            transform.position,
            movepoint.position + new Vector3(0, 0.10f, 0),
            movespeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movepoint.position + new Vector3(0, 0.10f, 0)) <= 0.05f
            && !movementLocked)
        {
            // Handle horizontal input
            if (Mathf.Abs(movement.x) == 1f)
            {
                if (!IsMovementBlocked('x'))
                {
                    movepoint.position += new Vector3(movement.x, 0f, 0f) * moveMultiplier;
                }
            }
            // Handle vertical input
            else if (Mathf.Abs(movement.y) == 1f)
            {
                if (!IsMovementBlocked('y'))
                {
                    movepoint.position += new Vector3(0f, movement.y, 0f) * moveMultiplier;
                }
            }
        }
    }

    void CheckIfPlayerIsIdle()
    {
        if (!playerIsIdle && !idleTimerPaused)
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
                // Player is moving or interacting, reset the idle timer
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
            if (!Physics2D.OverlapCircle(movepoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .05f, movementStopperLayer))
            {
                //Remains of a system to make sure movepoint is in grass, so player doesn't get in battle while leaving grass
                // Collider2D collider = Physics2D.OverlapCircle(movepoint.position + new Vector3(Input.GetAxisRaw("Horizontal") * 0.75f, 0f, 0f), .05f);
                // if (collider != null && collider.CompareTag("Tall Grass"))
                // {
                //     Debug.Log("Tall Grass!");
                //     // Do something if the object with the specific tag is at that position
                // }
                return false;
            }
            else
            {
                FindObjectOfType<AudioManager>().Play("damage");
                animator.SetFloat("Horizontal", Input.GetAxisRaw("Horizontal")); //this doesn't update idle
                idleTimer = 0f;
                return true;
            }
        }
        else if (axis == 'y')
        {
            if (!Physics2D.OverlapCircle(movepoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .05f, movementStopperLayer))
                return false;
            else
            {
                FindObjectOfType<AudioManager>().Play("damage");
                animator.SetFloat("Horizontal", Input.GetAxisRaw("Vertical")); //this doesn't update idle
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
        FindObjectOfType<PlayerPositionManager>().CheckIfLoser();
        if (!NPCTracker.IsDefeated("boss")) //hardcoded NPC id
        {
            yield return new WaitForSeconds(2); //This is hardcoded. Adjust this with animation speed
            canWildEncounter = true;
            if (!FindObjectOfType<PlayerPositionManager>().lost) movementLocked = false;

        }
        else //If final boss beaten
        {
            StartCoroutine(FindObjectOfType<DialogueManager>().ScrollText(endText, endingNPC));
        }

    }

    //This only seems to trigger half the time but I can't figure out how to make it trigger all the time.
    //I wanted to move this to TallGrass but it only works on the player. Oh Well!
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Flip inGrass bool
        if (other.CompareTag("Movepoint") || other.CompareTag("Untagged")) return;
        //Debug.Log(other.tag);
        if (other.CompareTag("Tall Grass"))
        {
            // Debug.Log("Entering tall grass!");
            // inGrass = true;
            FindObjectOfType<AudioManager>().Play("grass");
            other.GetComponent<TallGrass>().RandomEncounter();
        }
        else if (other.CompareTag("Cave Floor"))
        {
            FindObjectOfType<AudioManager>().Play("cave"); //TODO: Change to cave walk sound
            other.GetComponent<TallGrass>().RandomEncounter();
        }
    }

    //For an unknown reason FlipWalkingAnimation is called a certain number of times
    //based on the animation speed because it runs on EVERY SINGLE BRANCH OF THE ANIMATION TREE. 
    //I have tried to diagnose the bug for literal HOURS and am tired of thinking about it
    //so catchWalkRecalls is the solution. Remember to adjust it when you adjust movespeed.
    public void FlipWalkingAnimation()
    {
        if (catchWalkRecalls <= 0)
        {
            // Debug.Log($"The bool is: {animator.GetBool("Foot")}");
            // Debug.Log($"Flipper is {flipper}");
            if (flipper)
            {
                flipper = false;
                // animator.SetTrigger("LeftFoot");
                animator.SetBool("Foot", false);
                catchWalkRecalls = catchWalkRecallsBaseline;
            }
            else
            {
                flipper = true;
                // animator.SetTrigger("RightFoot");
                animator.SetBool("Foot", true);
                catchWalkRecalls = catchWalkRecallsBaseline;
            }
        }
        else catchWalkRecalls--;
        // AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        // foreach (AnimatorClipInfo i in clips)
        // {
        //     Debug.Log(i.clip.name);
        // }

    }
}