using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    
    public static bool IsMovementBlocked = false;
    
    private float horizontal;
    
    private void Start()
    {
        // Получаем компоненты, если не назначены в инспекторе
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        // Получаем ввод в Update
        if (!IsMovementBlocked)
        {
            horizontal = Input.GetAxis("Horizontal");
        }
        else
        {
            horizontal = 0;
        }
        
        // Обновляем анимацию и поворот спрайта
        bool isWalking = !IsMovementBlocked && (horizontal != 0);
        
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
        
        if (!IsMovementBlocked && horizontal != 0)
        {
            spriteRenderer.flipX = horizontal < 0;
        }
    }
    
    private void FixedUpdate()
    {
        // Движение через физику в FixedUpdate
        if (!IsMovementBlocked)
        {
            Vector2 move = new Vector2(horizontal * speed * Time.fixedDeltaTime, rb.linearVelocity.y);
            rb.linearVelocity = new Vector2(move.x, rb.linearVelocity.y);
        }
        else
        {
            // Останавливаем движение по X при блокировке
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}