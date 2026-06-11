using System.Collections;
using UnityEngine;

// [DefaultExecutionOrder(1)]
public class note_hitbox_script : MonoBehaviour
{
    [SerializeField]
    
 
    private float distance;         // Дистанция бокскаста - это дистанция движения ноты за кадр
    // private Vector3 prevFrame;
    public BoxCollider Box_collider;
    // private bool isHitBack;
    private bool isHitFront;
    RaycastHit SingleHit;
    RaycastHit[] hitFront;
    // RaycastHit hitBack;
    private Vector3 last_frame;
    private Vector3 current_frame;
    // private Vector3 offset = new Vector3(0f, 0f, -0.15f);       // оффсет для центра
    public bool HitPlayer = false;

    Vector3 StretchedSize;
    Vector3 StretchedCenter;

    public GameObject Shards;

    public bool destroyed;

    LayerMask mask;


    void Start()
    {
        hitFront = new RaycastHit[0];
        last_frame = transform.root.position;
        mask = ~LayerMask.GetMask("Note");

        // StartCoroutine(RayCast());
        
        StartCoroutine(SingleBoxCast());
    }

    void Update()
    {
        
        if (transform.parent.position.z <= -9.7f)
        {
            Destroy(transform.root.gameObject);
        }


        foreach (RaycastHit hit in hitFront)
        {
            if (hit.collider != null)
            {
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    hit.transform.gameObject.GetComponent<Player_hitbox_script>().gotHit = true;

                    // Debug.Log("hit player");

                    HitPlayer = true;
                    Destroy(transform.root.gameObject);
                }
            }
        }


        if (isHitFront)
        {
            if (SingleHit.collider != null)
            {
                if (SingleHit.transform.gameObject.CompareTag("Player"))
                {
                    SingleHit.transform.gameObject.GetComponent<Player_hitbox_script>().gotHit = true;

                    Destroy(transform.root.gameObject);
                }
            }
        }
    }

    // void OnDrawGizmos()         // визуализация, не влияет на игру
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireCube(StretchedCenter, StretchedSize);
    // }


    IEnumerator SingleBoxCast()
    {
        current_frame = transform.root.position;
        yield return null;

        while (!destroyed)
        {
            last_frame = current_frame;
            current_frame = transform.root.position;
            distance = Vector3.Distance(last_frame, current_frame);

            isHitFront = Physics.BoxCast(Box_collider.transform.position, Box_collider.size / 2, transform.forward, out SingleHit, transform.rotation, distance, layerMask: mask);

            yield return null;
        }
    }


    IEnumerator CastBoxes()
    {
        last_frame = current_frame;
        yield return null;
        // prevFrame = transform.root.position;
        current_frame = transform.root.position;
        // yield return null;
        // maxDistance = distance;
        distance = Vector3.Distance(last_frame, current_frame);
        // Debug.Log("prevframe" + last_frame + "curFrame" + current_frame);
        // Debug.Log(distance);

        hitFront = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2, transform.forward, transform.rotation, distance);
        // isHitBack = Physics.BoxCast(Box_collider.transform.position + offset, Box_collider.size / 2, transform.forward * -1, out hitBack, transform.rotation, distance);
        // yield return null;
    }

    IEnumerator RayCast()
    {
        current_frame = transform.root.position;
        yield return null;
        while (!destroyed)
        {
            last_frame = current_frame;
            current_frame = transform.root.position;

            distance = Vector3.Distance(last_frame, current_frame);

            hitFront = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2f, transform.forward, transform.rotation, distance, layerMask: mask);

            yield return null;
        }
    }

    // IEnumerator RayCast2()
    // {
    //     current_frame = transform.root.position;
    //     yield return null;
    //     while (!destroyed)
    //     {
    //         last_frame = current_frame;
    //         current_frame = transform.root.position;

    //         distance = Vector3.Distance(last_frame, current_frame);

    //         StretchedSize = new Vector3(Box_collider.size.x, Box_collider.size.y, distance);
    //         StretchedCenter = transform.position + transform.forward * distance / 2f;

    //         hitFront = Physics.BoxCastAll(StretchedCenter, StretchedSize * 1.05f / 2f, transform.forward, transform.rotation, 0.001f, layerMask: mask);

    //         yield return null;
    //     }
    // }
}