using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    public string speaker = "Unknown";
    [TextArea(3,10)]
    public string[] sentences;

    public float TimeStamp = 3f;
}
