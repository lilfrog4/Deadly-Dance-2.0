using Unity.VisualScripting;
using UnityEngine;

public class PunchHitboxScript : MonoBehaviour
{
    public BoxCollider Box_Collider;
    public GameObject Shards;
    public float distance;
    RaycastHit hit;
    private bool hitSmth;

    LayerMask mask;

    void Start()
    {
        mask = ~LayerMask.GetMask("Player"); 
    }


    void Update()
    {
        hitSmth = Physics.BoxCast(Box_Collider.transform.position, Box_Collider.size / 2, transform.forward, out hit, transform.rotation, distance, layerMask: mask);

        if (hitSmth)
        {
            if (hit.transform.gameObject.CompareTag("Note"))
            {
                GameObject NoteShards = Instantiate(Shards, new Vector3(hit.transform.position.x, hit.transform.position.y + 1f, hit.transform.position.z), transform.rotation);
                NoteShards.transform.root.GetComponent<SpriteRenderer>().color = hit.transform.root.GetComponentInChildren<SpriteRenderer>().color;
                Destroy(hit.transform.root.gameObject);
                gameObject.SetActive(false);
                Debug.Log("hit!");
            }
        }
    }

    // void OnDrawGizmos()         // визуализация, не влияет на игру
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(transform.position, transform.forward * distance);
    //     // Gizmos.DrawWireCube(transform.position + transform.forward * 0.1f, Box_Collider.size);

    //     if (hitSmth)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
    //         // Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, Box_Collider.size);
    //     }
    // }
    }
