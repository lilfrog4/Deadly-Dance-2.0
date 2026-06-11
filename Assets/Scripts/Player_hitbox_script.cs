using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using TMPro;


public class Player_hitbox_script : MonoBehaviour
{
    public MusicManager mg;
    public bool gotHit;
    public int player_health;
    private List<int> hit_info = new List<int>();
    private bool isInvulnerable;
    [Header("Не трогать")]
    public float invulnerableTime;

    public GameObject ParentObject;
    private Coroutine FlashCoroutine;
    public GameOverScreen GameOverScreen;
    public WinnerScreenScript WinnerScreenScript;

    public GameObject HPtext;
    private TMP_Text HPstring;

    public GameObject Chargetext;
    private TMP_Text ChargeString;
    characterControls CC;
 

    void Start()
    {
        CC = ParentObject.GetComponent<characterControls>();

        HPstring = HPtext.GetComponent<TMP_Text>();
        HPstring.text = player_health.ToString();

        ChargeString = Chargetext.GetComponent<TMP_Text>();
        //player_health = 2;
        
        
        // Debug.Log(player_health);
        
    }

    void Update()
    {
        GetDamage(); 
        
    }
    private void GetDamage()
    {
        if (gotHit)
        {
            gotHit = false;
            CC.AbsorbTimer = CC.AbsorbTime;

            if (!isInvulnerable)
            {
                StartCoroutine(setInvulnerability(invulnerableTime));

                if (player_health > 1) 
                { 
                    mg.damageSound.Play();
                    player_health = player_health - 1;

                    HPstring.text = player_health.ToString();
                    ChargeString.text = "0";
                    ChargeString.color = Color.white;
                    transform.GetComponent<CharactersValues>().Charge = 0;
                }

                else
                {
                    mg.damageSound.Play();
                    player_health = 0;
                    Destroy(ParentObject);
                    GameOverScreen.SetUpTrue();
                    mg.mainSongForThisScene.Stop();
                    mg.loseSound.Play();
                    Time.timeScale = 0;
                    
                }

                // Debug.Log(player_health);

                if (FlashCoroutine == null)
                    FlashCoroutine = StartCoroutine(Sprite_Damage_Flash());
                else
                {
                    StopCoroutine(FlashCoroutine);
                    FlashCoroutine = StartCoroutine(Sprite_Damage_Flash());
                }
            }

        }
    }
    private IEnumerator setInvulnerability(float time)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
    } // дать персонажу неуязвимость на (время)


    IEnumerator Sprite_Damage_Flash()
    {
        int t = 0;
        while(t != (5 * invulnerableTime)) //т +=1 каждые 0.21 сек. Цикл выполнится 5 раз = пройдет примерно 1 секунда()
        {
            ParentObject.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
            yield return new WaitForSeconds(0.07f);
            ParentObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            yield return new WaitForSeconds(0.07f);
            ParentObject.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
            yield return new WaitForSeconds(0.07f);
            ParentObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255); // 0.07 * 3 = 0.21
            t += 1;
        }
    }
}