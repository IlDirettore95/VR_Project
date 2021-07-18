using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    

    [SerializeField] private SettingsPopup settingsPopup;

   

   

   

    // Start is called before the first frame update
    void Start()
    {
        
        settingsPopup.close();
    }

    // Update is called once per frame
    void Update()
    {
        // scoreLabel.text = Time.realtimeSinceStartup.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameEvent.isPaused)
            {
                settingsPopup.close();
            }
            else
            {
                settingsPopup.open();
            }
            
        }
    }

   

    public void onOpenSettings()
    {
        settingsPopup.open();
    }

    public void onClosingSettings()
    {
        settingsPopup.close();
    }
}
