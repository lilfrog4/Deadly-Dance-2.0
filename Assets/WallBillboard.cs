using System.Collections;
using UnityEngine;

public class WallBillboard : MonoBehaviour
{
    private Vector3 StartCoords;
    private Quaternion StartRotation;
    IEnumerator RotateToCamera()
    {
        float length = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        float height = Mathf.Abs(transform.position.y - Camera.main.transform.position.y);
        float angle = Mathf.Atan(height / length) * 100f;

        // Debug.Log(angle);

        float PrevZ = transform.position.z;
        float CurZ = transform.position.z;

        while (transform.position.z > Camera.main.transform.position.z)
        {
            Vector3 WallPivot = transform.Find("Pivot").transform.position;
            // float PathTraveled = Mathf.Abs(transform.position.z - StartCoords.z);
            float PathTraveled = Mathf.Abs(PrevZ - CurZ);

            // transform.rotation = StartRotation;
            transform.RotateAround(WallPivot, Vector3.left, -1 * angle * (PathTraveled / length));
            yield return new WaitForFixedUpdate();
            PrevZ = CurZ;
            CurZ = transform.position.z;
        }
    }

    void Start()
    {
        StartRotation = transform.rotation;
        StartCoords = transform.position;
        StartCoroutine(RotateToCamera());
    }
    void Update()
    {
        
    }
}