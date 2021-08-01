using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    

    [SerializeField] private SettingsPopup settingsPopup;
    [SerializeField] private DeathPopUp deathPopUp;
   

   

   

    // Start is called before the first frame update
    void Start()
    {
        deathPopUp.close();
        settingsPopup.close();
    }

    // Update is called once per frame
    void Update()
    {
        
        // scoreLabel.text = Time.realtimeSinceStartup.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsPopup.open();
        }
        
        else if (GameEvent.isDead)
        {
            deathPopUp.open();
            GameEvent.isDead = false;
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

    public void onOpenDeath()
    {
        deathPopUp.open();
    }

   public void  onClosingDeath()
    {
        deathPopUp.close();
    }
}
