using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 3f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isOnLadder = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (rb == null || animator == null) return;

        if (isOnLadder)
        {
            rb.gravityScale = 0f;

            float verticalInput = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);

            // Управление параметрами аниматора
            animator.SetBool("isClimbing", true);

            if (verticalInput != 0)
            {
                animator.SetBool("isClimbingMoving", true);
                animator.speed = 1f;
            }
            else
            {
                animator.SetBool("isClimbingMoving", false);
                animator.speed = 1f;
            }
        }
        else
        {
            rb.gravityScale = 1f;
            animator.SetBool("isClimbing", false);
            animator.SetBool("isClimbingMoving", false);
            animator.speed = 1f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }
    }
}