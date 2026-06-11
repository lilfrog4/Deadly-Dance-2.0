using UnityEngine;

public class FuseFireLower_Script : MonoBehaviour
{
    private float InitialYPos;
    private float FinalYPos;
    private float PathLength;
    private float ZLength;
    private float Traveled;
    private float PlayerZCoord = -4.143f;
    private float InitialZcoord;
    void Start()
    {
        InitialZcoord = transform.parent.position.z;
        
        // FinalYPos = 0f;
        
        ZLength = Mathf.Abs(transform.position.z - PlayerZCoord);
        // Debug.Log(PathLength);
    }

    void Update()
    {
        InitialYPos = transform.parent.Find("FireStartPos").transform.position.y;
        FinalYPos = transform.root.position.y;
        PathLength = Mathf.Abs(InitialYPos - FinalYPos);

        Traveled = Mathf.Abs(InitialZcoord - transform.parent.position.z);
        // if (transform.parent.GetComponent<AppearAnimations>().AnimationInProcess == false)
        // {
        //     transform.position = new Vector3(transform.position.x, InitialYPos - PathLength * (Traveled / ZLength), transform.position.z);
        // }
        transform.position = new Vector3(transform.position.x, InitialYPos - PathLength * (Traveled / ZLength), transform.position.z);
    }
}