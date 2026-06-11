using UnityEngine;
using UnityEngine.InputSystem;

public class CursorScript : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject Lines;
    public GameObject SNB;
    public GameObject WNB;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(MainCamera.ScreenToWorldPoint(Input.mousePosition));
        transform.position = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        Lines.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, 2f);
        SNB.transform.position = new Vector3(MainCamera.transform.position.x + 8f, MainCamera.transform.position.y + 4f, 1f);
        WNB.transform.position = new Vector3(MainCamera.transform.position.x + 6.5f, MainCamera.transform.position.y + 4f, 1f);

        Vector3 MouseScroll = Mouse.current.scroll.ReadValue();

        if (MouseScroll.y < 0)
        {
            MainCamera.transform.position += new Vector3(0f, -1f, 0f);
        }
        else if (MouseScroll.y > 0)
        {
            MainCamera.transform.position += new Vector3(0f, 1f, 0f);
        }
    }
}