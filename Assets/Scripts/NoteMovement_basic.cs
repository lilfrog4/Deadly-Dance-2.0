using UnityEngine;
public class NoteMovement_basic : MonoBehaviour

{
    public Transform Note_Transform;
    public int notespeed = 35;
    // private float inc;
    void Start()
    {
        
    }


    void Update()
    {
        // inc += 0.000005f;
        Note_Transform.Translate(Vector3.back * notespeed * 0.6f * Time.deltaTime, Space.World);
        // Note_Transform.Translate(Vector3.back * notespeed * inc, Space.World);
    }

}