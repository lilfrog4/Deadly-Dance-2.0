using UnityEngine;
using System;
using System.Collections.Generic;

public class TriggerValues : MonoBehaviour
{
    public int editorLane;
    // public int occupied_space;
    public int editorID;
    public string triggerType;
    public int usedGroup;
    public float delay;
    public float duration;

    public string spriteName;       // Для SpriteTrigger
    public float opacity;           // Для AlphaTrigger
}