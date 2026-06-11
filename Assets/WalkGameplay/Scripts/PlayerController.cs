using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    private CharacterController controller;
    public static bool IsMovementBlocked = false;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        float horizontal = 0;
        
        // Движение только если не заблокировано
        if (!IsMovementBlocked)
        {
            horizontal = Input.GetAxis("Horizontal");
            Vector3 move = transform.right * horizontal;
            controller.Move(move * speed * Time.deltaTime);
        }
        
        // Анимация: если движение заблокировано или нет движения — не ходим
        bool isWalking = !IsMovementBlocked && (horizontal != 0);
        
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
        
        // Разворот спрайта (только если движение не заблокировано)
        if (!IsMovementBlocked && horizontal != 0)
        {
            spriteRenderer.flipX = horizontal < 0;
        }
    }
}