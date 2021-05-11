using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    Camera _camera;
    PlayerStatus _status;
    GUIStyle _styleH;
    GUIStyle _styleS;
    GUIStyle _styleF;
    GUIStyle _styleD;
    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _status = GetComponent<PlayerStatus>();

        _styleH = new GUIStyle();
        _styleH.fontSize = 28;
        _styleH.normal.textColor = Color.red;

        _styleS = new GUIStyle();
        _styleS.fontSize = 28;
        _styleS.normal.textColor = Color.green;

        _styleF = new GUIStyle();
        _styleF.fontSize = 28;
        _styleF.normal.textColor = Color.white;

        _styleD = new GUIStyle();
        _styleD.fontSize = 48;
        _styleD.normal.textColor = Color.red;
    }
    
    private void OnGUI()
    {
        int size = 24;
        float posX = _camera.pixelWidth / 20;
        float posY = _camera.pixelHeight / 10;
        GUI.Label(new Rect(posX, posY, size, size), "Health\n" + GetBar(_status.GetHealth(), _status.GetMaxHealth()), _styleH);

        size = 24;
        posX = _camera.pixelWidth / 20;
        posY = _camera.pixelHeight / 10 + 64;
        GUI.Label(new Rect(posX, posY, size, size), "Stamina\n" + GetBar(_status.GetStamina(), _status.GetMaxStamina()), _styleS);

        size = 24;
        posX = _camera.pixelWidth / 20;
        posY = _camera.pixelHeight / 10 + 128;
        GUI.Label(new Rect(posX, posY, size, size), "Fuel\n" + GetBar(_status.GetFuel(), _status.GetMaxFuel()), _styleF);

        if(_status.IsAlive() == false)
        {
            size = 48;
            posX = _camera.pixelWidth / 2 - 128;
            posY = _camera.pixelHeight / 3;
            GUI.Label(new Rect(posX, posY, size, size), "SEI MORTO", _styleD);
        }
        
    }

    private string GetBar(float value, float maxValue)
    {
        string  s = "[ ";
        int cont = (int)Mathf.Ceil((value / maxValue)*10);
        int maxBar = 10;
        for(int i = 0; i < cont; i++)
        {
            s += "\\  ";
        }
        for(int i = 0; i < (maxBar - cont); i++)
        {
            s += "   ";
        }
        s += "]";
        return s;
    }
}
