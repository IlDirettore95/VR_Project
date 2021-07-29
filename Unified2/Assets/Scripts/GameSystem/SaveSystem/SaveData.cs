using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable()]
public class SaveData
{
    public string _currentScene
    {
        get; set;
    }

    public int dp
    {
        get; set;
    }

    public int quality
    {
        get; set;
    }
    
    public SaveData(string sceneName, int dp, int quality)
    {
        _currentScene = sceneName;
        this.dp = dp;
        this.quality = quality;
    }
}
