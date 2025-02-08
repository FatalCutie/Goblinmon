using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTileMovement : MonoBehaviour
{
    public float movespeed = 5f;
    public Transform movepoint;
    public Animator animator;
    Vector2 movement; //This is sloppy and a bandaid port. Fix this later

    public LayerMask movementStopperLayer;

    // Start is called before the first frame update
    void Start()
    {
        movepoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movepoint.position + new Vector3(0, 0.5f, 0), movespeed * Time.deltaTime);
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        //Movement
        if (Vector3.Distance(transform.position, movepoint.position + new Vector3(0, 0.5f, 0)) <= 0.05f)
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

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log("COLLISION!!!!");
    //     // Check if the player collides with a tile
    //     if (collision.collider.CompareTag("Tall Grass"))
    //     {
    //         // Run your script or logic here
    //         Debug.Log("Player touched the tile!");
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: This only triggers once when the player enters tall grass. Going to need
        //To add a trigger on every grass individually or find another way
        Debug.Log("COLLISION!!!!");
        if (other.CompareTag("Tall Grass"))
        {
            other.GetComponent<TallGrass>().RandomEncounter();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {

    }
}