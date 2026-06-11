using UnityEngine;

public class ExplosionHitboxScript : MonoBehaviour
{
    public BoxCollider Box_collider;
    // RaycastHit hitFront;
    private RaycastHit[] isHit;

    void Start()
    {
        CastBox();
        // isHit = Physics.BoxCast(transform.position, new Vector3(50f, 0.2f, 20f) / 2f, transform.forward * -1f, out hitFront, transform.rotation, 1f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Box_collider.transform.position, transform.forward * -2f);
        Gizmos.DrawWireCube(Box_collider.transform.position + transform.forward * -2f, Box_collider.size);

        // if (isHit)
        // {
        //     Gizmos.color = Color.red;
        // }
    }

    private void CastBox()
    {
        isHit = Physics.BoxCastAll(Box_collider.transform.position, Box_collider.size / 2f, transform.forward * -1f, transform.rotation, 2f);

        foreach (RaycastHit hit in isHit)
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                hit.transform.gameObject.GetComponent<Player_hitbox_script>().gotHit = true;
            }
        }
    }

    void Update()
    {
        
    }

    // void OnDrawGizmos()
    // {
    //     if (isHit)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawRay(transform.position, transform.forward * -1f * hitFront.distance);
    //         Gizmos.DrawWireCube(transform.position + transform.forward * -1f * hitFront.distance, Box_collider.size);
    //     }
    //     else
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawRay(transform.position, transform.forward * -1f);
    //     }
    // }
}