using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public int ShotSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * -1 * ShotSpeed * 0.6f * Time.deltaTime, Space.World);
    }
}