using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameSystem gameSys;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            gameSys.NewGame();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            gameSys.Continue();
        }
    }
}
