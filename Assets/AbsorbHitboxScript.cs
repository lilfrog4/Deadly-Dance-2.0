using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

// [DefaultExecutionOrder(9999)]
public class AbsorbHitboxScript : MonoBehaviour
{
    public GameObject ControlsScriptObject;
    public BoxCollider Box_Collider;
    public GameObject Shards;
    public float distance;
    public int HoldColor;
    public bool AwaitingHoldColor = true;

    public int ChargeCap;

    public GameObject ChargeText;
    private TMP_Text ChargeString;

    CharactersValues CV;

    LayerMask mask;

    RaycastHit[] hit;

    // private List<GameObject> NotesToDelete;


    void Start()
    {
        ChargeString = ChargeText.GetComponent<TMP_Text>();

        mask = ~LayerMask.GetMask("Player");

        CV = transform.parent.GetComponent<CharactersValues>();
    }

    void Update()
    {

        hit = Physics.BoxCastAll(Box_Collider.transform.position, Box_Collider.size / 2f, transform.forward, transform.rotation, distance, layerMask: mask);

        if (hit.Length == 0)
        {
            return;
        }

        System.Array.Sort(hit, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        foreach (RaycastHit RChit in hit)
        {
            if (RChit.transform.gameObject.CompareTag("Note"))
            {
                if (AwaitingHoldColor == true)
                {
                    // Debug.Log(hit[0].transform.gameObject.tag);
                    
                    if (HoldColor != hit[0].transform.parent.gameObject.GetComponent<NoteValues>().NoteColor)
                    {
                        CV.Charge = 0;
                    }
                    HoldColor = hit[0].transform.parent.gameObject.GetComponent<NoteValues>().NoteColor;

                    AwaitingHoldColor = false;
                }
                
                ChargeString.color = new Color((int)(HoldColor / 1000000f) / 255f, (HoldColor % 1000000 - HoldColor % 1000) / 255000f, (int)(HoldColor % 1000f) / 255f);
                // Debug.Log((HoldColor % 1000000 - HoldColor % 1000) / 255000f);
                
                if (RChit.transform.parent.gameObject.GetComponent<NoteValues>().Absorbable == true & RChit.transform.parent.gameObject.GetComponent<NoteValues>().NoteColor == HoldColor)
                {
                    float randomZangle = UnityEngine.Random.Range(0f, 360f);
                    Quaternion ZRot = Quaternion.Euler(0f, 0f, randomZangle);
                    GameObject NoteShards = Instantiate(Shards, new Vector3(RChit.transform.position.x, RChit.transform.position.y + 1f, RChit.transform.position.z), ZRot);
                    NoteShards.transform.root.GetComponent<SpriteRenderer>().color = RChit.transform.root.GetComponentInChildren<SpriteRenderer>().color;

                    
                    CV.Charge += 1;

                    if (CV.Charge > ChargeCap)
                    {
                        CV.Charge = ChargeCap;
                    }

                    ChargeString.text = CV.Charge.ToString();
                    
                    CV.ChargeColor = HoldColor;


                    Destroy(RChit.transform.root.gameObject);

                    ControlsScriptObject.GetComponent<characterControls>().AbsorbTimer = 0f;
                    // Debug.Log("absorb!");
                }
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawRay(Box_Collider.transform.position, transform.forward * distance);
    //     // Gizmos.DrawWireCube(transform.position + transform.forward * 0.1f, Box_Collider.size);
    // }
}