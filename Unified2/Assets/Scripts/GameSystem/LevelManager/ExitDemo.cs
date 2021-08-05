using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(6f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Loader.Load("MainMenu");
    }
}
