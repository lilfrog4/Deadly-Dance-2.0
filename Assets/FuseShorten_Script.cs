using UnityEngine;

public class FuseShorten_Script : MonoBehaviour
{
    private float InitialZPos;
    private float FinalZPos;
    private float PathLength;
    private float Traveled;
    public Sprite FuseSprite1;
    public Sprite FuseSprite2;
    private SpriteRenderer FuseSR;
    void Start()
    {
        InitialZPos = transform.position.z;
        FinalZPos = -4.143f;
        PathLength = Mathf.Abs(InitialZPos - FinalZPos);

        FuseSR = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        Traveled = Mathf.Abs(InitialZPos - transform.position.z);

        if (Traveled < PathLength * 2f / 3f & Traveled > PathLength / 3f)
        {
            FuseSR.sprite = FuseSprite1;
        }
        else if (Traveled > PathLength / 3f)
        {
            FuseSR.sprite = FuseSprite2;
        }

        // Rect ImageSize = new Rect(0f, 0f, 900f, 900f * (1f - Traveled / PathLength));
        // CroppedSprite = Sprite.Create(FuseSprite.texture, ImageSize, new Vector2(0.5f, 0f), 100f);       // эта паскуда превращает игру в слайдшоу
        // FuseSR.sprite = CroppedSprite;
    }
}