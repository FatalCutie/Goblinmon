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
        transform.position = Vector3.MoveTowards(transform.position, movepoint.position, movespeed * Time.deltaTime);
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        //Movement
        if (Vector3.Distance(transform.position, movepoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (!IsMovementBlocked('x'))
                {
                    movepoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    UpdateAnimator('x');
                    //animator.SetFloat("Horizontal", )
                }

            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (!IsMovementBlocked('y'))
                {
                    movepoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    UpdateAnimator('y');
                }
            }


            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    //Makes sure player can move to object
    bool IsMovementBlocked(char axis)
    {
        if (axis == 'x')
        {
            if (Physics2D.OverlapCircle(movepoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 2f, movementStopperLayer))
                return true;
        }
        else if (axis == 'y')
        {
            if (Physics2D.OverlapCircle(movepoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 2f, movementStopperLayer))
                return true;
        }
        else
        {
            Debug.LogWarning("IsMovementBlocked fed unrecognized character. Please check PlayerTileMovment.");
            return false;
        }

        //TODO: Play bump audio
        return false;
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
}
