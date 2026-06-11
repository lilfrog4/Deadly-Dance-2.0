using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

// [DefaultExecutionOrder(9999)]
public class PlayerProjectileScript : MonoBehaviour
{
    private float distance;
    public BoxCollider Box_collider;
    RaycastHit[] hit;

    RaycastHit[] PrevFront;
    RaycastHit[] CurFront;
    RaycastHit[] Back;

    public PlayerProjectileValues Values;

    private Vector3 last_frame;
    private Vector3 current_frame;

    private Vector3 PlayerCoords;
    private Vector3 castOrigin;

    LayerMask mask;

    private bool destroyed = false;

    void Start()
    {
        PlayerCoords = Box_collider.transform.position;
        mask = LayerMask.GetMask("Note");

        // hit = new RaycastHit[0];
        // float P2Bdistance = Vector3.Distance(Box_collider.transform.position, PlayerCoords);

        // last_frame = transform.root.position;
        // hit = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2, transform.forward * -1, transform.rotation, P2Bdistance, layerMask: mask);
        // System.Array.Sort(hit, (hit1, hit2) => hit2.distance.CompareTo(hit1.distance));

        // StartCoroutine(RayCast2());
        // HandleCollision();
        StartCoroutine(FBRaycasts());
        StartCoroutine(DelayedDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        // StartCoroutine(CastBoxes());

        // HandleCollision();


    }

    private IEnumerator FBRaycasts()
    {
        CurFront = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2f, transform.forward, Box_collider.transform.rotation, 4f, layerMask: mask);
        yield return null;

        while (!destroyed)
        {
            PrevFront = CurFront;
            CurFront = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2f, transform.forward, Box_collider.transform.rotation, 4f, layerMask: mask);

            distance = Vector3.Distance(PlayerCoords, Box_collider.transform.position);
            // Debug.Log(distance);
            Back = Physics.BoxCastAll(PlayerCoords, Box_collider.size / 2f, transform.forward, Box_collider.transform.rotation, distance, layerMask: mask);
            System.Array.Sort(Back, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            // Debug.Log(CurFront.Length);
            if (PrevFront.Length == 0)
            {
                yield return null;
                continue;
            }

            foreach (RaycastHit backHit in Back)
            {
                if (backHit.collider != null)
                {
                    bool intersect = Array.Exists(PrevFront, element => element.transform?.gameObject == backHit.transform.gameObject);
                    if (backHit.transform.gameObject.CompareTag("Note") & intersect == true)
                    {
                        Values.HP -= 1;
                        Destroy(backHit.transform.root.gameObject);
                    }

                    if (Values.HP <= 0)
                    {
                        Destroy(transform.root.gameObject);
                    }
                }
            }

            yield return null;
        }
        
    }

    private void HandleCollision()
    {
        foreach (RaycastHit RChit in hit)           // поменять
        {
            if (RChit.collider != null)
            {
                if (RChit.transform.gameObject.CompareTag("Note"))
                {
                   
                    // if (destroyed == true)
                    // {
                    //     Destroy(transform.root.gameObject);
                    //     return;
                    // }
                    
                    if (destroyed == false & RChit.transform.GetComponent<note_hitbox_script>().destroyed == false)
                    {
                        RChit.transform.GetComponent<note_hitbox_script>().destroyed = true;
                        Values.HP -= 1;
                        Destroy(RChit.transform.root.gameObject);
                    }

                    if (Values.HP <= 0)
                    {
                        destroyed = true;

                        Destroy(transform.root.gameObject);
                        return;
                    }
                    
                }
            }
        }
    }

    IEnumerator CastBoxes()
    {
        last_frame = current_frame;
        yield return null;

        current_frame = transform.root.position;
        distance = Vector3.Distance(last_frame, current_frame);

        hit = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2, transform.forward * -1, transform.rotation, distance, layerMask: mask);

        System.Array.Sort(hit, (hit1, hit2) => hit2.distance.CompareTo(hit1.distance));

        // isHitBack = Physics.BoxCast(Box_collider.transform.position + offset, Box_collider.size / 2, transform.forward * -1, out hitBack, transform.rotation, distance);
        // yield return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(PlayerCoords, transform.forward * distance);
        Gizmos.DrawRay(Box_collider.transform.position, transform.forward * 4f);
        
        // foreach(RaycastHit RC in hit)
        // {
        //     Gizmos.DrawWireCube(Box_collider.transform.position + new Vector3(0f, 0f, RC.distance), Box_collider.size);
        // }
    }

    IEnumerator RayCast()
    {
        current_frame = Box_collider.transform.root.position;
        yield return null;
        while (!destroyed)
        {
            last_frame = current_frame;
            current_frame = Box_collider.transform.root.position;

            distance = Vector3.Distance(last_frame, current_frame);

            // distance = Vector3.Distance(last_frame, current_frame);

            hit = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2f, transform.forward, Box_collider.transform.rotation, distance, layerMask: mask);
            // Debug.Log(distance);
            System.Array.Sort(hit, (hit1, hit2) => hit2.distance.CompareTo(hit1.distance));

            yield return null;
        }
    }

    IEnumerator RayCast2()
    {
        current_frame = transform.root.position;
        yield return null;
        while (!destroyed)
        {
            last_frame = current_frame;
            current_frame = transform.root.position;

            distance = Vector3.Distance(last_frame, current_frame);

            Vector3 StretchedCenter = transform.position + transform.forward * distance / 2f;

            Box_collider.size = new Vector3(Box_collider.size.x, Box_collider.size.y, distance);
            // Box_collider.transform.position = StretchedCenter;

            // Vector3 StretchedSize = new Vector3(Box_collider.size.x, Box_collider.size.y, distance);
            

            hit = Physics.BoxCastAll(StretchedCenter, Box_collider.size / 2f, transform.forward, transform.rotation, 0.001f, layerMask: mask);

            yield return null;
        }
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(transform.root.gameObject);
    }
}