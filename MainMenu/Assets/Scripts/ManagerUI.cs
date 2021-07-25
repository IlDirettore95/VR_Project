using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    [SerializeField] private Button options;
    [SerializeField] private Button exitGame;
    [SerializeField] private Button back;
    [SerializeField] private Dropdown dp;
    [SerializeField] private Text graphics;
    
    [SerializeField] private Image backGround;

    public void OpenSettings()
    {
        backGround.gameObject.SetActive(true);
        back.gameObject.SetActive(true);
        dp.gameObject.SetActive(true);
        graphics.enabled = true;
        
        newGame.gameObject.SetActive(false);
        continueGame.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        exitGame.gameObject.SetActive(false);
        
    }

    public void CloseSettings()
    {
        backGround.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        dp.gameObject.SetActive(false);
        graphics.enabled = false;
        
        newGame.gameObject.SetActive(true);
        continueGame.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        exitGame.gameObject.SetActive(true);
    }

    public void NewGame()
    {
        
    }

    public void ContinueGame()
    {
        
    }

    public void ExitGame()
    {
        
    }

    public void ChangeGraphics()
    {
        
    }
    
    
    
}
