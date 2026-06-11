using System.Collections.Generic;
using UnityEngine;

public class spawnScript : MonoBehaviour
{
    // public float lane1 = -2.57f;
    // public float lane2 = -1.29f;
    // public float lane3 = 0f;
    // public float lane4 = 1.29f;
    // public float lane5 = 2.57f;

    private List<float> laneList = new List<float>() {-4f, -2f, 0f, 2f, 4f};
    // private List<float> laneList = new List<float>() {0f};
    public int randlane = 0;
    public float randpos = 0f;

    public GameObject note;
    
    public float spawntime = 2;
    public float timer = 0;
    public Vector3 spawnPos = new Vector3();
    void Start()
    {
        
    }


    void Update()
    {
        if (timer < spawntime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            randlane = Random.Range(0, 5);
            randpos = laneList[randlane];
            spawnPos = new Vector3(randpos, -4f, 14.53f);
            Instantiate(note, spawnPos, transform.rotation);
            spawntime = Random.Range(0.1f, 0.2f);
            timer = 0;
        }
        
    }
}
