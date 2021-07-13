using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class rappresent a dialogue object.
 * Every dialogue has a sequence of sentences and is referred to a NPC's name
 */
[System.Serializable]
public class Dialogue
{
    public string[] sentences;
    [TextArea(3, 10)]
    public string name;
}
