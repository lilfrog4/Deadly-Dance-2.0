using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class characterControls : MonoBehaviour
{
    // private SpriteRenderer spriteRenderer;
    public Animator Char_Animator;
    
    public GameObject PlayerHitbox;
    public GameObject PunchHitbox;
    public GameObject AbsorbHitbox;
    public GameObject SmallAttack;
    public GameObject MediumAttack;
    public GameObject HeavyAttack;
    private float Hitbox_Grounded_Y;

    public Transform chel;
    public GameObject PunchArc;
    public GameObject AbsorbVisual;
    public float PunchDuration;
    public float AbsorbTimer;
    public float AbsorbTime;
    private bool AbsorbButtonUp = true;
    public float ArmCooldown;
    private bool ArmActionAvailable = true;
    public float duration = 0.1f;           // Длительность движения после телепорта (от края ряда до центра)
    public float GoUpDuration = 0.2f;
    public float GoDownDuration = 0.2f;
    public float FloatDuration = 0.1f;
    public float JumpHeight = 0.9f;
    // public bool chelMoving = false;
    private bool movingLeft = false;
    private bool movingRight = false;
    private bool falling = false;
    private bool spriteMovingLeft = false;
    private bool spriteMovingRight = false;
    private bool SpriteJumping = false;
    private bool grounded = true;
    private bool JRollExpended = false;
    private bool JumpBuffer = false;

    private bool A_up = true;
    private bool D_up = true;
    private bool Punch_up = true;

    public int currentlane = 2;
    public int nextlane;

    private CharactersValues CharValues;
    public GameObject ChargeText;

    Coroutine myCoroutine;

    private List<float> laneList = new List<float>() {-4f, -2f, 0f, 2f, 4f};            // Координаты рядов

    void Start()
    {
        CharValues = PlayerHitbox.GetComponent<CharactersValues>();

        Hitbox_Grounded_Y = PlayerHitbox.transform.position.y;
        PunchHitbox.SetActive(false);
        AbsorbHitbox.SetActive(false);
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 30;
        // Time.timeScale = 0.5f;
    }

    void Update()
    {
        StartCoroutine(frameCompare());

        if (spriteMovingLeft)
        {
            // spriteRenderer.flipX = false;
            // spriteRenderer.sprite = MoveSprite;
            Char_Animator.SetBool("MovingLeft", true);
            Char_Animator.SetBool("MovingRight", false);
            // Char_Animator.SetBool("Jumping", false);
        }
        else if (spriteMovingRight)
        {
            // spriteRenderer.flipX = true;
            // spriteRenderer.sprite = MoveSprite;
            Char_Animator.SetBool("MovingLeft", false);
            Char_Animator.SetBool("MovingRight", true);
            // Char_Animator.SetBool("Jumping", false);
        }
        else if (grounded)
        {
            // spriteRenderer.flipX = false; 
            // spriteRenderer.sprite = IdleSprite;
            Char_Animator.SetBool("MovingLeft", false);
            Char_Animator.SetBool("MovingRight", false);
            Char_Animator.SetBool("Jumping", false);
        }

        if (!spriteMovingLeft & !spriteMovingRight & !grounded)
        {
            Char_Animator.SetBool("Jumping", true);
            Char_Animator.SetBool("MovingLeft", false);
            Char_Animator.SetBool("MovingRight", false);
        }

        if (chel.transform.position.y == -3)
        {
            grounded = true;
            falling = false;
            JRollExpended = false;
            Char_Animator.SetBool("Jumping", false);
        }

        if (falling)
        {
            Char_Animator.SetBool("Falling", true);
        }
        else
        {
            Char_Animator.SetBool("Falling", false);
        }

        if (Input.GetKey(KeyCode.A) & (chel.transform.position.x >= -2f) & !JRollExpended & A_up)
        {
            spriteMovingLeft = true;
            spriteMovingRight = false;
            A_up = false;
            if (myCoroutine != null)            // Останавливает движение к центру если оно в процессе, перс тпшится до центра текущего ряда, а потом до полпути на следующий
            {
                StopCoroutine(myCoroutine);
            }
            
            nextlane = currentlane - 1;
            
            myCoroutine = StartCoroutine(LerpMove(new Vector3(laneList[currentlane], chel.transform.position.y, chel.transform.position.z), new Vector3(laneList[nextlane], chel.transform.position.y, chel.transform.position.z)));
            PlayerHitbox.transform.position = new Vector3(laneList[nextlane], PlayerHitbox.transform.position.y, PlayerHitbox.transform.position.z);
            currentlane = nextlane;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            A_up = true;
        }

        if (Input.GetKey(KeyCode.D) & (chel.transform.position.x <= 2f) & !JRollExpended & D_up)
        {
            spriteMovingRight = true;
            spriteMovingLeft = false;

            D_up = false;
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
            }
            
            nextlane = currentlane + 1;
    
            myCoroutine = StartCoroutine(LerpMove(new Vector3(laneList[currentlane], chel.transform.position.y, chel.transform.position.z), new Vector3(laneList[nextlane], chel.transform.position.y, chel.transform.position.z)));
            PlayerHitbox.transform.position = new Vector3(laneList[nextlane], PlayerHitbox.transform.position.y, PlayerHitbox.transform.position.z);
            currentlane = nextlane;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            D_up = true;
        }


                                                        // ПРЫЖОК
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) & grounded)
        {
            JumpBuffer = false;
            SpriteJumping = true;
            StartCoroutine(Jump());
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) & falling)
        {
            JumpBuffer = true;
        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) & grounded & JumpBuffer)
        {
            JumpBuffer = false;
            SpriteJumping = true;
            StartCoroutine(Jump());
        }

        if (Input.GetKey(KeyCode.LeftArrow) & grounded & Punch_up & ArmActionAvailable)
        {
            Punch_up = false; 
            StartCoroutine(Punch());
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Punch_up = true;
        }

        if (Input.GetKey(KeyCode.DownArrow) & AbsorbButtonUp & ArmActionAvailable & grounded)
        {
            AbsorbButtonUp = false;
            StartCoroutine(Absorb());
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            StopCoroutine(Absorb());
            AbsorbHitbox.SetActive(false);
            AbsorbButtonUp = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Attack();
        }
    }

    
    IEnumerator LerpMove(Vector3 start, Vector3 end)            // Функция движения. Перс тпшится посередине двух линий, затем плавно выравнивается по центру следующей
    {
        Char_Animator.SetBool("Punching", false);
        if (!grounded)
        {
            JRollExpended = true;
        }

        Vector3 warpPos = new Vector3((start.x + end.x) / 2f, chel.transform.position.y, chel.transform.position.z);
        chel.transform.position = warpPos;          // (Телепорт)
        
        float te = 0;
        while (te < duration)           // Плавное движение в течение duration
        {
            float t = Time.deltaTime / duration;
            // chel.transform.position += Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            chel.transform.position += new Vector3((end.x - warpPos.x) * t, 0f, 0f);
            te += Time.deltaTime;
            yield return null;
        }
        chel.transform.position = new Vector3(end.x, chel.transform.position.y, chel.transform.position.z);
        spriteMovingLeft = false;
        spriteMovingRight = false;
    }

    IEnumerator GoUp(float height)
    {
        float te = 0;
        while (te < GoUpDuration)
        {
            float t = Time.deltaTime / GoUpDuration;
            // chel.transform.position += Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            chel.transform.position += new Vector3(0f, height * t, 0f);
            PlayerHitbox.transform.position += new Vector3(0f, height * t, 0f);
            te += Time.deltaTime;
            yield return null;
        }
        chel.transform.position = new Vector3(chel.transform.position.x, -3 + height, chel.transform.position.z);
    }

    IEnumerator GoDown(float fallDistance)
    {
        float te = 0;
        while (te < GoDownDuration)
        {
            float t = Time.deltaTime / GoDownDuration;
            // chel.transform.position += Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            chel.transform.position += new Vector3(0f, fallDistance * t * -1, 0f);
            PlayerHitbox.transform.position += new Vector3(0f, fallDistance * t * -1, 0f);
            te += Time.deltaTime;
            yield return null;
        }
        chel.transform.position = new Vector3(chel.transform.position.x, -3f, chel.transform.position.z);
        PlayerHitbox.transform.position = new Vector3(PlayerHitbox.transform.position.x, Hitbox_Grounded_Y, PlayerHitbox.transform.position.z);
    }

    IEnumerator Jump()
    {
        grounded = false;
        StartCoroutine(GoUp(JumpHeight));
        yield return new WaitForSeconds(GoUpDuration + FloatDuration);
        float fallDistance = 3 - Mathf.Abs(chel.transform.position.y);
        
        falling = true;
        StartCoroutine(GoDown(fallDistance));
        yield return new WaitForSeconds(GoDownDuration);
        SpriteJumping = false;
    }

    IEnumerator Punch()
    {
        if (!ArmActionAvailable)
        {
            yield break;
        }
        // if (Char_Animator.GetBool("Punching"))
        // {
        //     Char_Animator.Play("Punching", -1);
        // }

        // Debug.Log("punch!");
        Char_Animator.SetBool("Punching", true);

        StartCoroutine(StartArmCooldown());

        Instantiate(PunchArc, new Vector3(PlayerHitbox.transform.position.x, transform.position.y + 0.52f, PlayerHitbox.transform.position.z + 0.2f), transform.rotation);

        PunchHitbox.SetActive(true);
        yield return new WaitForSeconds(PunchDuration);
        PunchHitbox.SetActive(false);
        StartCoroutine(DelayedPunchAnimationOff());
    }

    public IEnumerator Absorb()
    {
        AbsorbHitbox.SetActive(true);
        StartCoroutine(StartAbsorbTimer());
        StartCoroutine(StartArmCooldown());

        Char_Animator.SetBool("Absorbing", true);

        GameObject AbsorbtionVisual = Instantiate(AbsorbVisual, new Vector3(transform.position.x, transform.position.y + 0.52f, transform.position.z + 0.2f), transform.rotation);
        AbsorbtionVisual.transform.SetParent(transform);

        yield return new WaitUntil(() => AbsorbTimer >= AbsorbTime);

        AbsorbHitbox.SetActive(false);
        AbsorbHitbox.GetComponent<AbsorbHitboxScript>().AwaitingHoldColor = true;
        Destroy(AbsorbtionVisual);

        Char_Animator.SetBool("Absorbing", false);
    }

    IEnumerator StartAbsorbTimer()
    {
        AbsorbTimer = 0f;

        while (AbsorbTimer < AbsorbTime)
        {
            AbsorbTimer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator DelayedPunchAnimationOff()
    {
        // if (Char_Animator.GetBool("Punching"))
        // {
        //     yield break;
        // }
        float timer = 0f;
        while (timer < 0.1f - PunchDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Char_Animator.SetBool("Punching", false);
    }

    IEnumerator StartArmCooldown()
    {
        ArmActionAvailable = false;
        float timer = 0f;
        while (timer < ArmCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        ArmActionAvailable = true;
    }

    public void Attack()
    {
        if (CharValues.Charge >= 3 && CharValues.Charge < 7)
        {
            GameObject Attack = Instantiate(SmallAttack, new Vector3(PlayerHitbox.transform.position.x, PlayerHitbox.transform.position.y, PlayerHitbox.transform.position.z + 0.3f), transform.rotation);
            Attack.transform.Find("Collider").GetComponent<PlayerProjectileValues>().Damage = CharValues.Charge * 5;
            // Attack.transform.Find("Collider").GetComponent<PlayerProjectileScript>().PlayerCoords = PlayerHitbox.transform.position;
            CharValues.Charge = 0;
            ChargeText.GetComponent<TMP_Text>().text = "0";
            ChargeText.GetComponent<TMP_Text>().color = Color.white;
        }
        else if (CharValues.Charge >= 7 && CharValues.Charge < 15)
        {
            GameObject Attack = Instantiate(MediumAttack, new Vector3(PlayerHitbox.transform.position.x, PlayerHitbox.transform.position.y, PlayerHitbox.transform.position.z + 0.3f), transform.rotation);
            Attack.transform.Find("Collider").GetComponent<PlayerProjectileValues>().Damage = CharValues.Charge * 7;
            // Attack.transform.Find("Collider").GetComponent<PlayerProjectileScript>().PlayerCoords = PlayerHitbox.transform.position;
            CharValues.Charge = 0;
            ChargeText.GetComponent<TMP_Text>().text = "0";
            ChargeText.GetComponent<TMP_Text>().color = Color.white;
        }
        else if (CharValues.Charge >= 15)
        {
            GameObject Attack = Instantiate(HeavyAttack, new Vector3(PlayerHitbox.transform.position.x, PlayerHitbox.transform.position.y, PlayerHitbox.transform.position.z + 0.3f), transform.rotation);
            Attack.transform.Find("Collider").GetComponent<PlayerProjectileValues>().Damage = CharValues.Charge * 10;
            // Attack.transform.Find("Collider").GetComponent<PlayerProjectileScript>().PlayerCoords = PlayerHitbox.transform.position;
            CharValues.Charge = 0;
            ChargeText.GetComponent<TMP_Text>().text = "0";
            ChargeText.GetComponent<TMP_Text>().color = Color.white;
        }
    }

    IEnumerator frameCompare()         // Сравнение позиции перса каждый кадр, определяет, стоит он или движется, и куда. Тут же будет обработка для прыжков, джампролла и тд. Нужно для спрайтов
    {
        float lastframeX = chel.transform.position.x;
        float lastframeY = chel.transform.position.y;
        yield return null;
        if (chel.transform.position.x == lastframeX)
        {
            // chelMoving = false;
            movingLeft = false;
            movingRight = false;
        }
        else if (chel.position.x < lastframeX)
        {
            // chelMoving = true;
            movingLeft = true;
            movingRight = false;
        }
        else if (chel.position.x > lastframeX)
        {
            // chelMoving = true;
            movingLeft = false;
            movingRight = true;
        }
        yield return null;
    }
}