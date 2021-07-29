using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class rappresent a dialogue object.
 * Every dialogue has a sequence of sentences
 * A Dialog might be skippable
 */
[System.Serializable]
public class Dialogue
{
    [TextArea(4, 10)]
    public string[] sentences;
}
