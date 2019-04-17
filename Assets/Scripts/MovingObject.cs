using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class MovingObject : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    // By storing the reciprocal of the move time we can use it 
    // by multiplying instead of dividing, this is more efficient.
    private float inverseMoveTime;

    public float MoveTime = 0.1f;
    public LayerMask blockingLayer;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(boxCollider);

        rb2D = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb2D);

        inverseMoveTime = 1f / MoveTime;
    }

    // Move returns true if it is able to move and false if not. 
    // Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit) 
    {
        // Store start position to move from, based on objects current transform position.
        Vector2 start = transform.position;
        // Calculate end position based on the direction parameters passed in when calling Move.
        Vector2 end = start + new Vector2 (xDir, yDir);

        // Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;
        hit = Physics2D.Linecast (start, end, blockingLayer);
        boxCollider.enabled = true;
        
        if (hit.transform == null)
        {
            // If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination.
            StartCoroutine(SmoothMovement (end));
            return true;
        }

        return false;
    }

    // AttemptMove takes a generic parameter T to specify the type of 
    // component we expect our unit to interact with if blocked.
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    // Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        // Calculate the remaining distance to move based on the square magnitude  
        // of the difference between current position and end parameter. 
        // Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // While that distance is greater than a very small amount (Epsilon, almost zero).
        while (sqrRemainingDistance > float.Epsilon)
        {
            // Find a new position proportionally closer to the end, based on the MoveTime.
            Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            // Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            // Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }

    protected abstract void OnCantMove <T> (T component)
        where T : Component;
}
