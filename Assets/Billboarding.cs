using UnityEngine;

public class Billboarding : MonoBehaviour
{
    void FixedUpdate()
    {
        Vector3 target = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        // target.y = transform.position.y;
        transform.LookAt(target);
        // transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);
    }
}