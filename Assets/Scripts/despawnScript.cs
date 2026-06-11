using UnityEngine;

public class despawnScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
        {
            Destroy(collision.transform.parent.gameObject);
        }
}
