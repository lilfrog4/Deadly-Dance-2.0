using UnityEngine;

public class CrackerNote_Script : MonoBehaviour
{
    private float PlayerZPos = -4.143f;
    public GameObject Shockwave;

    void Start()
    {
        
    }


    void Update()
    {
        if (transform.position.z <= PlayerZPos)
        {
            GameObject Wave = Instantiate(Shockwave, new Vector3(transform.position.x, transform.position.y, -4.143f - 0.3f), transform.rotation);
            Color NoteColor = transform.Find("GroundNote").GetComponent<SpriteRenderer>().color;
            Wave.transform.Find("Glow").GetComponent<SpriteRenderer>().color = new Color(NoteColor.r, NoteColor.g, NoteColor.b, 38f / 255f);
            Wave.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = NoteColor;
            Destroy(gameObject);
        }
    }
}